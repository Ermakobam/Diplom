using System.Text.RegularExpressions;

namespace IPriziv.Server.Engine.Health
{
    /// <summary>Разбор и нормализация статей заболеваний из текстовых полей.</summary>
    public static class DiseaseArticleParser
    {
        private static readonly Regex ArticleTokenRegex = new(
            @"\d+\s*-?\s*[а-яёa-z]?",
            RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

        public static IReadOnlyList<string> Extract(params string[] sources)
        {
            var result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var source in sources)
            {
                if (string.IsNullOrWhiteSpace(source))
                    continue;

                foreach (Match match in ArticleTokenRegex.Matches(source))
                {
                    var normalized = Normalize(match.Value);
                    if (!string.IsNullOrEmpty(normalized))
                        result.Add(normalized);
                }
            }

            return result.OrderBy(x => x, StringComparer.OrdinalIgnoreCase).ToList();
        }

        public static string Normalize(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return "";

            var chars = value.ToLowerInvariant()
                .Replace("ё", "е", StringComparison.Ordinal)
                .Where(char.IsLetterOrDigit)
                .ToArray();
            return new string(chars);
        }

        public static string NormalizeHealthCategory(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return "";

            return new string(value.ToLowerInvariant()
                .Replace("ё", "е", StringComparison.Ordinal)
                .Where(char.IsLetterOrDigit)
                .ToArray());
        }

        public static HashSet<string> NormalizeSet(string csv)
            => csv.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(Normalize)
                .Where(x => !string.IsNullOrEmpty(x))
                .ToHashSet(StringComparer.OrdinalIgnoreCase);
    }
}
