using System.Text.RegularExpressions;

namespace IPriziv.Server.Engine.Controls
{
    /// <summary>
    /// Контроли с пометкой «МО» / «Московская область» подходят
    /// к списку частей Московской области.
    /// </summary>
    public static class ControlMoscowOblastUnitRules
    {
        /// <summary>Номера и текстовые ключи частей МО.</summary>
        public static readonly IReadOnlyList<string> MoscowOblastUnitKeys = new[]
        {
            "23626",
            "76736",
            "84993",
            "71422",
            "887 КП (ПВО ОА)",
            "19612",
            "55229-2",
            "45603-2",
            "49371",
            "ФАУ МО РФ \"ЦСКА\"",
            "11616",
            "11606",
            "56014",
            "11361",
            "64248",
            "41516",
            "190 ВШП",
            "55597",
            "36290",
            "54169-ГС",
            "43034",
            "74400",
            "33790-Б",
            "33790-Г",
            "95501",
            "ВА П.Великого",
            "03523",
            "26178",
            "17204",
            "03340",
            "32103",
            "92925",
            "92574",
            "42113",
            "61996",
            "62843",
            "42829",
            "15681",
            "12556",
            "58172",
            "03667",
            "28289",
            "52399",
            "54164",
            "93723",
            "28337",
            "43292",
            "43099",
            "03770",
            "34608",
            "36360",
            "47126",
            "30258",
            "92154",
            "25503",
            "30663",
            "25801-11",
            "25801-13",
            "61235",
            "Военно-Инженерная Академия",
            "55591",
            "44026",
            "19889",
            "85084",
            "73535",
            "74036",
            "32516",
            "11300",
            "33877",
            "3641",
            "3111",
            "3472",
            "6686",
            "ГВКГ войск НГ",
            "84411",
            "73407",
        };

        private static readonly HashSet<string> HyphenatedKeys = new(
            MoscowOblastUnitKeys
                .Where(k => k.Contains('-', StringComparison.Ordinal))
                .Select(NormalizeFullKey),
            StringComparer.OrdinalIgnoreCase);

        private static readonly HashSet<string> PlainNumbers = new(
            MoscowOblastUnitKeys
                .Where(k => !k.Contains('-', StringComparison.Ordinal))
                .Select(NormalizeDigitsOnly)
                .Where(x => x.Length > 0),
            StringComparer.Ordinal);

        private static readonly IReadOnlyList<string> NamedKeys = MoscowOblastUnitKeys
            .Where(k => !IsMostlyDigitsOrHyphenatedNumber(k))
            .ToArray();

        /// <summary>
        /// «Московская область», «мо обл», целое поле «МО»/«Мо».
        /// Не цепляет «МО РФ» и не путает с городом Москвой.
        /// </summary>
        private static readonly Regex LongOblastMarkerRegex = new(
            @"московск\w*\s+обл\w*|"
            + @"\bмос?\.?\s*обл(?:асть|асти|\.)?",
            RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);

        private static readonly Regex ShortMoMarkerRegex = new(
            @"^(?:мо|мос)\.?$",
            RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);

        // «направление МО», но не «МО РФ» и не «Москва»
        private static readonly Regex EmbeddedMoMarkerRegex = new(
            @"(?<![А-Яа-яЁёA-Za-z0-9])мо(?![А-Яа-яЁёA-Za-z0-9])(?!\s*рф)",
            RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);

        public static bool LooksLikeMoscowOblast(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return false;

            var trimmed = text.Trim();
            if (ShortMoMarkerRegex.IsMatch(trimmed))
                return true;

            if (LongOblastMarkerRegex.IsMatch(trimmed))
                return true;

            // «направление МО» — только если нет явного «МО РФ»
            if (EmbeddedMoMarkerRegex.IsMatch(trimmed)
                && !trimmed.Contains("мо рф", StringComparison.OrdinalIgnoreCase)
                && !trimmed.Contains("москва", StringComparison.OrdinalIgnoreCase))
                return true;

            return false;
        }

        public static bool IsMoscowOblastUnitNumber(string unitNumber)
        {
            if (string.IsNullOrWhiteSpace(unitNumber))
                return false;

            var full = NormalizeFullKey(unitNumber);
            if (HyphenatedKeys.Contains(full))
                return true;

            // hyphenated list keys must match fully (25801 ≠ 25801-11)
            if (unitNumber.Contains('-', StringComparison.Ordinal))
                return false;

            var digits = NormalizeDigitsOnly(unitNumber);
            return digits.Length > 0 && PlainNumbers.Contains(digits);
        }

        public static bool TeamIsMoscowOblastUnit(IEnumerable<string> teamUnitTokens)
        {
            if (teamUnitTokens == null)
                return false;

            foreach (var token in teamUnitTokens)
            {
                if (string.IsNullOrWhiteSpace(token))
                    continue;

                if (IsMoscowOblastUnitNumber(token))
                    return true;

                var full = NormalizeFullKey(token);
                if (HyphenatedKeys.Contains(full))
                    return true;

                foreach (var hk in HyphenatedKeys)
                {
                    if (token.Contains(hk, StringComparison.OrdinalIgnoreCase))
                        return true;
                }

                foreach (var number in ControlTeamMatchRules.ExtractUnitNumbers(token))
                {
                    if (IsMoscowOblastUnitNumber(number))
                        return true;
                }

                foreach (var named in NamedKeys)
                {
                    if (token.Equals(named, StringComparison.OrdinalIgnoreCase))
                        return true;
                    if (named.Length >= 6
                        && token.Length >= named.Length
                        && token.Contains(named, StringComparison.OrdinalIgnoreCase))
                        return true;
                }
            }

            return false;
        }

        public static bool MatchesMoscowOblastControl(
            string controlPlace,
            string controlMilitaryUnitName,
            string controlDisplayName,
            IReadOnlyList<string> teamUnitTokens)
        {
            // поля по отдельности: иначе «МО» в «МО РФ» из имени части даёт ложный маркер
            var hasMarker =
                LooksLikeMoscowOblast(controlPlace)
                || LooksLikeMoscowOblast(controlMilitaryUnitName)
                || LooksLikeMoscowOblast(controlDisplayName);

            if (!hasMarker)
                return false;

            return TeamIsMoscowOblastUnit(teamUnitTokens);
        }

        private static bool IsMostlyDigitsOrHyphenatedNumber(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;
            if (value.Contains('-', StringComparison.Ordinal)
                && value.Any(char.IsDigit))
                return true;
            var digits = value.Count(char.IsDigit);
            return digits >= 3 && digits >= value.Count(c => !char.IsWhiteSpace(c)) / 2;
        }

        private static string NormalizeFullKey(string unitNumber)
        {
            if (string.IsNullOrWhiteSpace(unitNumber))
                return string.Empty;
            return unitNumber.Trim().ToLowerInvariant();
        }

        private static string NormalizeDigitsOnly(string unitNumber)
        {
            if (string.IsNullOrWhiteSpace(unitNumber))
                return string.Empty;

            var digits = new string(unitNumber.Where(char.IsDigit).ToArray());
            if (digits.Length < 3)
                return string.Empty;

            var trimmed = digits.TrimStart('0');
            return trimmed.Length == 0 ? string.Empty : trimmed;
        }
    }
}
