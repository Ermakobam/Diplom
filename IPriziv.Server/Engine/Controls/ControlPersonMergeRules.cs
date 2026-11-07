namespace IPriziv.Server.Engine.Controls
{
    /// <summary>
    /// Правила массового объединения персон контролей:
    /// одинаковое ФИО (поле «Имя»), полная дата рождения и военкомат.
    /// </summary>
    public static class ControlPersonMergeRules
    {
        public sealed class PersonSnapshot
        {
            public int Id { get; init; }
            public string LastName { get; init; }
            public string FirstName { get; init; }
            public string MiddleName { get; init; }
            public DateTime? BirthDate { get; init; }
            public byte? MilitaryOfficeId { get; init; }
            public int? RecruitId { get; init; }
        }

        public sealed class MergePlan
        {
            /// <summary>Целевая персона → список id, которые в неё вливаются.</summary>
            public Dictionary<int, List<int>> Merges { get; init; } = new();

            /// <summary>Персоны, у которых есть однофамилец с другим ДР или военкоматом.</summary>
            public int ConflictPersonCount { get; init; }

            public int MergedPersonCount => Merges.Values.Sum(x => x.Count);
        }

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

        public static string BuildMergeKey(PersonSnapshot person)
        {
            if (!TryExactBirthDate(person.BirthDate, out var birth))
                return null;

            return $"{BuildNameKey(person.LastName, person.FirstName, person.MiddleName)}|{birth:yyyy-MM-dd}|{person.MilitaryOfficeId?.ToString() ?? ""}";
        }

        public static MergePlan BuildPlan(IReadOnlyList<PersonSnapshot> persons)
        {
            var byName = persons
                .GroupBy(p => BuildNameKey(p.LastName, p.FirstName, p.MiddleName))
                .Where(g => !string.IsNullOrEmpty(g.Key))
                .ToList();

            var conflictPersonCount = 0;
            foreach (var nameGroup in byName)
            {
                if (nameGroup.Count() < 2)
                    continue;

                var list = nameGroup.ToList();
                foreach (var person in list)
                {
                    var hasMismatch = list.Any(other =>
                        other.Id != person.Id
                        && !SameBirthAndOffice(person, other));
                    if (hasMismatch)
                        conflictPersonCount++;
                }
            }

            var merges = new Dictionary<int, List<int>>();
            foreach (var group in persons
                         .Select(p => (Person: p, Key: BuildMergeKey(p)))
                         .Where(x => x.Key != null)
                         .GroupBy(x => x.Key)
                         .Where(g => g.Count() > 1))
            {
                var ordered = group
                    .Select(x => x.Person)
                    .OrderByDescending(p => p.RecruitId.HasValue)
                    .ThenBy(p => p.Id)
                    .ToList();

                var target = ordered[0];
                var sources = ordered.Skip(1).Select(p => p.Id).ToList();
                if (sources.Count > 0)
                    merges[target.Id] = sources;
            }

            return new MergePlan
            {
                Merges = merges,
                ConflictPersonCount = conflictPersonCount
            };
        }

        private static bool SameBirthAndOffice(PersonSnapshot a, PersonSnapshot b)
        {
            if (!TryExactBirthDate(a.BirthDate, out var birthA)
                || !TryExactBirthDate(b.BirthDate, out var birthB)
                || birthA != birthB)
                return false;

            return Nullable.Equals(a.MilitaryOfficeId, b.MilitaryOfficeId);
        }
    }
}
