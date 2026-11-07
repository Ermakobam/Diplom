using System.Text.RegularExpressions;

namespace IPriziv.Server.Engine.Controls
{
    /// <summary>
    /// Контроли с пометкой «подальше» / отдалённости подходят
    /// только к списку дальних в/ч (в т.ч. 54046 → получатель 91711).
    /// </summary>
    public static class ControlFarAwayUnitRules
    {
        /// <summary>
        /// Номера дальних частей. 91711 — получатель для 54046.
        /// </summary>
        public static readonly IReadOnlyList<string> FarAwayUnitNumbers = new[]
        {
            "54046", "91711",
            "30616", "79187", "29760", "83497", "61460", "20155", "13766",
            "77165", "06982", "06984", "13076", "14245", "89553", "35700",
            "75376", "16605", "40963", "48514", "40213", "11754", "44440",
            "36138", "01907", "45117", "28037", "81819", "30114", "52577", "61756",
        };

        private static readonly HashSet<string> FarAwayNormalized = new(
            FarAwayUnitNumbers.Select(NormalizeUnitNumberKey).Where(x => x.Length > 0),
            StringComparer.Ordinal);

        /// <summary>
        /// Пары «часть → получатель» (и обратно): 54046 ↔ 91711.
        /// </summary>
        private static readonly IReadOnlyDictionary<string, string[]> RecipientPairs =
            new Dictionary<string, string[]>(StringComparer.Ordinal)
            {
                ["54046"] = new[] { "91711" },
                ["91711"] = new[] { "54046" },
            };

        /// <summary>
        /// Доп. номера получателей для токенов команды (54046 ↔ 91711).
        /// </summary>
        public static IEnumerable<string> ExpandRecipientNumbers(IEnumerable<string> seeds)
        {
            if (seeds == null)
                yield break;

            var seen = new HashSet<string>(StringComparer.Ordinal);
            foreach (var seed in seeds)
            {
                if (string.IsNullOrWhiteSpace(seed))
                    continue;

                foreach (var number in ControlTeamMatchRules.ExtractUnitNumbers(seed)
                             .Append(NormalizeUnitNumberKey(seed))
                             .Where(x => x.Length > 0))
                {
                    var key = NormalizeUnitNumberKey(number);
                    if (key.Length == 0 || !RecipientPairs.TryGetValue(key, out var partners))
                        continue;

                    foreach (var partner in partners)
                    {
                        if (seen.Add(partner))
                            yield return partner;
                    }
                }
            }
        }

        /// <summary>
        /// «подальше», «по дальше», «дальше», «отдалённ…», «далекие/дальние» и т.п.
        /// </summary>
        private static readonly Regex FarAwayMarkerRegex = new(
            @"по\s*дальше|"
            + @"\bдальше\b|"
            + @"отдал[её]нн?\w*|"
            + @"\bдал[её]к\w*|"
            + @"\bдальн\w*|"
            + @"\bудал[её]нн?\w*",
            RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);

        public static bool LooksLikeFarAway(string text)
            => !string.IsNullOrWhiteSpace(text) && FarAwayMarkerRegex.IsMatch(text);

        public static bool IsFarAwayUnitNumber(string unitNumber)
        {
            var key = NormalizeUnitNumberKey(unitNumber);
            return key.Length > 0 && FarAwayNormalized.Contains(key);
        }

        public static bool TeamIsFarAwayUnit(IEnumerable<string> teamUnitTokens)
        {
            if (teamUnitTokens == null)
                return false;

            foreach (var token in teamUnitTokens)
            {
                if (IsFarAwayUnitNumber(token))
                    return true;

                foreach (var number in ControlTeamMatchRules.ExtractUnitNumbers(token))
                {
                    if (IsFarAwayUnitNumber(number))
                        return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Контроль «подальше» подходит команде из списка дальних в/ч.
        /// </summary>
        public static bool MatchesFarAwayControl(
            string controlPlace,
            string controlMilitaryUnitName,
            string controlDisplayName,
            IReadOnlyList<string> teamUnitTokens)
        {
            var text = string.Join(" ",
                new[] { controlPlace, controlMilitaryUnitName, controlDisplayName }
                    .Where(x => !string.IsNullOrWhiteSpace(x)));

            if (!LooksLikeFarAway(text))
                return false;

            return TeamIsFarAwayUnit(teamUnitTokens);
        }

        private static string NormalizeUnitNumberKey(string unitNumber)
        {
            if (string.IsNullOrWhiteSpace(unitNumber))
                return string.Empty;

            var digits = new string(unitNumber.Where(char.IsDigit).ToArray());
            var trimmed = digits.TrimStart('0');
            return trimmed.Length == 0 ? string.Empty : trimmed;
        }
    }
}
