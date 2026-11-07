using IPriziv.Server.Data.Enumerations;

namespace IPriziv.Server.Engine.Controls
{
    /// <summary>
    /// ТСП: призывников с трудной семейной ситуацией можно отправлять
    /// только в части Москвы или Московской области.
    /// </summary>
    public static class TeamTspFitnessRules
    {
        public static bool HasTsp(FamilySituation? familySituation)
            => familySituation is not null and not FamilySituation.Normal;

        /// <summary>Команда относится к Москве или Московской области.</summary>
        public static bool IsMoscowRegionUnit(IEnumerable<string> teamUnitTokens)
        {
            if (teamUnitTokens == null)
                return false;

            return ControlMoscowUnitRules.TeamIsMoscowUnit(teamUnitTokens)
                   || ControlMoscowOblastUnitRules.TeamIsMoscowOblastUnit(teamUnitTokens);
        }

        /// <summary>
        /// Устаревший транспортный признак «дальняя» (дни в пути / самолёт).
        /// Для ТСП предпочтителен <see cref="IsMoscowRegionUnit"/>.
        /// </summary>
        public static bool IsFarTeam(byte? daysInRoad, TeamTransportType? transportType)
            => (daysInRoad ?? 0) > 0 || transportType == TeamTransportType.Airplane;

        public static TeamTspFitnessVerdict Evaluate(
            FamilySituation? familySituation,
            IEnumerable<string> teamUnitTokens)
        {
            if (!HasTsp(familySituation))
                return TeamTspFitnessVerdict.Fit();

            if (IsMoscowRegionUnit(teamUnitTokens))
                return TeamTspFitnessVerdict.Fit();

            return TeamTspFitnessVerdict.Unfit(
                "tsp_outside_moscow_region",
                "ТСП: только Москва или Московская область");
        }

        /// <summary>
        /// Совместимость: без токенов в/ч — по старому признаку «дальняя».
        /// </summary>
        public static TeamTspFitnessVerdict Evaluate(
            FamilySituation? familySituation,
            byte? daysInRoad,
            TeamTransportType? transportType)
        {
            if (!HasTsp(familySituation))
                return TeamTspFitnessVerdict.Fit();

            if (!IsFarTeam(daysInRoad, transportType))
                return TeamTspFitnessVerdict.Fit();

            return TeamTspFitnessVerdict.Unfit(
                "tsp_far_team",
                "ТСП: не подходит в дальнюю команду");
        }
    }

    public sealed class TeamTspFitnessVerdict
    {
        public bool IsFit { get; init; }
        public string ReasonCode { get; init; }
        public string Reason { get; init; }

        public static TeamTspFitnessVerdict Fit()
            => new() { IsFit = true, ReasonCode = "ok" };

        public static TeamTspFitnessVerdict Unfit(string reasonCode, string reason)
            => new() { IsFit = false, ReasonCode = reasonCode, Reason = reason };
    }
}
