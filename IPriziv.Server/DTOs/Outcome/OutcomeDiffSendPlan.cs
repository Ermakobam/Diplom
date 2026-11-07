namespace IPriziv.Server.DTOs.Outcome;

/// <summary>
/// Остатки наряда воинских частей по роду/виду войск
/// </summary>
public sealed record OutcomeDiffSendPlan
{
    /// <summary>
    /// Род/вид войск
    /// </summary>
    public string TeamType { get; init; }

    /// <summary>
    /// Команды/воинские части
    /// </summary>
    public TeamInfo[] TeamsInfo { get; init; }
}

/// <summary>
/// Информация о воинской части
/// </summary>
public sealed record TeamInfo
{
    /// <summary>
    /// Номер части
    /// </summary>
    public string MilitaryUnit { get; init; }

    /// <summary>
    /// Род/вид войск
    /// </summary>
    public string MilitaryTypeReq { get; init; }

    /// <summary>
    /// Станция
    /// </summary>
    public string Station { get; init; }

    /// <summary>
    /// Остаток водителей
    /// </summary>
    public int DriversDiff { get; init; }

    /// <summary>
    /// Остаток наряда команды
    /// </summary>
    public int SentPlanDiff { get; init; }

    /// <summary>
    /// СПгМ
    /// </summary>
    public short InternalNumber { get; init; }

    /// <summary>
    /// Окружной номер
    /// </summary>
    public string DistrictNumber { get; init; }

}
