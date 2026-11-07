using IPriziv.Server.Data.Enumerations;

namespace IPriziv.Server.Engine.Appearance
{
    /// <summary>Разбор столбца «ТСП» из плана явки.</summary>
    public static class AppearanceFamilySituationRules
    {
        public static FamilySituation? TryParse(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return null;

            var raw = text.Trim();
            var compact = raw.Replace(" ", "", StringComparison.Ordinal)
                .Replace(".", "", StringComparison.Ordinal)
                .ToLowerInvariant();

            if (IsNo(compact, raw))
                return FamilySituation.Normal;

            if (compact.Contains("дет", StringComparison.Ordinal)
                || compact.Contains("ребен", StringComparison.Ordinal)
                || compact.Contains("ребён", StringComparison.Ordinal)
                || compact.Equals("children", StringComparison.Ordinal))
                return FamilySituation.Children;

            if (compact.Contains("пенсионер", StringComparison.Ordinal)
                || compact.Contains("pensioner", StringComparison.Ordinal))
                return FamilySituation.PensionerParent;

            if (compact.Contains("инвалид", StringComparison.Ordinal)
                || compact.Contains("больн", StringComparison.Ordinal)
                || compact.Contains("sick", StringComparison.Ordinal))
                return FamilySituation.SickParent;

            if (IsYes(compact, raw))
                return FamilySituation.Present;

            return null;
        }

        /// <summary>
        /// «Да» без уточнения не затирает уже выбранный конкретный тип ТСП.
        /// </summary>
        public static FamilySituation? Merge(FamilySituation? current, FamilySituation? incoming)
        {
            if (incoming == null)
                return null;

            if (incoming == FamilySituation.Present
                && current is FamilySituation.Children
                    or FamilySituation.PensionerParent
                    or FamilySituation.SickParent
                    or FamilySituation.Present)
            {
                return current;
            }

            return incoming;
        }

        private static bool IsNo(string compact, string raw)
            => compact is "нет" or "н" or "no" or "n" or "0" or "-" or "ложь" or "false"
               || raw.Equals("Normal", StringComparison.OrdinalIgnoreCase);

        private static bool IsYes(string compact, string raw)
            => compact is "да" or "д" or "yes" or "y" or "1" or "+" or "истина" or "true"
               || compact == "тсп"
               || compact.Contains("трудн", StringComparison.Ordinal)
               || compact.Contains("сложн", StringComparison.Ordinal)
               || raw.Equals("Present", StringComparison.OrdinalIgnoreCase);
    }
}
