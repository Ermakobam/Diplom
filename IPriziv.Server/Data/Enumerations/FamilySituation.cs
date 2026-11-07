namespace IPriziv.Server.Data.Enumerations
{
    public enum FamilySituation
    {
        Normal = 1,
        Children,
        PensionerParent,
        SickParent,
        /// <summary>ТСП без уточнения типа (из плана явки: «Да»).</summary>
        Present
    }
}
