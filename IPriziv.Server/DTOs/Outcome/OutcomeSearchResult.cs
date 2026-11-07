using IPriziv.Server.Data.Enumerations;

namespace IPriziv.Server.DTOs.Outcome
{
    public class OutcomeSearchResult
    {
        public int? RecruitId { get; set; }
        public bool HasControl { get; set; }
        public DateTime? BirthDate { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string MilitaryOfficeName { get; set; }
        public byte? MilitaryOfficeId { get; set; }
        public double SearchDistance { get; set; }
        public string DataBase { get; set; }
        public int Id { get; internal set; }
        public LastAction LastAction { get; set; }
        public string Barcode { get; set; }
        public int BagStatus { get; set; }
    }

    public class LastAction
    {
        public DateTime? CreatedAt { get; set; }
        public RecruitState State { get; set; }
        public short? InternalTeamId { get; set; }
        public string MilitaryUnitName { get; set; }
        public string MilitaryTypeName { get; set; }
        public string StationName { get; set; }
        public string OutDate { get; set; }
        public string? ExpectedDate { get; set; }
        public string? Health { get; set; }
    }
}
