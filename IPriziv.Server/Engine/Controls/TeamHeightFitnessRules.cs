using System.Text.RegularExpressions;
using IPriziv.Server.Engine.Health;

namespace IPriziv.Server.Engine.Controls
{
    /// <summary>
    /// Требования по росту при комплектовании (модалка подходящих команд).
    /// </summary>
    public static class TeamHeightFitnessRules
    {
        /// <summary>Танковые части: водители/экипаж — рост до 175.</summary>
        public static readonly IReadOnlyList<string> TankCrewUnitNumbers = new[]
        {
            "19612", "61899", "76736", "84993", "71422"
        };

        /// <summary>Мотострелковые части: водители/экипаж — рост до 180.</summary>
        public static readonly IReadOnlyList<string> MotorRifleCrewUnitNumbers = new[]
        {
            "23626", "61899"
        };

        private static readonly Regex UnitNumberRegex = new(
            @"\d{4,6}",
            RegexOptions.Compiled);

        private static readonly Regex GuardDutyRegex = new(
            @"караул",
            RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);

        private static readonly Regex TankTypeRegex = new(
            @"танк",
            RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);

        private static readonly Regex MotorRifleTypeRegex = new(
            @"мотострел|мсп\b|мсд\b",
            RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);

        public readonly record struct HeightConstraint(byte? MinCm, byte? MaxCm, string Label);

        public static HeightConstraint? ResolveConstraint(
            TeamImpliedUnitRequirementRules.UnitRequirementKind kind,
            string militaryUnitName,
            string militaryTypeName,
            string teamNote,
            IEnumerable<string> alternativeNames,
            bool isDriverOrCrewRole)
        {
            if (kind is TeamImpliedUnitRequirementRules.UnitRequirementKind.Spetsnaz
                or TeamImpliedUnitRequirementRules.UnitRequirementKind.MarineInfantry
                or TeamImpliedUnitRequirementRules.UnitRequirementKind.Vdv)
            {
                var label = TeamImpliedUnitRequirementRules.FormatKindLabel(kind) ?? "часть";
                return new HeightConstraint(170, 185, label);
            }

            if (kind == TeamImpliedUnitRequirementRules.UnitRequirementKind.NavalShip)
                return new HeightConstraint(null, 185, "корабельный ВМФ");

            if (isDriverOrCrewRole)
            {
                var tank = MatchesAnyUnitNumber(
                    TankCrewUnitNumbers, militaryUnitName, alternativeNames, teamNote);
                var msv = MatchesAnyUnitNumber(
                    MotorRifleCrewUnitNumbers, militaryUnitName, alternativeNames, teamNote);

                if (tank && msv)
                {
                    if (LooksLikeMotorRifle(militaryTypeName, teamNote))
                        return new HeightConstraint(null, 180, "мотострелковая часть (водитель/экипаж)");
                    return new HeightConstraint(null, 175, "танковая часть (водитель/экипаж)");
                }

                if (tank)
                    return new HeightConstraint(null, 175, "танковая часть (водитель/экипаж)");
                if (msv)
                    return new HeightConstraint(null, 180, "мотострелковая часть (водитель/экипаж)");
            }

            var joined = string.Join(" ",
                new[] { militaryUnitName, militaryTypeName, teamNote }
                    .Where(x => !string.IsNullOrWhiteSpace(x)));
            if (alternativeNames != null)
                joined = string.Join(" ", new[] { joined }.Concat(alternativeNames.Where(x => !string.IsNullOrWhiteSpace(x))));

            if (TeamHealthFitnessRules.IsNationalGuard(militaryTypeName)
                || TeamHealthFitnessRules.IsNationalGuard(joined)
                || IsGuardDuty(joined))
            {
                return new HeightConstraint(155, null, "НГ / караульная часть");
            }

            return null;
        }

        public static TeamHeightFitnessVerdict Evaluate(
            byte? heightCm,
            TeamImpliedUnitRequirementRules.UnitRequirementKind kind,
            string militaryUnitName,
            string militaryTypeName,
            string teamNote,
            IEnumerable<string> alternativeNames,
            bool isDriverOrCrewRole)
        {
            var constraint = ResolveConstraint(
                kind,
                militaryUnitName,
                militaryTypeName,
                teamNote,
                alternativeNames,
                isDriverOrCrewRole);

            if (constraint == null)
                return TeamHeightFitnessVerdict.Fit();

            var min = constraint.Value.MinCm;
            var max = constraint.Value.MaxCm;
            var label = constraint.Value.Label;
            var rangeLabel = FormatRange(min, max);

            if (heightCm is null or 0)
            {
                return TeamHeightFitnessVerdict.Unfit(
                    "missing_height",
                    $"Рост не указан для {label}; нужно: {rangeLabel}");
            }

            if (min.HasValue && heightCm.Value < min.Value)
            {
                return TeamHeightFitnessVerdict.Unfit(
                    "height_mismatch",
                    $"Рост {heightCm} см не подходит для {label}; нужно: {rangeLabel}");
            }

            if (max.HasValue && heightCm.Value > max.Value)
            {
                return TeamHeightFitnessVerdict.Unfit(
                    "height_mismatch",
                    $"Рост {heightCm} см не подходит для {label}; нужно: {rangeLabel}");
            }

            return TeamHeightFitnessVerdict.Fit();
        }

        public static bool IsGuardDuty(string text)
            => !string.IsNullOrWhiteSpace(text) && GuardDutyRegex.IsMatch(text);

        public static bool IsDriverOrCrewRole(byte? specialtyId)
            => ControlDriverSpecialtyRules.IsDriverSpecialty(specialtyId);

        public static bool LooksLikeTank(string text)
            => !string.IsNullOrWhiteSpace(text) && TankTypeRegex.IsMatch(text);

        public static bool LooksLikeMotorRifle(params string[] texts)
        {
            foreach (var text in texts)
            {
                if (!string.IsNullOrWhiteSpace(text) && MotorRifleTypeRegex.IsMatch(text))
                    return true;
            }
            return false;
        }

        public static string FormatRange(byte? minCm, byte? maxCm)
        {
            if (minCm.HasValue && maxCm.HasValue)
                return $"{minCm}–{maxCm} см";
            if (minCm.HasValue)
                return $"от {minCm} см";
            if (maxCm.HasValue)
                return $"до {maxCm} см";
            return "без ограничений";
        }

        private static bool MatchesAnyUnitNumber(
            IReadOnlyList<string> numbers,
            string militaryUnitName,
            IEnumerable<string> alternativeNames,
            string teamNote)
        {
            foreach (var number in numbers)
            {
                if (ContainsUnitNumber(militaryUnitName, number))
                    return true;
                if (ContainsUnitNumber(teamNote, number))
                    return true;
                if (alternativeNames == null)
                    continue;
                foreach (var alt in alternativeNames)
                {
                    if (ContainsUnitNumber(alt, number))
                        return true;
                }
            }

            return false;
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

    public sealed class TeamHeightFitnessVerdict
    {
        public bool IsFit { get; init; }
        public string ReasonCode { get; init; }
        public string Reason { get; init; }

        public static TeamHeightFitnessVerdict Fit()
            => new() { IsFit = true, ReasonCode = "ok" };

        public static TeamHeightFitnessVerdict Unfit(string reasonCode, string reason)
            => new() { IsFit = false, ReasonCode = reasonCode, Reason = reason };
    }
}
