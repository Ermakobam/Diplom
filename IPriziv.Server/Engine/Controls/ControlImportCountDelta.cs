namespace IPriziv.Server.Engine.Controls
{
    /// <summary>
    /// Дельта численности списка после импорта докладов/допусков (текущее − до обновления).
    /// </summary>
    public static class ControlImportCountDelta
    {
        public static Dictionary<short, int> Compute(
            IReadOnlyDictionary<short, int> previousCounts,
            IReadOnlyDictionary<short, int> newCounts)
        {
            var ids = previousCounts.Keys
                .Concat(newCounts.Keys)
                .Distinct();

            var result = new Dictionary<short, int>();
            foreach (var id in ids)
            {
                previousCounts.TryGetValue(id, out var previous);
                newCounts.TryGetValue(id, out var current);
                result[id] = current - previous;
            }

            return result;
        }

        public static bool IsTrackedControlTypeName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;

            if (string.Equals(name.Trim(), "Доклады", StringComparison.OrdinalIgnoreCase))
                return true;

            return name.Contains("форма допуска", StringComparison.OrdinalIgnoreCase);
        }
    }
}
