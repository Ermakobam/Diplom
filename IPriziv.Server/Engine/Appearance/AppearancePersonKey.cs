namespace IPriziv.Server.Engine.Appearance
{
    /// <summary>Ключ уникальности человека для импорта плана явки.</summary>
    public static class AppearancePersonKey
    {
        public static string Build(string lastName, string firstName, string middleName, DateTime birthDate)
            => $"{NormalizeName(lastName)}|{NormalizeName(firstName)}|{NormalizeName(middleName)}|{birthDate:yyyy-MM-dd}";

        public static string NormalizeName(string value)
            => (value ?? "").Trim().Replace("  ", " ", StringComparison.Ordinal);

        public static bool TryParseFio(string fio, out string lastName, out string firstName, out string middleName)
        {
            lastName = firstName = middleName = "";
            var parts = fio.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0)
                return false;
            lastName = parts[0].Trim();
            firstName = parts.Length > 1 ? parts[1].Trim() : "";
            middleName = parts.Length > 2 ? string.Join(' ', parts.Skip(2)).Trim() : "";
            return !string.IsNullOrWhiteSpace(lastName);
        }
    }
}
