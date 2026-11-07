using IPriziv.Server.Data.Enumerations;

namespace IPriziv.Server.Engine.Recruits
{
    /// <summary>
    /// Правила поиска свободного призывника по штрихкоду на комплектовании.
    /// </summary>
    public static class GatheringBarcodeRules
    {
        public static string Normalize(string barcode)
            => (barcode ?? string.Empty).Trim();

        public static bool IsUsableBarcode(string barcode)
            => !string.IsNullOrWhiteSpace(Normalize(barcode));

        /// <summary>
        /// Свободный для переноса в команду: прошёл отбор и ещё не в команде.
        /// </summary>
        public static bool IsAvailableForGathering(RecruitState state, short? gatheredSubTeamId)
            => gatheredSubTeamId == null && state == RecruitState.SelectionPassed;
    }
}
