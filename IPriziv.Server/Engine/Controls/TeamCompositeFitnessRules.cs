using System;
using System.Collections.Generic;
using System.Linq;
using IPriziv.Server.Engine.Health;

namespace IPriziv.Server.Engine.Controls
{
    /// <summary>
    /// Сводка годности: здоровье + контроль назначения + допуск по наряду.
    /// </summary>
    public static class TeamCompositeFitnessRules
    {
        public static TeamCompositeFitnessVerdict Compose(
            TeamHealthRuleVerdict health,
            TeamControlFitnessVerdict control,
            TeamAdmissionVerdict admission,
            TeamNpuFitnessVerdict npu = null,
            TeamHeightFitnessVerdict height = null,
            TeamTspFitnessVerdict tsp = null,
            int controlMatchScore = 0,
            IEnumerable<string> controlMatchHighlights = null,
            int admissionMatchScore = 0,
            IEnumerable<string> admissionMatchHighlights = null)
        {
            health ??= TeamHealthRuleVerdict.Fit();
            control ??= TeamControlFitnessVerdict.Fit();
            admission ??= TeamAdmissionVerdict.Fit();
            npu ??= TeamNpuFitnessVerdict.Fit();
            height ??= TeamHeightFitnessVerdict.Fit();
            tsp ??= TeamTspFitnessVerdict.Fit();

            var reasons = new List<string>();
            if (!health.IsFit)
            {
                var healthReason = TeamHealthFitnessRules.FormatBriefReason(health);
                if (!string.IsNullOrWhiteSpace(healthReason))
                    reasons.Add(healthReason);
            }

            if (!control.IsFit)
            {
                if (control.Reasons is { Count: > 0 })
                    reasons.AddRange(control.Reasons.Where(r => !string.IsNullOrWhiteSpace(r)));
                else if (!string.IsNullOrWhiteSpace(control.Reason))
                    reasons.Add(control.Reason);
            }

            if (!admission.IsFit && !string.IsNullOrWhiteSpace(admission.Reason))
                reasons.Add(admission.Reason);

            if (!npu.IsFit && !string.IsNullOrWhiteSpace(npu.Reason))
                reasons.Add(npu.Reason);

            if (!height.IsFit && !string.IsNullOrWhiteSpace(height.Reason))
                reasons.Add(height.Reason);

            if (!tsp.IsFit && !string.IsNullOrWhiteSpace(tsp.Reason))
                reasons.Add(tsp.Reason);

            var isFit = health.IsFit && control.IsFit && admission.IsFit && npu.IsFit && height.IsFit && tsp.IsFit;
            var (categoryFit, articlesFit) = SplitHealthFactors(health);
            var (matchScore, matchHighlights) = isFit
                ? TeamSuitabilityRankRules.Merge(
                    controlMatchScore,
                    controlMatchHighlights,
                    admissionMatchScore,
                    admissionMatchHighlights)
                : (0, (IReadOnlyList<string>)Array.Empty<string>());

            return new TeamCompositeFitnessVerdict
            {
                IsFit = isFit,
                HealthFit = health.IsFit,
                CategoryFit = categoryFit,
                ArticlesFit = articlesFit,
                ControlFit = control.IsFit,
                AdmissionFit = admission.IsFit,
                NpuFit = npu.IsFit,
                HeightFit = height.IsFit,
                TspFit = tsp.IsFit,
                Reasons = reasons,
                Reason = reasons.Count == 0 ? null : string.Join("; ", reasons),
                MatchScore = matchScore,
                MatchHighlights = matchHighlights,
            };
        }

        /// <summary>
        /// Разделяет вердикт здоровья на категорию годности и статьи заболеваний.
        /// </summary>
        public static (bool CategoryFit, bool ArticlesFit) SplitHealthFactors(TeamHealthRuleVerdict health)
        {
            if (health == null || health.IsFit)
                return (true, true);

            return health.ReasonCode switch
            {
                "missing_category" or "category_mismatch" => (false, true),
                "missing_articles" or "disallowed_articles" or "blacklisted" => (true, false),
                _ => (false, false),
            };
        }
    }

    public sealed class TeamControlFitnessVerdict
    {
        public bool IsFit { get; init; }
        public string Reason { get; init; }
        public IReadOnlyList<string> Reasons { get; init; } = Array.Empty<string>();

        public static TeamControlFitnessVerdict Fit()
            => new() { IsFit = true };

        public static TeamControlFitnessVerdict Unfit(string reason)
            => new()
            {
                IsFit = false,
                Reason = reason,
                Reasons = string.IsNullOrWhiteSpace(reason)
                    ? Array.Empty<string>()
                    : new[] { reason },
            };

        /// <summary>
        /// Направленные контроли в другую часть → не подходит.
        /// Каждый контроль — отдельная причина (отдельная строка в UI).
        /// </summary>
        public static TeamControlFitnessVerdict FromMismatches(
            IReadOnlyList<string> mismatchLabels)
        {
            if (mismatchLabels == null || mismatchLabels.Count == 0)
                return Fit();

            var distinct = mismatchLabels
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (distinct.Count == 0)
                return Unfit("Контроль в другую часть/место");

            var lines = distinct
                .Select(label => $"Контроль в другую часть: {label}")
                .ToList();

            return new TeamControlFitnessVerdict
            {
                IsFit = false,
                Reasons = lines,
                Reason = string.Join("; ", lines),
            };
        }
    }

    public sealed class TeamCompositeFitnessVerdict
    {
        public bool IsFit { get; init; }
        public bool HealthFit { get; init; }
        public bool CategoryFit { get; init; } = true;
        public bool ArticlesFit { get; init; } = true;
        public bool ControlFit { get; init; }
        public bool AdmissionFit { get; init; }
        public bool NpuFit { get; init; } = true;
        public bool HeightFit { get; init; } = true;
        public bool TspFit { get; init; } = true;
        public string Reason { get; init; }
        public IReadOnlyList<string> Reasons { get; init; } = Array.Empty<string>();
        public int MatchScore { get; init; }
        public IReadOnlyList<string> MatchHighlights { get; init; } = Array.Empty<string>();
    }
}
