namespace IPriziv.Server.Engine.Controls
{
    /// <summary>
    /// Баллы приоритета подходящих команд в модалке комплектования/счётчиков.
    /// Чем выше сумма — тем выше команда в списке.
    /// </summary>
    public static class TeamSuitabilityRankRules
    {
        /// <summary>Совпадение контроля с конкретной в/ч команды.</summary>
        public const int ControlUnitWeight = 1000;

        /// <summary>Совпадение контроля только по роду войск.</summary>
        public const int ControlBranchWeight = 400;

        /// <summary>Контроль «водитель С» и есть дырки под водителей.</summary>
        public const int DriverWeight = 700;

        /// <summary>Форма допуска призывника совпала с режимным нарядом (к-95/165/225).</summary>
        public const int AdmissionWeight = 500;

        public static int CompareScores(int leftScore, int rightScore)
            => rightScore.CompareTo(leftScore);

        public static (int Score, IReadOnlyList<string> Highlights) Merge(
            int controlScore,
            IEnumerable<string> controlHighlights,
            int admissionScore,
            IEnumerable<string> admissionHighlights)
        {
            var highlights = new List<string>();
            void AddRange(IEnumerable<string> items)
            {
                if (items == null)
                    return;
                foreach (var item in items)
                {
                    if (string.IsNullOrWhiteSpace(item))
                        continue;
                    if (highlights.Exists(h =>
                            string.Equals(h, item, StringComparison.OrdinalIgnoreCase)))
                        continue;
                    highlights.Add(item.Trim());
                }
            }

            AddRange(controlHighlights);
            AddRange(admissionHighlights);
            return (controlScore + admissionScore, highlights);
        }
    }

    /// <summary>Результат проверки контроля с приоритетом для UI.</summary>
    public sealed class TeamControlFitnessEvaluation
    {
        public TeamControlFitnessVerdict Verdict { get; init; } = TeamControlFitnessVerdict.Fit();
        public int MatchScore { get; init; }
        public IReadOnlyList<string> MatchHighlights { get; init; } = Array.Empty<string>();
    }

    /// <summary>Результат проверки допуска с приоритетом для UI.</summary>
    public sealed class TeamAdmissionFitnessEvaluation
    {
        public TeamAdmissionVerdict Verdict { get; init; } = TeamAdmissionVerdict.Fit();
        public int MatchScore { get; init; }
        public IReadOnlyList<string> MatchHighlights { get; init; } = Array.Empty<string>();
    }
}
