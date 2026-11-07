namespace IPriziv.Server.Engine.Recruits
{
    /// <summary>
    /// Правила массового объединения призывников:
    /// одинаковые Фамилия, Имя, Отчество, полная дата рождения и военкомат
    /// (одинаковый Id либо канонически схожие названия).
    /// </summary>
    public static class RecruitMergeRules
    {
        public sealed class RecruitSnapshot
        {
            public int Id { get; init; }
            public string LastName { get; init; }
            public string FirstName { get; init; }
            public string MiddleName { get; init; }
            public DateTime? BirthDate { get; init; }
            public byte? MilitaryOfficeId { get; init; }
            public string MilitaryOfficeName { get; init; }
            public short? GatheredSubTeamId { get; init; }
            public int StateRank { get; init; }
        }

        public sealed class MergePlan
        {
            /// <summary>Целевой призывник → список id, которые в него вливаются.</summary>
            public Dictionary<int, List<int>> Merges { get; init; } = new();

            /// <summary>Призывники с тем же ФИО, но другой датой рождения или несхожим военкоматом.</summary>
            public int ConflictPersonCount { get; init; }

            public int MergedPersonCount => Merges.Values.Sum(x => x.Count);
        }

        /// <summary>
        /// Группы алиасов военкоматов (старые районные / «ВК …АО» → актуальный округ).
        /// </summary>
        private static readonly string[][] OfficeAliasGroups =
        {
            new[] { "северный", "вк сао", "сао", "тимирязевский", "коптевский", "головинский", "савеловский" },
            new[] { "северо-восточный", "вк свао", "свао", "бабушкинский", "останкинский", "бутырский" },
            new[] { "восточный", "вк вао", "вао", "преображенский", "измайловский", "перовский" },
            new[] { "юго-восточный", "вк ювао", "ювао", "кузьминский", "лефортовский", "люблинский" },
            new[] { "южный", "вк юао", "юао", "даниловский", "царицынский", "чертановский" },
            new[] { "юго-западный", "вк юзао", "юзао", "гагаринский", "академический", "черемушкинский", "бутовский" },
            new[] { "западный", "вк зао", "зао", "раменский", "кунцевский", "солнцевский" },
            new[] { "северо-западный", "вк сзао", "сзао", "хорошевский", "тушинский", "митинский" },
            new[] { "центральный", "вк цао", "цао", "тверской", "красносельский", "замоскворецкий" },
            new[] { "зеленоградский", "вк зелао", "зелао", "зеленоград" },
            new[]
            {
                "новомосковско-троицкий",
                "новомосковский и троицкий",
                "вк нитао",
                "нитао",
                "тинао"
            },
        };

        public static string BuildNameKey(string lastName, string firstName, string middleName)
            => $"{Normalize(lastName)}|{Normalize(firstName)}|{Normalize(middleName)}";

        public static string Normalize(string value)
            => (value ?? "").Trim().ToLowerInvariant().Replace("  ", " ", StringComparison.Ordinal);

        public static bool TryExactBirthDate(DateTime? birthDate, out DateTime date)
        {
            date = default;
            if (!birthDate.HasValue)
                return false;
            date = birthDate.Value.Date;
            return true;
        }

        /// <summary>
        /// Ключ военкомата: каноническое имя из группы алиасов, иначе нормализованное имя, иначе Id.
        /// </summary>
        public static string BuildOfficeKey(byte? officeId, string officeName)
        {
            var name = Normalize(officeName);
            if (!string.IsNullOrEmpty(name))
            {
                foreach (var group in OfficeAliasGroups)
                {
                    if (group.Any(alias => NameMatchesAlias(name, alias)))
                        return group[0];
                }

                return name;
            }

            return officeId?.ToString() ?? "";
        }

        private static bool NameMatchesAlias(string name, string alias)
        {
            if (name == alias)
                return true;

            // Короткие коды («зао», «вао») только точное совпадение — иначе «юзао»/«свао».
            if (alias.Length < 8)
                return false;

            return name.Contains(alias, StringComparison.Ordinal)
                || alias.Contains(name, StringComparison.Ordinal);
        }

        public static bool SameOffice(RecruitSnapshot a, RecruitSnapshot b)
        {
            if (a.MilitaryOfficeId.HasValue
                && b.MilitaryOfficeId.HasValue
                && a.MilitaryOfficeId == b.MilitaryOfficeId)
                return true;

            var keyA = BuildOfficeKey(a.MilitaryOfficeId, a.MilitaryOfficeName);
            var keyB = BuildOfficeKey(b.MilitaryOfficeId, b.MilitaryOfficeName);
            if (string.IsNullOrEmpty(keyA) || string.IsNullOrEmpty(keyB))
                return keyA == keyB;

            return keyA == keyB;
        }

        public static string BuildMergeKey(RecruitSnapshot recruit)
        {
            if (!TryExactBirthDate(recruit.BirthDate, out var birth))
                return null;

            var officeKey = BuildOfficeKey(recruit.MilitaryOfficeId, recruit.MilitaryOfficeName);
            return $"{BuildNameKey(recruit.LastName, recruit.FirstName, recruit.MiddleName)}|{birth:yyyy-MM-dd}|{officeKey}";
        }

        public static MergePlan BuildPlan(IReadOnlyList<RecruitSnapshot> recruits)
        {
            var byName = recruits
                .GroupBy(r => BuildNameKey(r.LastName, r.FirstName, r.MiddleName))
                .Where(g => !string.IsNullOrEmpty(g.Key))
                .ToList();

            var conflictPersonCount = 0;
            foreach (var nameGroup in byName)
            {
                if (nameGroup.Count() < 2)
                    continue;

                var list = nameGroup.ToList();
                foreach (var recruit in list)
                {
                    var hasMismatch = list.Any(other =>
                        other.Id != recruit.Id
                        && !SameBirthAndOffice(recruit, other));
                    if (hasMismatch)
                        conflictPersonCount++;
                }
            }

            var merges = new Dictionary<int, List<int>>();
            foreach (var group in recruits
                         .Select(r => (Recruit: r, Key: BuildMergeKey(r)))
                         .Where(x => x.Key != null)
                         .GroupBy(x => x.Key)
                         .Where(g => g.Count() > 1))
            {
                var ordered = group
                    .Select(x => x.Recruit)
                    .OrderByDescending(r => r.GatheredSubTeamId.HasValue)
                    .ThenByDescending(r => r.StateRank)
                    .ThenBy(r => r.Id)
                    .ToList();

                var target = ordered[0];
                var sources = ordered.Skip(1).Select(r => r.Id).ToList();
                if (sources.Count > 0)
                    merges[target.Id] = sources;
            }

            return new MergePlan
            {
                Merges = merges,
                ConflictPersonCount = conflictPersonCount
            };
        }

        private static bool SameBirthAndOffice(RecruitSnapshot a, RecruitSnapshot b)
        {
            if (!TryExactBirthDate(a.BirthDate, out var birthA)
                || !TryExactBirthDate(b.BirthDate, out var birthB)
                || birthA != birthB)
                return false;

            return SameOffice(a, b);
        }
    }
}
