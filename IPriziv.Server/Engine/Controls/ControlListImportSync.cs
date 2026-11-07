using IPriziv.Server.Data;
using IPriziv.Server.Data.Models;
using IPriziv.Server.Engine.Controls;
using Microsoft.EntityFrameworkCore;

namespace IPriziv.Server.Engine.Controls
{
    /// <summary>
    /// Синхронизация людей в записке при импорте: сохраняем совпавших,
    /// добавляем новых, удаляем отсутствующих в файле.
    /// </summary>
    public sealed class ControlListImportSyncState
    {
        public ControlList List { get; init; }
        public Dictionary<string, ControlRecruit> ExistingByKey { get; init; }
        public HashSet<string> SeenKeys { get; } = new(StringComparer.Ordinal);
        public int Added { get; set; }
        public int Removed { get; set; }
    }

    public static class ControlListImportSync
    {
        public static string PersonKey(
            string lastName,
            string firstName,
            string middleName,
            DateTime? birthDate)
        {
            var last = ControlPersonName.ToPersonCase(lastName ?? "").ToUpperInvariant();
            var first = ControlPersonName.ToPersonCase(firstName ?? "").ToUpperInvariant();
            var middle = ControlPersonName.ToPersonCase(middleName ?? "").ToUpperInvariant();
            var birth = birthDate?.Date.ToString("yyyy-MM-dd") ?? "";
            return $"{last}|{first}|{middle}|{birth}";
        }

        public static async Task<ControlListImportSyncState> BeginAsync(
            AppDbContext context,
            ControlList list,
            CancellationToken cancellationToken = default)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            var existing = list.Id == 0
                ? new List<ControlRecruit>()
                : await context.ControlRecruits
                    .Include(r => r.ControlPerson)
                    .Where(r => r.ControlListId == list.Id)
                    .ToListAsync(cancellationToken);

            var byKey = new Dictionary<string, ControlRecruit>(StringComparer.Ordinal);
            foreach (var recruit in existing)
            {
                var key = PersonKey(
                    recruit.LastName,
                    recruit.FirstName,
                    recruit.MiddleName,
                    recruit.BirthDate);
                if (!byKey.ContainsKey(key))
                    byKey[key] = recruit;
            }

            return new ControlListImportSyncState
            {
                List = list,
                ExistingByKey = byKey
            };
        }

        /// <summary>
        /// true — человека нужно добавить; false — уже есть / дубль в файле.
        /// </summary>
        public static bool ShouldAddPerson(ControlListImportSyncState state, string personKey, out bool isDuplicateInFile)
        {
            isDuplicateInFile = false;
            if (state == null || string.IsNullOrWhiteSpace(personKey))
                return false;

            if (!state.SeenKeys.Add(personKey))
            {
                isDuplicateInFile = true;
                return false;
            }

            return !state.ExistingByKey.ContainsKey(personKey);
        }

        public static void MarkAdded(ControlListImportSyncState state)
        {
            if (state != null)
                state.Added++;
        }

        public static async Task RemoveUnseenAsync(
            AppDbContext context,
            ControlListImportSyncState state,
            CancellationToken cancellationToken = default)
        {
            if (state == null || context == null)
                return;

            var toRemove = state.ExistingByKey
                .Where(kv => !state.SeenKeys.Contains(kv.Key))
                .Select(kv => kv.Value)
                .ToList();

            if (toRemove.Count == 0)
                return;

            var personIds = toRemove
                .Select(r => r.ControlPersonId)
                .Where(id => id != 0)
                .Distinct()
                .ToList();
            var removeRecruitIds = toRemove.Select(r => r.Id).ToHashSet();

            context.ControlRecruits.RemoveRange(toRemove);
            state.Removed += toRemove.Count;

            if (personIds.Count == 0)
                return;

            var stillLinkedPersonIds = await context.ControlRecruits
                .Where(r => personIds.Contains(r.ControlPersonId) && !removeRecruitIds.Contains(r.Id))
                .Select(r => r.ControlPersonId)
                .Distinct()
                .ToListAsync(cancellationToken);

            var orphanPersonIds = personIds
                .Except(stillLinkedPersonIds)
                .ToList();

            if (orphanPersonIds.Count == 0)
                return;

            var orphans = await context.ControlPersons
                .Where(p => orphanPersonIds.Contains(p.Id))
                .ToListAsync(cancellationToken);
            if (orphans.Count > 0)
                context.ControlPersons.RemoveRange(orphans);
        }

        /// <summary>
        /// Итоговые численности: для нетронутых списков — как было;
        /// для синхронизированных — previous + added − removed.
        /// </summary>
        public static Dictionary<short, int> BuildNewCounts(
            IReadOnlyDictionary<short, int> previousCounts,
            IEnumerable<ControlListImportSyncState> syncStates)
        {
            var result = new Dictionary<short, int>();
            if (previousCounts != null)
            {
                foreach (var (listId, count) in previousCounts)
                    result[listId] = count;
            }

            if (syncStates == null)
                return result;

            foreach (var state in syncStates)
            {
                if (state?.List == null || state.List.Id == 0)
                    continue;

                previousCounts.TryGetValue(state.List.Id, out var previous);
                result[state.List.Id] = previous + state.Added - state.Removed;
            }

            return result;
        }
    }
}
