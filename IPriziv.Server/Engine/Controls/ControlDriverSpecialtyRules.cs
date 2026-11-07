using System.Text.RegularExpressions;
using IPriziv.Server.Data.Enumerations;

namespace IPriziv.Server.Engine.Controls
{
    /// <summary>
    /// Контроль «водитель С»: не назначение в конкретную часть,
    /// а допуск в команды, где остались дырки под водителей.
    /// </summary>
    public static class ControlDriverSpecialtyRules
    {
        public const string NoDriverHolesReason =
            "В части отсутствуют дырки под водителей";

        /// <summary>
        /// Специальности, которые считаются «дырками под водителей» (как в нарядах/сборе).
        /// </summary>
        public static readonly DriverLicenseType[] DriverSpecialtyTypes =
        {
            DriverLicenseType.DriverC,
            DriverLicenseType.DriverD,
            DriverLicenseType.DriverE,
            DriverLicenseType.Parachutist,
        };

        /// <summary>
        /// «водитель С/C», «водители "C"», «водитель категории C» и т.п.
        /// </summary>
        private static readonly Regex DriverCRegex = new(
            @"водител\w*.{0,24}(?:категор\w*.{0,8})?[""'«]?\s*[CcСс]\b",
            RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

        public static bool IsDriverSpecialty(byte? specialtyId)
        {
            if (specialtyId == null)
                return false;
            var value = (DriverLicenseType)specialtyId.Value;
            for (var i = 0; i < DriverSpecialtyTypes.Length; i++)
            {
                if (DriverSpecialtyTypes[i] == value)
                    return true;
            }
            return false;
        }

        public static int ComputeDriverVacant(int plannedSlots, int dispatchedCount)
            => Math.Max(0, plannedSlots - dispatchedCount);

        public static bool IsDriverCControl(ControlDestinationRef control)
        {
            if (control == null)
                return false;
            return IsDriverCText(control.DisplayName)
                   || IsDriverCText(control.Place)
                   || IsDriverCText(control.MilitaryTypeName)
                   || IsDriverCText(control.MilitaryUnitName);
        }

        public static bool IsDriverCText(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return false;
            return DriverCRegex.IsMatch(text);
        }

        /// <summary>
        /// Контроль «водитель С» подходит команде, если есть дырки под водителей.
        /// </summary>
        public static bool MatchesDriverVacancy(bool hasDriverCControl, int driverVacant)
            => hasDriverCControl && driverVacant > 0;
    }
}
