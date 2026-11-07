using System;
using System.Text.RegularExpressions;

namespace IPriziv.Server.Engine.Controls
{
    /// <summary>
    /// Правила соответствия контроля команде (в/ч + род войск) для комплектования.
    /// Place в БД бывает: «в/ч 85084», «направление вч41516», номер части,
    /// род войск (РВСН/ВКС/СВ/… — любая часть этого рода),
    /// либо НЕ назначение (округ САО/ВАО, «Судимые», специальность) — такие Place не считаем destination.
    /// </summary>
    public static class ControlTeamMatchRules
    {
        private static readonly Regex UnitNumberRegex = new(
            @"\d{4,6}",
            RegexOptions.Compiled);

        private static readonly Regex UnitMarkerRegex = new(
            @"\bв\s*/?\s*ч\b|\bвч\b|\bвоенн\w*\s+част",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// Части ВКС, относящиеся к ПВО. Остальные части ВКС к ПВО не относятся.
        /// </summary>
        public static readonly IReadOnlyList<string> VksPvoUnitNumbers = new[]
        {
            "03523", "92925", "61996", "62843", "92574", "43856", "40963"
        };

        private static readonly HashSet<string> VksPvoUnitNumbersNormalized = new(
            VksPvoUnitNumbers.Select(NormalizeUnitNumberKey),
            StringComparer.Ordinal);

        /// <summary>Известные обозначения родов/видов войск в текстах контролей.</summary>
        private static readonly Regex MilitaryBranchRegex = new(
            @"\b("
            + @"РВСН|ВКС|ВДВ|ВМФ|ВВС|ПВО|РЭБ|"
            + @"Росгварди\w*|РГ|НГ|"
            + @"ФСБ|МЧС|ЦП|ЦОВУ|"
            + @"ГУ\s*ГШ|"
            + @"сухопутн\w*|"
            + @"СВ"
            + @")\b",
            RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);

        /// <summary>
        /// Номер части ВКС из списка ПВО (с учётом ведущих нулей: 03523 ≡ 3523).
        /// </summary>
        public static bool IsVksPvoUnitNumber(string unitNumber)
        {
            if (string.IsNullOrWhiteSpace(unitNumber))
                return false;

            var key = NormalizeUnitNumberKey(unitNumber);
            return key.Length > 0 && VksPvoUnitNumbersNormalized.Contains(key);
        }

        /// <summary>
        /// В токенах команды есть номер части ВКС-ПВО.
        /// </summary>
        public static bool TeamHasVksPvoUnit(IEnumerable<string> teamUnitTokens)
        {
            if (teamUnitTokens == null)
                return false;

            foreach (var token in teamUnitTokens)
            {
                foreach (var number in ExtractUnitNumbers(token))
                {
                    if (IsVksPvoUnitNumber(number))
                        return true;
                }

                if (IsVksPvoUnitNumber(token))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Собирает токены для сверки Place / имени в/ч: основное имя, альтернативные,
        /// дополнительные тексты (например примечание команды в наряде) и номера частей.
        /// </summary>
        public static IReadOnlyList<string> BuildUnitTokens(
            string unitName,
            IEnumerable<string> alternativeNames,
            params string[] extraTexts)
        {
            var tokens = new List<string>();
            AddTokenWithNumbers(tokens, unitName);
            if (alternativeNames != null)
            {
                foreach (var alt in alternativeNames)
                    AddTokenWithNumbers(tokens, alt);
            }
            if (extraTexts != null)
            {
                foreach (var extra in extraTexts)
                    AddTokenWithNumbers(tokens, extra);
            }

            // Алиасы в/ч из справочника комплектования (Таманская → 23626 и т.п.)
            foreach (var related in ControlUnitAliasCatalog.ResolveRelatedTexts(tokens))
                AddTokenWithNumbers(tokens, related);

            // 54046 ↔ получатель 91711
            foreach (var recipient in ControlFarAwayUnitRules.ExpandRecipientNumbers(tokens.ToArray()))
                AddTokenWithNumbers(tokens, recipient);

            return tokens;
        }

        /// <summary>
        /// Номера в/ч (4–6 цифр) из произвольного текста контроля (Place, примечание, имя).
        /// </summary>
        public static IReadOnlyList<string> ExtractUnitNumbers(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return Array.Empty<string>();

            var normalized = NormalizeForNumbers(text);
            var result = new List<string>();
            foreach (Match match in UnitNumberRegex.Matches(normalized))
            {
                if (!result.Contains(match.Value))
                    result.Add(match.Value);
            }
            return result;
        }

        /// <summary>
        /// Place/примечание похожи на назначение в часть (а не округ/категория списка).
        /// </summary>
        public static bool LooksLikeUnitDestination(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return false;

            if (UnitMarkerRegex.IsMatch(text))
                return true;

            if (ExtractUnitNumbers(text).Count > 0)
                return true;

            if (ControlFarAwayUnitRules.LooksLikeFarAway(text))
                return true;

            if (ControlMoscowOblastUnitRules.LooksLikeMoscowOblast(text))
                return true;

            if (ControlMoscowUnitRules.LooksLikeMoscow(text))
                return true;

            return ControlUnitAliasCatalog.LooksLikeCatalogDestination(text);
        }

        /// <summary>
        /// В тексте указан род/вид войск (РВСН, ВКС, СВ, Росгвардия…).
        /// </summary>
        public static bool LooksLikeMilitaryBranch(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return false;
            return MilitaryBranchRegex.IsMatch(text);
        }

        /// <summary>
        /// Контроли «N форма допуска» — про допуск, не про направление в часть.
        /// </summary>
        public static bool IsAdmissionFormControl(string displayName)
            => !string.IsNullOrWhiteSpace(displayName)
               && displayName.Contains("форма допуска", StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// Текст (Place или имя в/ч контроля) считается совпадающим с токеном команды.
        /// Для токенов с цифрами — Contains / совпадение номеров; иначе — точное равенство.
        /// </summary>
        public static bool TextMatchesUnitToken(string text, string token)
        {
            if (string.IsNullOrWhiteSpace(text) || string.IsNullOrWhiteSpace(token))
                return false;

            var haystack = text.Trim();
            var needle = token.Trim();

            if (HasDigits(needle) || HasDigits(haystack))
            {
                if (haystack.Contains(needle, StringComparison.OrdinalIgnoreCase)
                    || needle.Contains(haystack, StringComparison.OrdinalIgnoreCase))
                    return true;

                var textNumbers = ExtractUnitNumbers(haystack);
                var tokenNumbers = ExtractUnitNumbers(needle);
                if (textNumbers.Count > 0 && tokenNumbers.Count > 0
                    && textNumbers.Any(n => tokenNumbers.Contains(n)))
                    return true;

                var normHay = NormalizeUnitText(haystack);
                var normNeedle = NormalizeUnitText(needle);
                if (normHay.Length > 0 && normNeedle.Length > 0
                    && (normHay.Contains(normNeedle, StringComparison.OrdinalIgnoreCase)
                        || normNeedle.Contains(normHay, StringComparison.OrdinalIgnoreCase)))
                    return true;

                return false;
            }

            return string.Equals(haystack, needle, StringComparison.OrdinalIgnoreCase);
        }

        public static bool TextMatchesAnyUnitToken(string text, IEnumerable<string> tokens)
        {
            if (string.IsNullOrWhiteSpace(text) || tokens == null)
                return false;

            foreach (var token in tokens)
            {
                if (TextMatchesUnitToken(text, token))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Годность по контролю: достаточно совпадения с командой по любому
        /// направленному контролю (в/ч, род войск) или контролю «водитель С»
        /// при наличии дырок под водителей. Если направленных нет — не мешает.
        /// </summary>
        public static TeamControlFitnessVerdict EvaluateControlFitness(
            IEnumerable<ControlDestinationRef> controls,
            short? teamMilitaryUnitId,
            byte? teamMilitaryTypeId,
            IReadOnlyList<string> teamUnitTokens,
            string teamMilitaryTypeName = null,
            int driverVacant = 0)
            => EvaluateControlFitnessDetailed(
                controls,
                teamMilitaryUnitId,
                teamMilitaryTypeId,
                teamUnitTokens,
                teamMilitaryTypeName,
                driverVacant).Verdict;

        /// <summary>
        /// То же, что <see cref="EvaluateControlFitness"/>, плюс причины приоритета
        /// (контроль назначения / водитель) и балл для сортировки подходящих команд.
        /// </summary>
        public static TeamControlFitnessEvaluation EvaluateControlFitnessDetailed(
            IEnumerable<ControlDestinationRef> controls,
            short? teamMilitaryUnitId,
            byte? teamMilitaryTypeId,
            IReadOnlyList<string> teamUnitTokens,
            string teamMilitaryTypeName = null,
            int driverVacant = 0)
        {
            var list = (controls ?? Enumerable.Empty<ControlDestinationRef>())
                .Where(c => c != null && !IsAdmissionFormControl(c.DisplayName))
                .ToList();

            var driverControls = list
                .Where(ControlDriverSpecialtyRules.IsDriverCControl)
                .ToList();
            var hasDriverC = driverControls.Count > 0;
            var driverMatch = ControlDriverSpecialtyRules.MatchesDriverVacancy(
                hasDriverC, driverVacant);

            var directed = list
                .Where(c => !ControlDriverSpecialtyRules.IsDriverCControl(c)
                            && IsDirectedControl(c))
                .ToList();

            var matchedControls = directed
                .Where(c => MatchesTeam(
                    c.MilitaryUnitId,
                    c.MilitaryUnitName,
                    c.Place,
                    c.MilitaryTypeId,
                    teamMilitaryUnitId,
                    teamMilitaryTypeId,
                    teamUnitTokens,
                    c.MilitaryTypeName,
                    teamMilitaryTypeName))
                .ToList();

            var destinationMatch = matchedControls.Count > 0;
            var highlights = new List<string>();
            var score = 0;

            if (destinationMatch)
            {
                var bestUnit = false;
                foreach (var c in matchedControls)
                {
                    var unitHit = MatchesMilitaryUnit(
                        c.MilitaryUnitId,
                        c.MilitaryUnitName,
                        c.Place,
                        teamMilitaryUnitId,
                        teamUnitTokens,
                        c.DisplayName);
                    if (unitHit)
                        bestUnit = true;
                    var label = FormatControlLabel(c);
                    if (string.IsNullOrWhiteSpace(label))
                        continue;
                    var line = unitHit
                        ? $"Контроль назначения: {label}"
                        : $"Контроль по роду войск: {label}";
                    if (!highlights.Exists(h => string.Equals(h, line, StringComparison.OrdinalIgnoreCase)))
                        highlights.Add(line);
                }

                score += bestUnit
                    ? TeamSuitabilityRankRules.ControlUnitWeight
                    : TeamSuitabilityRankRules.ControlBranchWeight;
            }

            if (driverMatch)
            {
                highlights.Add("Водитель С — есть дырки в наряде");
                score += TeamSuitabilityRankRules.DriverWeight;
            }

            if (destinationMatch || driverMatch)
            {
                return new TeamControlFitnessEvaluation
                {
                    Verdict = TeamControlFitnessVerdict.Fit(),
                    MatchScore = score,
                    MatchHighlights = highlights,
                };
            }

            if (directed.Count == 0 && !hasDriverC)
            {
                return new TeamControlFitnessEvaluation
                {
                    Verdict = TeamControlFitnessVerdict.Fit(),
                };
            }

            // Только «водитель С» и нет дырок — причина про дырки, не про контроль.
            if (hasDriverC && directed.Count == 0)
            {
                return new TeamControlFitnessEvaluation
                {
                    Verdict = TeamControlFitnessVerdict.Unfit(
                        ControlDriverSpecialtyRules.NoDriverHolesReason),
                };
            }

            var mismatchLabels = directed
                .Select(FormatControlLabel)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (hasDriverC)
            {
                // Есть и несовпавшие назначения, и водитель без дырок —
                // дырки указываем отдельно, без подписи «контроль».
                var lines = new List<string> { ControlDriverSpecialtyRules.NoDriverHolesReason };
                lines.AddRange(mismatchLabels.Select(label => $"Контроль в другую часть: {label}"));
                return new TeamControlFitnessEvaluation
                {
                    Verdict = new TeamControlFitnessVerdict
                    {
                        IsFit = false,
                        Reasons = lines,
                        Reason = string.Join("; ", lines),
                    },
                };
            }

            return new TeamControlFitnessEvaluation
            {
                Verdict = TeamControlFitnessVerdict.FromMismatches(mismatchLabels),
            };
        }

        public static bool IsDirectedControl(ControlDestinationRef control)
        {
            if (control == null)
                return false;
            if (HasDestination(control.MilitaryUnitId, control.Place, control.MilitaryUnitName))
                return true;
            if (control.MilitaryTypeId != null)
                return true;
            if (LooksLikeMilitaryBranch(control.Place)
                || LooksLikeMilitaryBranch(control.MilitaryTypeName))
                return true;
            return false;
        }

        /// <summary>
        /// Контроль подходит команде по в/ч: прямая ссылка на MilitaryUnitId
        /// или совпадение имени / Place с токенами команды (в т.ч. из примечания наряда).
        /// </summary>
        public static bool MatchesMilitaryUnit(
            short? controlMilitaryUnitId,
            string controlMilitaryUnitName,
            string controlPlace,
            short? teamMilitaryUnitId,
            IReadOnlyList<string> teamUnitTokens,
            string controlDisplayName = null)
        {
            if (teamMilitaryUnitId != null
                && controlMilitaryUnitId != null
                && controlMilitaryUnitId.Value == teamMilitaryUnitId.Value)
                return true;

            if (teamUnitTokens == null || teamUnitTokens.Count == 0)
                return false;

            if (TextMatchesAnyUnitToken(controlMilitaryUnitName, teamUnitTokens))
                return true;

            if (TextMatchesAnyUnitToken(controlPlace, teamUnitTokens))
                return true;

            if (ControlFarAwayUnitRules.MatchesFarAwayControl(
                    controlPlace, controlMilitaryUnitName, controlDisplayName, teamUnitTokens))
                return true;

            if (ControlMoscowOblastUnitRules.MatchesMoscowOblastControl(
                    controlPlace, controlMilitaryUnitName, controlDisplayName, teamUnitTokens))
                return true;

            if (ControlMoscowUnitRules.MatchesMoscowControl(
                    controlPlace, controlMilitaryUnitName, controlDisplayName, teamUnitTokens))
                return true;

            return false;
        }

        /// <summary>
        /// Род войск: если у контроля тип не задан — не мешает; если задан — должен совпасть.
        /// </summary>
        public static bool MatchesMilitaryType(byte? controlMilitaryTypeId, byte? teamMilitaryTypeId)
        {
            if (controlMilitaryTypeId == null || teamMilitaryTypeId == null)
                return true;

            return controlMilitaryTypeId.Value == teamMilitaryTypeId.Value;
        }

        /// <summary>
        /// Контроль с указанным родом войск подходит любой части этого рода.
        /// Исключение: «ПВО» среди ВКС — только части из списка VksPvoUnitNumbers;
        /// остальные части ВКС к ПВО не относятся.
        /// </summary>
        public static bool MatchesMilitaryBranch(
            byte? controlMilitaryTypeId,
            string controlPlace,
            string controlMilitaryTypeName,
            byte? teamMilitaryTypeId,
            string teamMilitaryTypeName,
            IReadOnlyList<string> teamUnitTokens = null)
        {
            var controlText = string.Join(" ",
                new[] { controlMilitaryTypeName, controlPlace }
                    .Where(x => !string.IsNullOrWhiteSpace(x)));
            var controlKeys = ExtractBranchKeys(controlText);
            var teamKeys = ExtractBranchKeys(teamMilitaryTypeName ?? "");
            var teamIsVksPvoUnit = TeamHasVksPvoUnit(teamUnitTokens);

            if (controlMilitaryTypeId != null
                && teamMilitaryTypeId != null
                && controlMilitaryTypeId.Value == teamMilitaryTypeId.Value)
            {
                // Один и тот же тип в БД (часто «ВКС»), но контроль текстом на ПВО —
                // тогда нужны только части ВКС из списка ПВО.
                if (controlKeys.Contains("ПВО")
                    && teamKeys.Contains("ВКС")
                    && !teamKeys.Contains("ПВО")
                    && !teamIsVksPvoUnit)
                    return false;

                return true;
            }

            if (controlKeys.Contains("ПВО"))
            {
                if (teamKeys.Contains("ПВО"))
                    return true;
                if (teamKeys.Contains("ВКС") && teamIsVksPvoUnit)
                    return true;
                return false;
            }

            if (controlKeys.Contains("ВКС"))
            {
                if (teamKeys.Contains("ВКС") || teamKeys.Contains("ПВО") || teamIsVksPvoUnit)
                    return true;
            }

            if (string.IsNullOrWhiteSpace(teamMilitaryTypeName))
                return false;

            if (string.IsNullOrWhiteSpace(controlText))
                return false;

            if (!LooksLikeMilitaryBranch(controlText)
                && string.IsNullOrWhiteSpace(controlMilitaryTypeName))
                return false;

            return BranchNamesCompatible(controlText, teamMilitaryTypeName);
        }

        /// <summary>
        /// Соответствие контроля команде: совпала в/ч (и тип не противоречит)
        /// либо указан род войск и он совпадает с родом команды (любая часть этого рода).
        /// </summary>
        public static bool MatchesTeam(
            short? controlMilitaryUnitId,
            string controlMilitaryUnitName,
            string controlPlace,
            byte? controlMilitaryTypeId,
            short? teamMilitaryUnitId,
            byte? teamMilitaryTypeId,
            IReadOnlyList<string> teamUnitTokens,
            string controlMilitaryTypeName = null,
            string teamMilitaryTypeName = null)
        {
            if (MatchesMilitaryBranch(
                    controlMilitaryTypeId,
                    controlPlace,
                    controlMilitaryTypeName,
                    teamMilitaryTypeId,
                    teamMilitaryTypeName,
                    teamUnitTokens))
                return true;

            if (!MatchesMilitaryUnit(
                    controlMilitaryUnitId,
                    controlMilitaryUnitName,
                    controlPlace,
                    teamMilitaryUnitId,
                    teamUnitTokens))
                return false;

            return MatchesMilitaryType(controlMilitaryTypeId, teamMilitaryTypeId);
        }

        /// <summary>
        /// Контроль «направлен» в конкретную часть (в/ч, имя в/ч или Place/Note с номером части).
        /// Округ (САО) или категория списка без номера части — не destination.
        /// </summary>
        public static bool HasDestination(short? militaryUnitId, string place)
            => HasDestination(militaryUnitId, place, null);

        public static bool HasDestination(short? militaryUnitId, string place, string militaryUnitName)
            => militaryUnitId != null
               || !string.IsNullOrWhiteSpace(militaryUnitName)
               || LooksLikeUnitDestination(place);

        /// <summary>
        /// Направленный контроль не совпадает с командой (часть/место/род войск).
        /// Без назначения — не считается расхождением.
        /// </summary>
        public static bool IsDestinationMismatch(
            short? controlMilitaryUnitId,
            string controlMilitaryUnitName,
            string controlPlace,
            byte? controlMilitaryTypeId,
            short? teamMilitaryUnitId,
            byte? teamMilitaryTypeId,
            IReadOnlyList<string> teamUnitTokens,
            string controlMilitaryTypeName = null,
            string teamMilitaryTypeName = null)
        {
            var directed = HasDestination(controlMilitaryUnitId, controlPlace, controlMilitaryUnitName)
                || controlMilitaryTypeId != null
                || LooksLikeMilitaryBranch(controlPlace)
                || LooksLikeMilitaryBranch(controlMilitaryTypeName);
            if (!directed)
                return false;

            return !MatchesTeam(
                controlMilitaryUnitId,
                controlMilitaryUnitName,
                controlPlace,
                controlMilitaryTypeId,
                teamMilitaryUnitId,
                teamMilitaryTypeId,
                teamUnitTokens,
                controlMilitaryTypeName,
                teamMilitaryTypeName);
        }

        private static string FormatControlLabel(ControlDestinationRef c)
        {
            if (!string.IsNullOrWhiteSpace(c.MilitaryUnitName))
                return c.MilitaryUnitName;
            if (!string.IsNullOrWhiteSpace(c.MilitaryTypeName))
                return c.MilitaryTypeName;
            if (!string.IsNullOrWhiteSpace(c.Place))
                return c.Place.Trim();
            return c.DisplayName;
        }

        /// <summary>Сопоставление обозначений рода войск (РВСН↔РВСН, СВ↔сухопутные…).</summary>
        public static bool BranchNamesCompatible(string controlText, string teamTypeName)
        {
            if (string.IsNullOrWhiteSpace(controlText) || string.IsNullOrWhiteSpace(teamTypeName))
                return false;

            var controlNorm = NormalizeBranchText(controlText);
            var teamNorm = NormalizeBranchText(teamTypeName);
            if (controlNorm.Length == 0 || teamNorm.Length == 0)
                return false;

            if (controlNorm.Contains(teamNorm, StringComparison.OrdinalIgnoreCase)
                || teamNorm.Contains(controlNorm, StringComparison.OrdinalIgnoreCase))
                return true;

            var controlKeys = ExtractBranchKeys(controlText);
            var teamKeys = ExtractBranchKeys(teamTypeName);
            if (controlKeys.Count > 0 && teamKeys.Count > 0
                && controlKeys.Overlaps(teamKeys))
                return true;

            if (controlKeys.Contains("СВ") && teamNorm.Contains("СУХОПУТ", StringComparison.OrdinalIgnoreCase))
                return true;
            if (teamKeys.Contains("СВ") && controlNorm.Contains("СУХОПУТ", StringComparison.OrdinalIgnoreCase))
                return true;

            if (controlKeys.Contains("РГ") && teamNorm.Contains("РОСГВАРД", StringComparison.OrdinalIgnoreCase))
                return true;
            if (teamKeys.Contains("РГ") && controlNorm.Contains("РОСГВАРД", StringComparison.OrdinalIgnoreCase))
                return true;
            if (controlKeys.Contains("РГ") && teamNorm.Contains("НАЦ", StringComparison.OrdinalIgnoreCase)
                && teamNorm.Contains("ГВАРД", StringComparison.OrdinalIgnoreCase))
                return true;

            return false;
        }

        private static HashSet<string> ExtractBranchKeys(string text)
        {
            var keys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (string.IsNullOrWhiteSpace(text))
                return keys;

            foreach (Match match in MilitaryBranchRegex.Matches(text))
            {
                var raw = match.Value.ToUpperInvariant();
                if (raw.StartsWith("РОСГВАРД", StringComparison.OrdinalIgnoreCase)
                    || raw == "РГ"
                    || raw == "НГ")
                    keys.Add("РГ");
                else if (raw.StartsWith("СУХОПУТ", StringComparison.OrdinalIgnoreCase))
                    keys.Add("СВ");
                else if (raw.StartsWith("ГУ", StringComparison.OrdinalIgnoreCase))
                    keys.Add("ГУ ГШ");
                else
                    keys.Add(raw);
            }

            return keys;
        }

        private static string NormalizeBranchText(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;
            return Regex.Replace(text.Trim().ToUpperInvariant(), @"\s+", " ");
        }

        private static string NormalizeUnitNumberKey(string unitNumber)
        {
            if (string.IsNullOrWhiteSpace(unitNumber))
                return string.Empty;

            var digits = new string(unitNumber.Where(char.IsDigit).ToArray());
            var trimmed = digits.TrimStart('0');
            return trimmed.Length == 0 ? string.Empty : trimmed;
        }

        private static void AddTokenWithNumbers(List<string> tokens, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return;

            var trimmed = value.Trim();
            AddToken(tokens, trimmed);
            foreach (var number in ExtractUnitNumbers(trimmed))
                AddToken(tokens, number);
        }

        private static void AddToken(List<string> tokens, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return;

            var trimmed = value.Trim();
            if (tokens.Exists(t => string.Equals(t, trimmed, StringComparison.OrdinalIgnoreCase)))
                return;

            tokens.Add(trimmed);
        }

        private static bool HasDigits(string value)
            => value.Any(char.IsDigit);

        /// <summary>Схлопывает пробелы внутри чисел: «41 516» → «41516».</summary>
        private static string NormalizeForNumbers(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            return Regex.Replace(text, @"(?<=\d)\s+(?=\d)", "");
        }

        /// <summary>Убирает маркеры в/ч и пробелы для сравнения.</summary>
        private static string NormalizeUnitText(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            var s = NormalizeForNumbers(text);
            s = UnitMarkerRegex.Replace(s, " ");
            s = Regex.Replace(s, @"\s+", "");
            return s.Trim();
        }
    }
}
