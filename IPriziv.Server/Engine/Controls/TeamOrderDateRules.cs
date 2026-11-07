namespace IPriziv.Server.Engine.Controls
{
    /// <summary>
    /// Дата наряда «сегодня»: до 06:00 ещё считается предыдущий календарный день
    /// (как у ночёвок и фильтра отправок).
    /// </summary>
    public static class TeamOrderDateRules
    {
        public static readonly TimeSpan DayRollover = new(6, 0, 0);

        public static DateTime GetEffectivePlanDate(DateTime now)
            => now.TimeOfDay < DayRollover ? now.Date.AddDays(-1) : now.Date;

        public static bool IsPlanDate(DateTime outDate, DateTime now)
            => outDate.Date == GetEffectivePlanDate(now);

        /// <summary>
        /// Подкоманды наряда с датой отправки = эффективное «сегодня».
        /// </summary>
        public static IReadOnlyList<T> FilterTodaySubTeams<T>(
            IEnumerable<T> subTeams,
            Func<T, DateTime> outDateSelector,
            DateTime? now = null)
        {
            if (subTeams == null)
                return Array.Empty<T>();

            var planDate = GetEffectivePlanDate(now ?? DateTime.Now);
            return subTeams
                .Where(s => outDateSelector(s).Date == planDate)
                .ToList();
        }

        public static string FormatTodaySubTeamsLabel(
            IEnumerable<(string Name, short SendPlanCount)> todaySubTeams)
        {
            var list = todaySubTeams?
                .Where(x => !string.IsNullOrWhiteSpace(x.Name) || x.SendPlanCount > 0)
                .Select(x =>
                {
                    var name = string.IsNullOrWhiteSpace(x.Name) ? "без имени" : x.Name.Trim();
                    return x.SendPlanCount > 0 ? $"{name} ({x.SendPlanCount})" : name;
                })
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList() ?? new List<string>();

            if (list.Count == 0)
                return "На сегодня подкоманд в наряде нет";

            return list.Count == 1
                ? $"Сегодня комплектуется: {list[0]}"
                : $"Сегодня комплектуются: {string.Join(", ", list)}";
        }
    }
}
