namespace IPriziv.Server.Engine.Health
{
    /// <summary>Чистые правила годности по статьям / категории для команд.</summary>
    public static class TeamHealthFitnessRules
    {
        public static readonly IReadOnlyDictionary<string, HashSet<string>> UnitWhitelistRules =
            new Dictionary<string, HashSet<string>>(StringComparer.Ordinal)
            {
                ["30616"] = DiseaseArticleParser.NormalizeSet(
                    "2г,10в,12в,13д,29в,30в,33б,34г,35г,37в,38в,39в,42г,45г,46в,47б,49в,51в,54б,54в,55б,55в,56в,57г,59в,60г,62д,65г,66г,66д,67г,68г,69г,72г,73г,80в,81г,82г"),
                ["19889"] = DiseaseArticleParser.NormalizeSet(
                    "2г,12в,13д,29в,30в,34г,37в,38в,39в,42г,45г,46в,47б,54в,54б,55б,56в,57г,60г,82г,65г,66г,66д,67г,68г,69г,72г,73г,74в,75в"),
            };

        public static readonly HashSet<string> Unit41516Blacklist = DiseaseArticleParser.NormalizeSet(
            "35г,18в,23в,38б,62б,63б,80б,80в,82б,83б,83в,89б,87а,87б,15");

        public static readonly HashSet<string> NgWhitelist = DiseaseArticleParser.NormalizeSet(
            "2г,12в,30в,34г,37в,38в,39в,42г,45г,46в,47б,49в,51в,54в,54б,55б,56в,57г,62д,65г,66г,66д,67г,68г,69г,72г,73г,74в,75в");

        public static readonly HashSet<string> NoteB3Group4Whitelist = DiseaseArticleParser.NormalizeSet(
            "2г,12в,54в,54б,55б,56в,57г,62д,65г,66г,66д,67г,68г,69г");

        public static bool TryMatchUnitNumber(string militaryUnitName, out string unitNumber)
        {
            unitNumber = null;
            if (string.IsNullOrWhiteSpace(militaryUnitName))
                return false;

            foreach (var key in new[] { "30616", "19889", "41516" })
            {
                if (militaryUnitName.Contains(key, StringComparison.Ordinal))
                {
                    unitNumber = key;
                    return true;
                }
            }

            return false;
        }

        public static bool IsNoteB3Group4(string note)
        {
            if (string.IsNullOrWhiteSpace(note))
                return false;

            var n = note.Replace(" ", "", StringComparison.Ordinal).ToLowerInvariant();
            return n.Contains("б-3", StringComparison.Ordinal)
                   && (n.Contains("гр.4", StringComparison.Ordinal)
                       || n.Contains("гр4", StringComparison.Ordinal)
                       || n.Contains("группа4", StringComparison.Ordinal));
        }

        public static bool IsNationalGuard(string militaryTypeName)
        {
            if (string.IsNullOrWhiteSpace(militaryTypeName))
                return false;
            if (militaryTypeName.Contains("НГ", StringComparison.OrdinalIgnoreCase))
                return true;
            return militaryTypeName.Contains("Росгвард", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Оценивает годность. Возвращает (isFit, reasonCode, details).
        /// reasonCode: ok | missing_articles | disallowed_articles | blacklisted | missing_category | category_mismatch
        /// </summary>
        public static TeamHealthRuleVerdict Evaluate(
            IReadOnlyList<string> recruitArticles,
            string recruitHealthCategory,
            string militaryUnitName,
            string militaryTypeName,
            string externalTeamNote,
            IReadOnlyList<string> teamHealthCategories)
        {
            if (TryMatchUnitNumber(militaryUnitName, out var unitNumber))
            {
                if (unitNumber == "41516")
                {
                    var bad = recruitArticles.Where(a => Unit41516Blacklist.Contains(a)).ToList();
                    if (bad.Count > 0)
                        return TeamHealthRuleVerdict.Unfit("blacklisted", bad);
                    return TeamHealthRuleVerdict.Fit();
                }

                if (UnitWhitelistRules.TryGetValue(unitNumber, out var whitelist))
                    return EvaluateWhitelist(recruitArticles, whitelist);
            }

            if (IsNoteB3Group4(externalTeamNote))
                return EvaluateWhitelist(recruitArticles, NoteB3Group4Whitelist);

            if (IsNationalGuard(militaryTypeName))
            {
                // Без статей заболеваний — в Росгвардию (НГ) подходит.
                if (recruitArticles == null || recruitArticles.Count == 0)
                    return TeamHealthRuleVerdict.Fit();
                return EvaluateWhitelist(recruitArticles, NgWhitelist);
            }

            return EvaluateHealthCategory(recruitHealthCategory, teamHealthCategories);
        }

        public static TeamHealthRuleVerdict EvaluateWhitelist(
            IReadOnlyList<string> recruitArticles,
            HashSet<string> allowed)
        {
            if (recruitArticles == null || recruitArticles.Count == 0)
                return TeamHealthRuleVerdict.Unfit("missing_articles");

            var disallowed = recruitArticles.Where(a => !allowed.Contains(a)).ToList();
            if (disallowed.Count == 0)
                return TeamHealthRuleVerdict.Fit();

            return TeamHealthRuleVerdict.Unfit("disallowed_articles", disallowed);
        }

        /// <summary>
        /// Градация категорий годности слева направо: А-1 (лучшая) … Б-4 (худшая).
        /// Индекс меньше = категория лучше.
        /// </summary>
        public static readonly string[] HealthCategoryScale =
        {
            "а1", "а2", "а3", "а4", "б1", "б2", "б3", "б4"
        };

        public static readonly string[] HealthCategoryLabels =
        {
            "А-1", "А-2", "А-3", "А-4", "Б-1", "Б-2", "Б-3", "Б-4"
        };

        /// <summary>
        /// Разбирает категорию годности в ранг по шкале А-1…Б-4 (0 = лучшая).
        /// </summary>
        public static bool TryGetHealthCategoryRank(string category, out int rank)
        {
            rank = -1;
            if (string.IsNullOrWhiteSpace(category))
                return false;

            var key = ExtractHealthCategoryKey(category);
            if (string.IsNullOrEmpty(key))
                return false;

            for (var i = 0; i < HealthCategoryScale.Length; i++)
            {
                if (string.Equals(key, HealthCategoryScale[i], StringComparison.Ordinal))
                {
                    rank = i;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Призывник подходит под требование наряда: его категория не хуже требуемой
        /// (равна или лучше, т.е. левее по шкале А-1…Б-4).
        /// </summary>
        public static bool MeetsHealthCategoryRequirement(string recruitCategory, string teamCategory)
        {
            if (!TryGetHealthCategoryRank(recruitCategory, out var recruitRank))
                return false;
            if (!TryGetHealthCategoryRank(teamCategory, out var teamRank))
                return false;
            return recruitRank <= teamRank;
        }

        /// <summary>
        /// Достаёт ключ категории (а1…б4) из строки вида «А-1», «А1», «кат. А-2» и т.п.
        /// </summary>
        public static string ExtractHealthCategoryKey(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return "";

            var normalized = NormalizeHealthCategoryKey(value);
            foreach (var key in HealthCategoryScale)
            {
                if (normalized == key)
                    return key;
            }

            // «А-1», «А 1», «кат.А-2», en-dash и т.п.
            var match = System.Text.RegularExpressions.Regex.Match(
                value,
                @"([АаAaBbБб])\s*[-–—.]?\s*([1-4])",
                System.Text.RegularExpressions.RegexOptions.CultureInvariant);
            if (!match.Success)
                return "";

            var letterKey = match.Groups[1].Value[0] switch
            {
                'A' or 'a' or 'А' or 'а' => 'а',
                'B' or 'b' or 'Б' or 'б' => 'б',
                _ => '\0'
            };
            if (letterKey == '\0')
                return "";

            return letterKey.ToString() + match.Groups[2].Value;
        }

        private static string NormalizeHealthCategoryKey(string value)
        {
            var normalized = DiseaseArticleParser.NormalizeHealthCategory(value);
            if (string.IsNullOrEmpty(normalized))
                return "";

            return normalized
                .Replace('a', 'а')
                .Replace('b', 'б');
        }

        /// <summary>
        /// Сверка категории годности: призывник подходит, если его категория
        /// не хуже хотя бы одного требования наряда команды (А-1 лучше Б-4).
        /// </summary>
        public static TeamHealthRuleVerdict EvaluateHealthCategory(
            string recruitCategory,
            IReadOnlyList<string> teamCategories)
        {
            if (teamCategories == null || teamCategories.Count == 0)
                return TeamHealthRuleVerdict.Fit();

            if (string.IsNullOrWhiteSpace(recruitCategory))
                return TeamHealthRuleVerdict.Unfit("missing_category");

            if (!TryGetHealthCategoryRank(recruitCategory, out _))
                return TeamHealthRuleVerdict.Unfit("category_mismatch", details: new[] { recruitCategory });

            var knownTeamCategories = teamCategories
                .Where(t => !string.IsNullOrWhiteSpace(t) && TryGetHealthCategoryRank(t, out _))
                .ToList();

            // Если у команды нет распознанных категорий — не блокируем.
            if (knownTeamCategories.Count == 0)
                return TeamHealthRuleVerdict.Fit();

            if (knownTeamCategories.Any(t => MeetsHealthCategoryRequirement(recruitCategory, t)))
                return TeamHealthRuleVerdict.Fit();

            return TeamHealthRuleVerdict.Unfit("category_mismatch", details: new[] { recruitCategory });
        }

        /// <summary>
        /// Фильтрует команды, в которые призывник подходит по статьям / категории годности.
        /// </summary>
        public static IReadOnlyList<T> FilterSuitableTeams<T>(
            IReadOnlyList<string> recruitArticles,
            string recruitHealthCategory,
            IEnumerable<T> teams,
            Func<T, string> militaryUnitName,
            Func<T, string> militaryTypeName,
            Func<T, string> externalTeamNote,
            Func<T, IReadOnlyList<string>> teamHealthCategories)
        {
            if (teams == null)
                return Array.Empty<T>();

            var result = new List<T>();
            foreach (var team in teams)
            {
                var verdict = Evaluate(
                    recruitArticles,
                    recruitHealthCategory,
                    militaryUnitName(team),
                    militaryTypeName(team),
                    externalTeamNote(team),
                    teamHealthCategories(team));
                if (verdict.IsFit)
                    result.Add(team);
            }
            return result;
        }

        /// <summary>Краткая причина для списка команд (без HTML).</summary>
        public static string FormatBriefReason(
            TeamHealthRuleVerdict verdict,
            IReadOnlyList<string> teamHealthCategories = null)
        {
            if (verdict == null || verdict.IsFit)
                return null;

            var details = verdict.Details != null && verdict.Details.Count > 0
                ? string.Join(", ", verdict.Details)
                : null;
            var teamCats = teamHealthCategories != null && teamHealthCategories.Count > 0
                ? string.Join(", ", teamHealthCategories.Where(c => !string.IsNullOrWhiteSpace(c)))
                : null;

            return verdict.ReasonCode switch
            {
                "blacklisted" => string.IsNullOrEmpty(details)
                    ? "Запрещённые статьи для в/ч 41516"
                    : $"Запрещённые статьи: {details}",
                "missing_articles" => "Нет статей заболеваний",
                "disallowed_articles" => string.IsNullOrEmpty(details)
                    ? "Статьи вне разрешённого списка"
                    : $"Статьи вне списка: {details}",
                "missing_category" => "Нет категории годности",
                "category_mismatch" => string.IsNullOrEmpty(teamCats)
                    ? (string.IsNullOrEmpty(details)
                        ? "Категория не подходит к наряду"
                        : $"Категория {details} не подходит к наряду")
                    : (string.IsNullOrEmpty(details)
                        ? $"Не подходит к наряду ({teamCats})"
                        : $"Категория {details} ≠ наряд ({teamCats})"),
                _ => "Не подходит по здоровью"
            };
        }
    }

    public sealed class TeamHealthRuleVerdict
    {
        public bool IsFit { get; init; }
        public string ReasonCode { get; init; }
        public IReadOnlyList<string> Details { get; init; } = Array.Empty<string>();

        public static TeamHealthRuleVerdict Fit()
            => new() { IsFit = true, ReasonCode = "ok" };

        public static TeamHealthRuleVerdict Unfit(string reasonCode, IReadOnlyList<string> details = null)
            => new()
            {
                IsFit = false,
                ReasonCode = reasonCode,
                Details = details ?? Array.Empty<string>()
            };
    }
}
