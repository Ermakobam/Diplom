using System;
using System.Text.RegularExpressions;
using IPriziv.Server.Data.Enumerations;

namespace IPriziv.Server.Engine.Controls
{
    /// <summary>
    /// Соответствие формы допуска призывника режимам наряда:
    /// к-165 → 1 форма, к-225 → 2 форма, к-95 → 3 форма.
    /// Учитывается AdmissId призывника и контроли «N форма допуска (получен/получена)».
    /// Другие режимы и AdmissId слотов наряда не учитываются.
    /// </summary>
    public static class TeamAdmissionFitnessRules
    {
        private static readonly Regex RegimeTokenRegex = new(
            @"\bк[-\s]?(\d{2,4})([аaбb])?\b",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex BareRegimeRegex = new(
            @"^\s*к?[-\s]?(\d{2,4})([аaбb])?\s*$",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// «1 форма допуска (получен)», «2 форма допуска (получена)» и т.п.
        /// </summary>
        private static readonly Regex ReceivedAdmissionFormRegex = new(
            @"([123])\s*форма\s*допуска.{0,48}\(\s*получен\w*\s*\)",
            RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);

        /// <summary>к-165 → 1, к-225 → 2, к-95 / к-95а / к-95б → 3.</summary>
        private static readonly Dictionary<string, byte> RegimeToAdmissionForm =
            new(StringComparer.OrdinalIgnoreCase)
            {
                ["165"] = (byte)AdmissionType.First,
                ["225"] = (byte)AdmissionType.Second,
                ["95"] = (byte)AdmissionType.Third,
                ["95а"] = (byte)AdmissionType.Third,
                ["95a"] = (byte)AdmissionType.Third,
                ["95б"] = (byte)AdmissionType.Third,
                ["95b"] = (byte)AdmissionType.Third,
            };

        public static bool IsFormAdmission(byte? admissId)
            => admissId is (byte)AdmissionType.First
                or (byte)AdmissionType.Second
                or (byte)AdmissionType.Third;

        public static bool IsKnownAdmissionRegime(string regimeCode)
        {
            if (string.IsNullOrWhiteSpace(regimeCode))
                return false;
            var normalized = NormalizeRegimeCode(regimeCode);
            return RegimeToAdmissionForm.ContainsKey(normalized)
                   || RegimeToAdmissionForm.ContainsKey(CanonicalRegimeCode(normalized));
        }

        /// <summary>
        /// Маппинг кода режима на форму допуска. Только 95 / 95а / 95б / 165 / 225.
        /// </summary>
        public static bool TryMapRegimeToAdmissionForm(string regimeNameOrCode, out byte admissId, out string regimeCode)
        {
            admissId = 0;
            regimeCode = null;

            if (string.IsNullOrWhiteSpace(regimeNameOrCode))
                return false;

            var raw = NormalizeRegimeCode(regimeNameOrCode.Trim());
            if (RegimeToAdmissionForm.TryGetValue(raw, out admissId))
            {
                regimeCode = CanonicalRegimeCode(raw);
                return true;
            }

            var bare = BareRegimeRegex.Match(regimeNameOrCode.Trim());
            if (bare.Success)
            {
                var code = NormalizeRegimeCode(bare.Groups[1].Value + bare.Groups[2].Value);
                if (RegimeToAdmissionForm.TryGetValue(code, out admissId))
                {
                    regimeCode = CanonicalRegimeCode(code);
                    return true;
                }
            }

            foreach (Match match in RegimeTokenRegex.Matches(regimeNameOrCode))
            {
                var code = NormalizeRegimeCode(match.Groups[1].Value + match.Groups[2].Value);
                if (RegimeToAdmissionForm.TryGetValue(code, out admissId))
                {
                    regimeCode = CanonicalRegimeCode(code);
                    return true;
                }
            }

            return false;
        }

        private static string NormalizeRegimeCode(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
                return "";
            return raw.Trim()
                .Replace(" ", "", StringComparison.Ordinal)
                .Replace("к-", "", StringComparison.OrdinalIgnoreCase)
                .Replace("к", "", StringComparison.OrdinalIgnoreCase)
                .Replace('a', 'а')
                .Replace('A', 'а')
                .Replace('b', 'б')
                .Replace('B', 'б')
                .ToLowerInvariant();
        }

        private static string CanonicalRegimeCode(string normalized)
        {
            if (string.IsNullOrWhiteSpace(normalized))
                return normalized;
            if (normalized.StartsWith("95", StringComparison.OrdinalIgnoreCase)
                && normalized.Length >= 3)
            {
                var suffix = normalized[^1];
                if (suffix is 'а' or 'б')
                    return "95" + suffix;
            }
            if (normalized.StartsWith("95", StringComparison.OrdinalIgnoreCase))
                return "95";
            if (normalized.StartsWith("165", StringComparison.OrdinalIgnoreCase))
                return "165";
            if (normalized.StartsWith("225", StringComparison.OrdinalIgnoreCase))
                return "225";
            return normalized;
        }

        /// <summary>
        /// Из текста контроля достаёт форму допуска, если она помечена как полученная.
        /// </summary>
        public static bool TryParseReceivedAdmissionForm(string text, out byte admissId)
        {
            admissId = 0;
            if (string.IsNullOrWhiteSpace(text))
                return false;

            var match = ReceivedAdmissionFormRegex.Match(text);
            if (!match.Success)
                return false;

            if (!byte.TryParse(match.Groups[1].Value, out var formNumber))
                return false;

            admissId = formNumber switch
            {
                1 => (byte)AdmissionType.First,
                2 => (byte)AdmissionType.Second,
                3 => (byte)AdmissionType.Third,
                _ => (byte)0
            };
            return IsFormAdmission(admissId);
        }

        /// <summary>
        /// Собирает полученные формы допуска из текстов контролей (DisplayName / Place / Note).
        /// </summary>
        public static IReadOnlyList<byte> CollectReceivedAdmissionForms(IEnumerable<string> texts)
        {
            var result = new List<byte>();
            if (texts == null)
                return result;

            foreach (var text in texts)
            {
                if (!TryParseReceivedAdmissionForm(text, out var form))
                    continue;
                if (!result.Contains(form))
                    result.Add(form);
            }

            return result;
        }

        /// <summary>
        /// Из текста достаёт только коды режимов допуска: 95, 165, 225.
        /// </summary>
        public static IReadOnlyList<string> ExtractRegimeCodes(params string[] texts)
        {
            var result = new List<string>();
            if (texts == null)
                return result;

            foreach (var text in texts)
            {
                if (string.IsNullOrWhiteSpace(text))
                    continue;

                if (TryMapRegimeToAdmissionForm(text, out _, out var singleCode)
                    && BareRegimeRegex.IsMatch(text.Trim()))
                {
                    if (!result.Exists(x => string.Equals(x, singleCode, StringComparison.OrdinalIgnoreCase)))
                        result.Add(singleCode);
                    continue;
                }

                foreach (Match match in RegimeTokenRegex.Matches(text))
                {
                    var code = NormalizeRegimeCode(match.Groups[1].Value + match.Groups[2].Value);
                    if (!IsKnownAdmissionRegime(code) && !IsKnownAdmissionRegime(CanonicalRegimeCode(code)))
                        continue;
                    var canonical = CanonicalRegimeCode(code);
                    if (!result.Exists(x => string.Equals(x, canonical, StringComparison.OrdinalIgnoreCase)))
                        result.Add(canonical);
                }
            }

            return result;
        }

        public static string FormatRegimeLabel(IEnumerable<string> regimeNames)
        {
            var labels = (regimeNames ?? Enumerable.Empty<string>())
                .Where(n => !string.IsNullOrWhiteSpace(n))
                .Select(n =>
                {
                    var t = n.Trim();
                    return t.StartsWith("к-", StringComparison.OrdinalIgnoreCase) ? t : $"к-{t}";
                })
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
                .ToList();
            return labels.Count == 0 ? null : string.Join(", ", labels);
        }

        public static string FormatAdmissionLabel(byte admissId)
            => admissId switch
            {
                (byte)AdmissionType.First => "1 форма",
                (byte)AdmissionType.Second => "2 форма",
                (byte)AdmissionType.Third => "3 форма",
                _ => "без формы допуска"
            };

        public static IReadOnlyList<string> GetRequiredAdmissionLabels(IEnumerable<TeamAdmissionSlot>? teamSlots)
        {
            if (teamSlots == null)
                return Array.Empty<string>();

            var forms = new HashSet<byte>();
            foreach (var slot in teamSlots)
            {
                if (slot.AdmissId is (byte)AdmissionType.First or (byte)AdmissionType.Second or (byte)AdmissionType.Third)
                {
                    forms.Add(slot.AdmissId.Value);
                }
                if (!string.IsNullOrWhiteSpace(slot.RegimeName) && TryMapRegimeToAdmissionForm(slot.RegimeName, out var formFromRegime, out _))
                {
                    forms.Add(formFromRegime);
                }
            }

            return forms
                .OrderBy(f => f)
                .Select(FormatAdmissionLabel)
                .ToList();
        }

        /// <summary>
        /// Проверка допуска по режимам наряда (к-95 / к-165 / к-225).
        /// Формы берутся из AdmissId призывника и из контролей «форма допуска (получена)».
        /// </summary>
        public static TeamAdmissionVerdict Evaluate(
            byte? recruitAdmissId,
            IReadOnlyList<TeamAdmissionSlot> teamSlots,
            IEnumerable<byte> receivedFormsFromControls = null)
            => EvaluateDetailed(recruitAdmissId, teamSlots, receivedFormsFromControls).Verdict;

        /// <summary>
        /// То же, что <see cref="Evaluate"/>, плюс причина приоритета при совпадении формы с нарядом.
        /// </summary>
        public static TeamAdmissionFitnessEvaluation EvaluateDetailed(
            byte? recruitAdmissId,
            IReadOnlyList<TeamAdmissionSlot> teamSlots,
            IEnumerable<byte> receivedFormsFromControls = null)
        {
            teamSlots ??= Array.Empty<TeamAdmissionSlot>();

            var mapped = new List<(string RegimeCode, byte Form)>();
            foreach (var slot in teamSlots)
            {
                if (!TryMapRegimeToAdmissionForm(slot.RegimeName, out var form, out var code))
                    continue;
                if (!mapped.Exists(x =>
                        string.Equals(x.RegimeCode, code, StringComparison.OrdinalIgnoreCase)
                        && x.Form == form))
                    mapped.Add((code, form));
            }

            if (mapped.Count == 0)
                return new TeamAdmissionFitnessEvaluation { Verdict = TeamAdmissionVerdict.Fit() };

            var requiredForms = mapped
                .Select(x => x.Form)
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            var regimes = mapped
                .Select(x => x.RegimeCode)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
                .ToList();

            var regimeLabel = FormatRegimeLabel(regimes) ?? "режимный наряд";
            var formsLabel = string.Join("/", requiredForms.Select(FormatAdmissionLabel));

            var ownedForms = new List<byte>();
            if (IsFormAdmission(recruitAdmissId))
                ownedForms.Add(recruitAdmissId.Value);

            if (receivedFormsFromControls != null)
            {
                foreach (var form in receivedFormsFromControls)
                {
                    if (!IsFormAdmission(form))
                        continue;
                    if (!ownedForms.Contains(form))
                        ownedForms.Add(form);
                }
            }

            var matchedForms = ownedForms
                .Where(form => requiredForms.Contains(form))
                .OrderBy(x => x)
                .ToList();

            if (matchedForms.Count > 0)
            {
                var matchedRegimes = mapped
                    .Where(x => matchedForms.Contains(x.Form))
                    .Select(x => x.RegimeCode)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
                    .ToList();
                var matchedRegimeLabel = FormatRegimeLabel(matchedRegimes) ?? regimeLabel;
                var matchedFormsLabel = string.Join("/", matchedForms.Select(FormatAdmissionLabel));
                return new TeamAdmissionFitnessEvaluation
                {
                    Verdict = TeamAdmissionVerdict.Fit(),
                    MatchScore = TeamSuitabilityRankRules.AdmissionWeight,
                    MatchHighlights = new[]
                    {
                        $"Допуск: {matchedFormsLabel} ({matchedRegimeLabel})"
                    },
                };
            }

            if (ownedForms.Count == 0)
            {
                return new TeamAdmissionFitnessEvaluation
                {
                    Verdict = TeamAdmissionVerdict.Unfit(
                        "missing_admission",
                        $"Нет формы допуска для наряда ({regimeLabel}); нужна: {formsLabel}"),
                };
            }

            var ownedLabel = string.Join("/", ownedForms.OrderBy(x => x).Select(FormatAdmissionLabel));
            return new TeamAdmissionFitnessEvaluation
            {
                Verdict = TeamAdmissionVerdict.Unfit(
                    "admission_mismatch",
                    $"{ownedLabel} не подходит к наряду ({regimeLabel}); нужна: {formsLabel}"),
            };
        }

        /// <summary>
        /// Для спецназа / МП / корабельного ВМФ — нужна именно 3 форма (поверх OR по слотам наряда).
        /// </summary>
        public static TeamAdmissionFitnessEvaluation EnsureThirdAdmissionForm(
            TeamAdmissionFitnessEvaluation current,
            TeamImpliedUnitRequirementRules.UnitRequirementKind kind,
            byte? recruitAdmissId,
            IEnumerable<byte> receivedFormsFromControls)
        {
            current ??= new TeamAdmissionFitnessEvaluation { Verdict = TeamAdmissionVerdict.Fit() };
            if (!TeamImpliedUnitRequirementRules.RequiresThirdAdmissionForm(kind))
                return current;

            var ownedForms = new List<byte>();
            if (IsFormAdmission(recruitAdmissId))
                ownedForms.Add(recruitAdmissId.Value);
            if (receivedFormsFromControls != null)
            {
                foreach (var form in receivedFormsFromControls)
                {
                    if (!IsFormAdmission(form))
                        continue;
                    if (!ownedForms.Contains(form))
                        ownedForms.Add(form);
                }
            }

            if (ownedForms.Contains((byte)AdmissionType.Third))
            {
                if (!current.Verdict.IsFit)
                    return current;

                var kindLabel = TeamImpliedUnitRequirementRules.FormatKindLabel(kind);
                var regimeLabel = TeamImpliedUnitRequirementRules.RequiredRegimeLabel(kind);
                var highlight = $"Допуск: 3 форма ({kindLabel}, {regimeLabel})";
                var highlights = (current.MatchHighlights ?? Array.Empty<string>()).ToList();
                if (!highlights.Exists(h => string.Equals(h, highlight, StringComparison.OrdinalIgnoreCase)))
                    highlights.Insert(0, highlight);

                return new TeamAdmissionFitnessEvaluation
                {
                    Verdict = current.Verdict,
                    MatchScore = Math.Max(current.MatchScore, TeamSuitabilityRankRules.AdmissionWeight),
                    MatchHighlights = highlights,
                };
            }

            var label = TeamImpliedUnitRequirementRules.FormatKindLabel(kind) ?? "часть";
            var regime = TeamImpliedUnitRequirementRules.RequiredRegimeLabel(kind) ?? "к-95";
            if (ownedForms.Count == 0)
            {
                return new TeamAdmissionFitnessEvaluation
                {
                    Verdict = TeamAdmissionVerdict.Unfit(
                        "missing_admission",
                        $"Нет 3 формы допуска для {label} ({regime})"),
                };
            }

            var ownedLabel = string.Join("/", ownedForms.OrderBy(x => x).Select(FormatAdmissionLabel));
            return new TeamAdmissionFitnessEvaluation
            {
                Verdict = TeamAdmissionVerdict.Unfit(
                    "admission_mismatch",
                    $"{ownedLabel} не подходит для {label}; нужна 3 форма ({regime})"),
            };
        }
    }

    public sealed class TeamAdmissionSlot
    {
        public string RegimeName { get; init; }
        public byte? AdmissId { get; init; }
    }

    public sealed class TeamAdmissionVerdict
    {
        public bool IsFit { get; init; }
        public string ReasonCode { get; init; }
        public string Reason { get; init; }

        public static TeamAdmissionVerdict Fit()
            => new() { IsFit = true, ReasonCode = "ok" };

        public static TeamAdmissionVerdict Unfit(string reasonCode, string reason)
            => new() { IsFit = false, ReasonCode = reasonCode, Reason = reason };
    }
}
