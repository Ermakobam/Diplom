using System.Text.RegularExpressions;
using IPriziv.Server.Data.Enumerations;
using IPriziv.Server.Engine.Recruits;

namespace IPriziv.Server.Engine.Controls
{
    /// <summary>
    /// Неявные требования по типу/номеру части (поверх наряда):
    /// спецназ 92154/43292 и морская пехота — к-95б + 3 форма допуска;
    /// корабельный ВМФ — к-95а + 3 форма + категория не хуже Б-2.
    /// </summary>
    public static class TeamImpliedUnitRequirementRules
    {
        public const string SpetsnazRegime = "95б";
        public const string MarineRegime = "95б";
        public const string NavalShipRegime = "95а";
        public const string NavalShipMaxHealthCategory = "Б-2";

        public static readonly IReadOnlyList<string> SpetsnazUnitNumbers = new[] { "92154", "43292" };

        private static readonly Regex UnitNumberRegex = new(
            @"\d{4,6}",
            RegexOptions.Compiled);

        private static readonly Regex MarineInfantryRegex = new(
            @"морск\w*\s*пехот|морпех|\bмп\b",
            RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);

        private static readonly Regex NavalShipMarkerRegex = new(
            @"кораб|плавсостав|надводн|подводн|плавбаз|"
            + @"\bбдк\b|\bнк\b|\bскр\b|\bмрк\b|\bмпк\b|"
            + @"эсминец|крейсер|фрегат|тральщик|катер",
            RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);

        public enum UnitRequirementKind
        {
            None = 0,
            Spetsnaz = 1,
            MarineInfantry = 2,
            NavalShip = 3,
            Vdv = 4,
            Semenovsky = 5,
            Preobrazhensky = 6,
        }

        private static readonly Regex SemenovskyRegex = new(
            @"сем[её]новск",
            RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);

        private static readonly Regex PreobrazhenskyRegex = new(
            @"преображенск",
            RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);

        public static UnitRequirementKind Detect(
            string militaryUnitName,
            string militaryTypeName,
            string teamNote,
            IEnumerable<string> alternativeNames = null)
        {
            var texts = new List<string>();
            void Add(string value)
            {
                if (!string.IsNullOrWhiteSpace(value))
                    texts.Add(value);
            }

            Add(militaryUnitName);
            Add(militaryTypeName);
            Add(teamNote);
            if (alternativeNames != null)
            {
                foreach (var alt in alternativeNames)
                    Add(alt);
            }

            var joined = string.Join(" ", texts);
            if (IsSpetsnazUnit(joined, militaryUnitName, alternativeNames))
                return UnitRequirementKind.Spetsnaz;

            if (IsSemenovsky(joined))
                return UnitRequirementKind.Semenovsky;

            if (IsPreobrazhensky(joined))
                return UnitRequirementKind.Preobrazhensky;

            if (IsMarineInfantry(joined))
                return UnitRequirementKind.MarineInfantry;

            if (IsNavalShipService(militaryTypeName, joined))
                return UnitRequirementKind.NavalShip;

            if (IsVdv(militaryTypeName) || IsVdv(joined))
                return UnitRequirementKind.Vdv;

            return UnitRequirementKind.None;
        }

        public static bool IsSemenovsky(string text)
            => !string.IsNullOrWhiteSpace(text) && SemenovskyRegex.IsMatch(text);

        public static bool IsPreobrazhensky(string text)
            => !string.IsNullOrWhiteSpace(text) && PreobrazhenskyRegex.IsMatch(text);

        public static bool IsVdv(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return false;
            return text.Contains("ВДВ", StringComparison.OrdinalIgnoreCase)
                   || text.Contains("воздушно-десант", StringComparison.OrdinalIgnoreCase)
                   || text.Contains("воздушно десант", StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsSpetsnazUnit(
            string joinedText,
            string militaryUnitName,
            IEnumerable<string> alternativeNames = null)
        {
            foreach (var number in SpetsnazUnitNumbers)
            {
                if (ContainsUnitNumber(militaryUnitName, number))
                    return true;
                if (alternativeNames != null)
                {
                    foreach (var alt in alternativeNames)
                    {
                        if (ContainsUnitNumber(alt, number))
                            return true;
                    }
                }
                if (!string.IsNullOrWhiteSpace(joinedText)
                    && ContainsUnitNumber(joinedText, number))
                    return true;
            }

            return false;
        }

        public static bool IsMarineInfantry(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return false;
            // «морская авиация» не морпех
            if (text.Contains("авиац", StringComparison.OrdinalIgnoreCase)
                && !text.Contains("пехот", StringComparison.OrdinalIgnoreCase)
                && !text.Contains("морпех", StringComparison.OrdinalIgnoreCase))
                return false;
            return MarineInfantryRegex.IsMatch(text);
        }

        public static bool IsNavalShipService(string militaryTypeName, string text)
        {
            if (!LooksLikeNavy(militaryTypeName) && !LooksLikeNavy(text))
                return false;
            return NavalShipMarkerRegex.IsMatch(text ?? "");
        }

        public static bool LooksLikeNavy(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return false;
            return text.Contains("ВМФ", StringComparison.OrdinalIgnoreCase)
                   || text.Contains("военно-морск", StringComparison.OrdinalIgnoreCase)
                   || text.Contains("военно морск", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Добавляет неявный режим наряда (к-95а/к-95б), если его ещё нет среди слотов.
        /// </summary>
        public static IReadOnlyList<TeamAdmissionSlot> AugmentAdmissionSlots(
            UnitRequirementKind kind,
            IReadOnlyList<TeamAdmissionSlot> slots)
        {
            var list = (slots ?? Array.Empty<TeamAdmissionSlot>()).ToList();
            var regime = kind switch
            {
                UnitRequirementKind.Spetsnaz => SpetsnazRegime,
                UnitRequirementKind.MarineInfantry => MarineRegime,
                UnitRequirementKind.NavalShip => NavalShipRegime,
                _ => null
            };
            if (regime == null)
                return list;

            var hasFamily95 = list.Any(s =>
                TeamAdmissionFitnessRules.TryMapRegimeToAdmissionForm(s.RegimeName, out var form, out var code)
                && form == (byte)AdmissionType.Third
                && code != null
                && code.StartsWith("95", StringComparison.OrdinalIgnoreCase));

            if (!hasFamily95)
                list.Add(new TeamAdmissionSlot { RegimeName = $"к-{regime}" });

            return list;
        }

        /// <summary>
        /// Для спецназа / МП / корабельного ВМФ нужна именно 3 форма допуска.
        /// </summary>
        public static bool RequiresThirdAdmissionForm(UnitRequirementKind kind)
            => kind is UnitRequirementKind.Spetsnaz
                or UnitRequirementKind.MarineInfantry
                or UnitRequirementKind.NavalShip;

        public static string FormatKindLabel(UnitRequirementKind kind)
            => kind switch
            {
                UnitRequirementKind.Spetsnaz => "спецназ",
                UnitRequirementKind.MarineInfantry => "морская пехота",
                UnitRequirementKind.NavalShip => "корабельный ВМФ",
                UnitRequirementKind.Vdv => "ВДВ",
                UnitRequirementKind.Semenovsky => "Семёновский полк",
                UnitRequirementKind.Preobrazhensky => "Преображенский полк",
                _ => null
            };

        public static string RequiredRegimeLabel(UnitRequirementKind kind)
            => kind switch
            {
                UnitRequirementKind.Spetsnaz => $"к-{SpetsnazRegime}",
                UnitRequirementKind.MarineInfantry => $"к-{MarineRegime}",
                UnitRequirementKind.NavalShip => $"к-{NavalShipRegime}",
                _ => null
            };

        /// <summary>
        /// Корабельный ВМФ: категория не хуже Б-2 (А-1…Б-2).
        /// </summary>
        public static bool RequiresNavalShipHealthCeiling(UnitRequirementKind kind)
            => kind == UnitRequirementKind.NavalShip;

        /// <summary>
        /// Допустимый диапазон НПУ для вида части (включительно).
        /// Спецназ / МП / корабли: 9–10; ВДВ / Семёновский / Преображенский: 8–10; иначе: 6–10.
        /// </summary>
        public static (byte Min, byte Max) GetAllowedNpuRange(UnitRequirementKind kind)
            => kind switch
            {
                UnitRequirementKind.Spetsnaz
                    or UnitRequirementKind.MarineInfantry
                    or UnitRequirementKind.NavalShip => (9, 10),
                UnitRequirementKind.Vdv
                    or UnitRequirementKind.Semenovsky
                    or UnitRequirementKind.Preobrazhensky => (8, 10),
                _ => (RecruitNpuRules.Min, RecruitNpuRules.Max),
            };

        public static TeamNpuFitnessVerdict EvaluateNpu(
            byte? recruitNpu,
            UnitRequirementKind kind)
        {
            var (min, max) = GetAllowedNpuRange(kind);
            var rangeLabel = min == max ? min.ToString() : $"{min}–{max}";
            var kindLabel = FormatKindLabel(kind);

            if (recruitNpu == null)
            {
                var where = string.IsNullOrWhiteSpace(kindLabel)
                    ? $"НПУ не указан; нужно: {rangeLabel}"
                    : $"НПУ не указан для {kindLabel}; нужно: {rangeLabel}";
                return TeamNpuFitnessVerdict.Unfit("missing_npu", where);
            }

            if (recruitNpu.Value < min || recruitNpu.Value > max)
            {
                var where = string.IsNullOrWhiteSpace(kindLabel)
                    ? $"НПУ {recruitNpu} не подходит; нужно: {rangeLabel}"
                    : $"НПУ {recruitNpu} не подходит для {kindLabel}; нужно: {rangeLabel}";
                return TeamNpuFitnessVerdict.Unfit("npu_mismatch", where);
            }

            return TeamNpuFitnessVerdict.Fit();
        }

        private static bool ContainsUnitNumber(string text, string unitNumber)
        {
            if (string.IsNullOrWhiteSpace(text) || string.IsNullOrWhiteSpace(unitNumber))
                return false;

            foreach (Match match in UnitNumberRegex.Matches(text))
            {
                if (NormalizeUnitNumber(match.Value) == NormalizeUnitNumber(unitNumber))
                    return true;
            }

            return text.Contains(unitNumber, StringComparison.Ordinal);
        }

        private static string NormalizeUnitNumber(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return "";
            var digits = new string(value.Where(char.IsDigit).ToArray());
            return digits.TrimStart('0');
        }
    }

    public sealed class TeamNpuFitnessVerdict
    {
        public bool IsFit { get; init; }
        public string ReasonCode { get; init; }
        public string Reason { get; init; }

        public static TeamNpuFitnessVerdict Fit()
            => new() { IsFit = true, ReasonCode = "ok" };

        public static TeamNpuFitnessVerdict Unfit(string reasonCode, string reason)
            => new() { IsFit = false, ReasonCode = reasonCode, Reason = reason };
    }
}
