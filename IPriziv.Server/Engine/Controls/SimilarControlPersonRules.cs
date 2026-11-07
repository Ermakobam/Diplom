using System;

namespace IPriziv.Server.Engine.Controls
{
    /// <summary>
    /// Похожая персона контроля: тот же год рождения и либо опечатка в ФИО,
    /// либо полное совпадение ФИО при другом военкомате.
    /// </summary>
    public static class SimilarControlPersonRules
    {
        public const int DefaultMaxFioEditDistance = 2;

        public static bool IsSimilar(
            string recruitLastName,
            string recruitFirstName,
            string recruitMiddleName,
            DateTime? recruitBirthDate,
            string recruitMilitaryOfficeName,
            string personLastName,
            string personFirstName,
            string personMiddleName,
            DateTime? personBirthDate,
            string personMilitaryOfficeName,
            int maxFioEditDistance = DefaultMaxFioEditDistance,
            bool requireSameBirthYear = true)
        {
            if (requireSameBirthYear && !SameBirthYear(recruitBirthDate, personBirthDate))
                return false;

            var fioDistance = FioEditDistance(
                recruitLastName, recruitFirstName, recruitMiddleName,
                personLastName, personFirstName, personMiddleName);

            var officeSame = string.Equals(
                Normalize(recruitMilitaryOfficeName),
                Normalize(personMilitaryOfficeName),
                StringComparison.Ordinal);

            if (fioDistance == 0)
                return !officeSame;

            return fioDistance <= maxFioEditDistance;
        }

        /// <summary>Та же персона: ФИО совпадает и военкомат тот же.</summary>
        public static bool IsExactMatch(
            string recruitLastName,
            string recruitFirstName,
            string recruitMiddleName,
            string recruitMilitaryOfficeName,
            string personLastName,
            string personFirstName,
            string personMiddleName,
            string personMilitaryOfficeName)
        {
            if (FioEditDistance(
                    recruitLastName, recruitFirstName, recruitMiddleName,
                    personLastName, personFirstName, personMiddleName) != 0)
                return false;

            return string.Equals(
                Normalize(recruitMilitaryOfficeName),
                Normalize(personMilitaryOfficeName),
                StringComparison.Ordinal);
        }

        public static int FioEditDistance(
            string lastNameA,
            string firstNameA,
            string middleNameA,
            string lastNameB,
            string firstNameB,
            string middleNameB)
        {
            var partDistance =
                EditDistance(Normalize(lastNameA), Normalize(lastNameB))
                + EditDistance(Normalize(firstNameA), Normalize(firstNameB))
                + EditDistance(Normalize(middleNameA), Normalize(middleNameB));

            // Склейка без пробелов: «РахматуловЭмильНаилович» vs раздельное ФИО
            var compactDistance = EditDistance(
                CompactFio(lastNameA, firstNameA, middleNameA),
                CompactFio(lastNameB, firstNameB, middleNameB));

            return Math.Min(partDistance, compactDistance);
        }

        public static bool SameBirthYear(DateTime? a, DateTime? b)
        {
            if (a == null || b == null)
                return false;
            return a.Value.Year == b.Value.Year;
        }

        public static bool SameCalendarDate(DateTime? a, DateTime? b)
        {
            if (a == null || b == null)
                return false;
            return a.Value.Date == b.Value.Date;
        }

        public static string CompactFio(string lastName, string firstName, string middleName)
            => Normalize(lastName) + Normalize(firstName) + Normalize(middleName);

        public static string Normalize(string value)
            => string.IsNullOrWhiteSpace(value)
                ? ""
                : value.Trim().ToLowerInvariant();

        public static int EditDistance(string a, string b)
        {
            a ??= "";
            b ??= "";
            if (a == b)
                return 0;
            if (a.Length == 0)
                return b.Length;
            if (b.Length == 0)
                return a.Length;

            var prev = new int[b.Length + 1];
            var curr = new int[b.Length + 1];
            for (var j = 0; j <= b.Length; j++)
                prev[j] = j;

            for (var i = 1; i <= a.Length; i++)
            {
                curr[0] = i;
                for (var j = 1; j <= b.Length; j++)
                {
                    var cost = a[i - 1] == b[j - 1] ? 0 : 1;
                    curr[j] = Math.Min(
                        Math.Min(curr[j - 1] + 1, prev[j] + 1),
                        prev[j - 1] + cost);
                }
                (prev, curr) = (curr, prev);
            }

            return prev[b.Length];
        }
    }
}
