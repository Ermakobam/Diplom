namespace IPriziv.Server.Data.Enumerations
{
    public enum StartHealthCategory : short
    {
        None = 0,
        Unfit = 1,
        LimitedFit = 2,
        TemporaryUnfit = 5,
        RestrictedFit = 15,
        Fit = 31
    }
}
