using System.Globalization;
using System.Text.RegularExpressions;

namespace IPriziv.Server.Engine.Controls
{
    /// <summary>Разбор и нормализация ФИО для импорта в контроли.</summary>
    public static class ControlPersonName
    {
        private static readonly CultureInfo Ru = CultureInfo.GetCultureInfo("ru-RU");
        private static readonly Regex WhitespaceRegex = new(@"\s+", RegexOptions.Compiled);

        public static bool TryParseFio(string fio, out string lastName, out string firstName, out string middleName)
        {
            lastName = firstName = middleName = "";
            if (string.IsNullOrWhiteSpace(fio))
                return false;

            var normalized = WhitespaceRegex.Replace(
                fio.Replace('\u00A0', ' ').Replace('\n', ' ').Replace('\r', ' ').Trim(),
                " ");

            // Иногда ФИО через запятую: «Иванов, Иван Иванович»
            normalized = normalized.Replace(',', ' ');
            normalized = WhitespaceRegex.Replace(normalized, " ").Trim();

            var parts = normalized.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0)
                return false;

            lastName = ToPersonCase(parts[0]);
            firstName = parts.Length > 1 ? ToPersonCase(parts[1]) : "";
            middleName = parts.Length > 2
                ? string.Join(' ', parts.Skip(2).Select(ToPersonCase))
                : "";

            return !string.IsNullOrWhiteSpace(lastName);
        }

        /// <summary>ИВАНОВ / иванов → Иванов; ИВАНОВ-ПЕТРОВ → Иванов-Петров.</summary>
        public static string ToPersonCase(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return "";

            var trimmed = value.Trim();
            if (trimmed.Contains('-', StringComparison.Ordinal))
            {
                return string.Join("-", trimmed
                    .Split('-', StringSplitOptions.None)
                    .Select(ToPersonCaseSegment));
            }

            return ToPersonCaseSegment(trimmed);
        }

        private static string ToPersonCaseSegment(string segment)
        {
            if (string.IsNullOrEmpty(segment))
                return segment;

            var lower = segment.ToLower(Ru);
            if (lower.Length == 1)
                return char.ToUpper(lower[0], Ru).ToString();

            return char.ToUpper(lower[0], Ru) + lower[1..];
        }
    }
}
