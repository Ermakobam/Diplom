using System.Globalization;
using System.Text.RegularExpressions;

namespace IPriziv.Server.Engine.Controls
{
    /// <summary>Чистые правила разбора списка Локтева (ODS: № п/п, ФИО, д.р., Куда, От кого).</summary>
    public static class LoktevListImportRules
    {
        public const string ControlTypeName = "Записки";
        public const string Demander = "Локтев";
        public const int PlaceMaxLength = 150;
        public const int OrdererMaxLength = 150;

        private static readonly CultureInfo Ru = CultureInfo.GetCultureInfo("ru-RU");

        private static readonly Regex WhitespaceRegex = new(@"\s+", RegexOptions.Compiled);
        private static readonly Regex YearSuffixRegex = new(@"г\.?\s*$", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        private static readonly Regex DotSpaceRegex = new(@"\.\s+", RegexOptions.Compiled);
        private static readonly Regex MilitaryUnitCodeRegex = new(
            @"в\s*/\s*ч\s*[№#]?\s*(?<code>\d{4,6}(?:-\d+)?)",
            RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

        public sealed class ParsedLoktevRow
        {
            public string SerialNumber { get; init; }
            public string LastName { get; init; }
            public string FirstName { get; init; }
            public string MiddleName { get; init; }
            public DateTime? BirthDate { get; init; }
            public string Place { get; init; }
            public string Orderer { get; init; }
            public string MilitaryUnitCode { get; init; }
            public string RawFio { get; init; }
        }

        public static bool TryParseRow(IReadOnlyList<string> cells, out ParsedLoktevRow row, out string skipReason)
        {
            row = null;
            skipReason = null;

            if (cells == null || cells.Count == 0 || IsEmptyRow(cells))
            {
                skipReason = "пустая строка";
                return false;
            }

            var serial = GetCell(cells, 0);
            var fio = GetCell(cells, 1);
            var birthText = GetCell(cells, 2);
            var placeRaw = GetCell(cells, 3);
            var ordererRaw = GetCell(cells, 4);

            if (IsHeaderRow(serial, fio, birthText))
            {
                skipReason = "заголовок";
                return false;
            }

            if (string.IsNullOrWhiteSpace(fio) || !ControlPersonName.TryParseFio(fio, out var last, out var first, out var middle))
            {
                skipReason = "нет ФИО";
                return false;
            }

            TryExtractMilitaryUnitCode(placeRaw, out var unitCode);

            row = new ParsedLoktevRow
            {
                SerialNumber = NormalizeSerialNumber(serial),
                LastName = last,
                FirstName = first,
                MiddleName = middle,
                BirthDate = ParseBirthDate(birthText),
                Place = Truncate(placeRaw?.Trim() ?? "", PlaceMaxLength),
                Orderer = Truncate(ordererRaw?.Trim() ?? "", OrdererMaxLength),
                MilitaryUnitCode = unitCode,
                RawFio = fio.Trim()
            };
            return true;
        }

        public static bool TryExtractMilitaryUnitCode(string place, out string code)
        {
            code = null;
            if (string.IsNullOrWhiteSpace(place))
                return false;

            var match = MilitaryUnitCodeRegex.Match(place);
            if (!match.Success)
                return false;

            code = match.Groups["code"].Value;
            return !string.IsNullOrWhiteSpace(code);
        }

        public static DateTime? ParseBirthDate(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return null;

            var normalized = NormalizeBirthText(text);
            if (string.IsNullOrWhiteSpace(normalized))
                return null;

            var formats = new[]
            {
                "dd.MM.yyyy", "d.M.yyyy", "dd.MM.yy", "d.M.yy",
                "yyyy-MM-dd", "dd/MM/yyyy", "d/M/yyyy", "d/M/yy"
            };

            if (DateTime.TryParseExact(normalized, formats, Ru, DateTimeStyles.None, out var exact))
                return NormalizeBirthDate(exact);

            if (DateTime.TryParse(normalized, Ru, DateTimeStyles.None, out var parsed))
                return NormalizeBirthDate(parsed);

            if (int.TryParse(normalized, NumberStyles.Integer, CultureInfo.InvariantCulture, out var yearOnly)
                && yearOnly >= 1950
                && yearOnly <= DateTime.Now.Year - 16)
            {
                return new DateTime(yearOnly, 1, 1);
            }

            return null;
        }

        public static string NormalizeSerialNumber(string serial)
        {
            if (string.IsNullOrWhiteSpace(serial))
                return "";

            var trimmed = serial.Trim();
            if (double.TryParse(trimmed, NumberStyles.Float, CultureInfo.InvariantCulture, out var numeric)
                && Math.Abs(numeric % 1) < 0.0000001
                && numeric > 0
                && numeric < 1_000_000)
            {
                return ((long)numeric).ToString(CultureInfo.InvariantCulture);
            }

            return trimmed;
        }

        public static string Truncate(string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value))
                return value ?? "";
            return value.Length <= maxLength ? value : value[..maxLength];
        }

        public static string NormalizeBirthText(string text)
        {
            var value = text.Replace('\u00A0', ' ').Trim();
            value = WhitespaceRegex.Replace(value, " ");
            value = YearSuffixRegex.Replace(value, "").Trim();
            value = DotSpaceRegex.Replace(value, ".");
            return value;
        }

        public static DateTime? NormalizeBirthDate(DateTime date)
        {
            var normalized = date.Date;
            if (normalized.Year < 1950 || normalized.Year > DateTime.Now.Year - 16)
                return null;
            return normalized;
        }

        public static bool IsHeaderRow(string serial, string fio, string birth)
        {
            if (!string.IsNullOrWhiteSpace(fio)
                && fio.Contains("ФИО", StringComparison.OrdinalIgnoreCase))
                return true;

            if (!string.IsNullOrWhiteSpace(serial)
                && (serial.Contains("№", StringComparison.Ordinal)
                    || serial.Contains("п/п", StringComparison.OrdinalIgnoreCase)))
                return true;

            if (!string.IsNullOrWhiteSpace(birth)
                && (birth.Contains("д.р", StringComparison.OrdinalIgnoreCase)
                    || birth.Contains("дата", StringComparison.OrdinalIgnoreCase)))
                return true;

            return false;
        }

        private static bool IsEmptyRow(IReadOnlyList<string> cells)
            => cells.All(c => string.IsNullOrWhiteSpace(c));

        private static string GetCell(IReadOnlyList<string> cells, int index)
            => index < cells.Count ? cells[index] ?? "" : "";
    }
}
