using System.Text.RegularExpressions;

namespace IPriziv.Server.Engine.Controls
{
    /// <summary>
    /// Контроли с пометкой «Москва» / «Мск» подходят к списку московских в/ч.
    /// </summary>
    public static class ControlMoscowUnitRules
    {
        /// <summary>Номера и текстовые ключи московских частей.</summary>
        public static readonly IReadOnlyList<string> MoscowUnitKeys = new[]
        {
            "61899",
            "ВУНЦ СВ \"ОА ВС РФ\"",
            "Московское ВОКУ",
            "53981",
            "72064",
            "504 пож.команда",
            "75384",
            "01904",
            "48428",
            "63791",
            "85098",
            "Военная комендатура",
            "3 ВОО почетного караула",
            "21 Военный оркестр МО РФ",
            "46588",
            "Военный университет МО РФ",
            "55001",
            "58138",
            "1019 КП ПВО",
            "96103",
            "44696",
            "83466",
            "53982",
            "48905",
            "38 НИИ БТВТ",
            "АПиП Александрова",
            "ФГУК \"ЦВО\" МО",
            "26000",
            "80705",
            "25776",
            "95555",
            "ЦВК по ОД Росгвардии",
            "7456",
            "5401",
            "3270",
            "3553",
            "5583",
            "3272",
            "1005",
            "35489",
        };

        private static readonly HashSet<string> MoscowNumbers = new(
            MoscowUnitKeys
                .Select(NormalizeUnitNumberKey)
                .Where(x => x.Length > 0),
            StringComparer.Ordinal);

        private static readonly IReadOnlyList<string> MoscowNamedKeys = MoscowUnitKeys
            .Where(k => !IsMostlyDigits(k))
            .ToArray();

        /// <summary>Мск, Москва, московск… (не «Московская область»).</summary>
        private static readonly Regex MoscowMarkerRegex = new(
            @"\bмск\b|"
            + @"\bмосква\b|"
            + @"\bг\.?\s*москва\b|"
            + @"московск\w*(?!\s+обл)",
            RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);

        public static bool LooksLikeMoscow(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return false;
            // область важнее города: «Московская область» не городская Москва
            if (ControlMoscowOblastUnitRules.LooksLikeMoscowOblast(text))
                return false;
            return MoscowMarkerRegex.IsMatch(text);
        }

        public static bool IsMoscowUnitNumber(string unitNumber)
        {
            var key = NormalizeUnitNumberKey(unitNumber);
            return key.Length > 0 && MoscowNumbers.Contains(key);
        }

        public static bool TeamIsMoscowUnit(IEnumerable<string> teamUnitTokens)
        {
            if (teamUnitTokens == null)
                return false;

            foreach (var token in teamUnitTokens)
            {
                if (string.IsNullOrWhiteSpace(token))
                    continue;

                if (IsMoscowUnitNumber(token))
                    return true;

                foreach (var number in ControlTeamMatchRules.ExtractUnitNumbers(token))
                {
                    if (IsMoscowUnitNumber(number))
                        return true;
                }

                foreach (var named in MoscowNamedKeys)
                {
                    if (token.Equals(named, StringComparison.OrdinalIgnoreCase))
                        return true;
                    // только если в токене команды есть полное имя части, не наоборот
                    // (иначе короткий «СВ» цепляет «ВУНЦ СВ …»)
                    if (named.Length >= 6
                        && token.Length >= named.Length
                        && token.Contains(named, StringComparison.OrdinalIgnoreCase))
                        return true;
                }
            }

            return false;
        }

        public static bool MatchesMoscowControl(
            string controlPlace,
            string controlMilitaryUnitName,
            string controlDisplayName,
            IReadOnlyList<string> teamUnitTokens)
        {
            var text = string.Join(" ",
                new[] { controlPlace, controlMilitaryUnitName, controlDisplayName }
                    .Where(x => !string.IsNullOrWhiteSpace(x)));

            if (!LooksLikeMoscow(text))
                return false;

            return TeamIsMoscowUnit(teamUnitTokens);
        }

        private static bool IsMostlyDigits(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;
            var digits = value.Count(char.IsDigit);
            return digits >= 4 && digits >= value.Count(c => !char.IsWhiteSpace(c)) / 2;
        }

        private static string NormalizeUnitNumberKey(string unitNumber)
        {
            if (string.IsNullOrWhiteSpace(unitNumber))
                return string.Empty;

            var digits = new string(unitNumber.Where(char.IsDigit).ToArray());
            // «1019 КП ПВО» / «504 пож» — короткие номера тоже учитываем, если ключ почти числовой
            if (digits.Length < 3)
                return string.Empty;

            var trimmed = digits.TrimStart('0');
            return trimmed.Length == 0 ? string.Empty : trimmed;
        }
    }
}
