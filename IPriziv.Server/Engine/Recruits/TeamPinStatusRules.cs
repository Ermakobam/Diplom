namespace IPriziv.Server.Engine.Recruits;

/// <summary>
/// Закрепление призывника за командой (поле Recruit.TeamPinStatus).
/// 0 — на беседе, 1 — закреплён за командой.
/// Медстатус крови — отдельно в Recruit.HealthStatus (0/1/2).
/// </summary>
public static class TeamPinStatusRules
{
    public const int Interviewing = 0;
    public const int Pinned = 1;

    public const int BloodUntested = 0;
    public const int BloodHealthy = 1;
    public const int BloodUnhealthy = 2;

    /// <summary>
    /// Все добавленные в команду закреплены.
    /// </summary>
    public static bool AreAllPinned(int pinnedCount, int recruitsCount)
        => recruitsCount > 0 && pinnedCount == recruitsCount;

    /// <summary>
    /// Все добавленные в команду здоровы по крови.
    /// </summary>
    public static bool AreAllBloodHealthy(int healthyCount, int recruitsCount)
        => recruitsCount > 0 && healthyCount == recruitsCount;

    /// <summary>
    /// Команда готова к убытию: все здоровы и все закреплены.
    /// </summary>
    public static bool IsReadyToDepart(int pinnedCount, int healthyCount, int recruitsCount)
        => AreAllPinned(pinnedCount, recruitsCount)
           && AreAllBloodHealthy(healthyCount, recruitsCount);

    public static string GetPinDisplayName(int status) => status switch
    {
        Pinned => "Закреплен за командой",
        _ => "На беседе",
    };
}
