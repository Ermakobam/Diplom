namespace IPriziv.Server.DTOs.Income
{
    public class IncomeGatheredTeamManager
    {
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string ShortFullName { get; set; }
        public string PhoneNumber { get; set; }
        public string ShortFullNameGC { get; set; }
        public string Sex { get; set; }
        public string PersonPasportSeries { get; set; }
        public string PersonPasportNumber { get; set; }
        public string PersonDocumentSeries { get; set; }
        public string PersonDocumentNumber { get; set; }
        public string PersonDocumentIssuedAt { get; set; }
        public string PersonDocumentIssuer { get; set; }
        public string PersonDocumentType { get; set; }
        public string FullAliasGC { get; set; }
        public int OfficerCount { get; set; }
        public int EnsignCount { get; set; }
        public int SergeantCount { get; set; }
        public string BornCity { get; set; }
        public DateTime? BirthDate { get; set; }
        public string MilitaryUnitCode { get; set; }
        public string MilitaryUnitAddress { get; set; }
        public DateTime? InDate { get; set; }
        public DateTime? OutDate { get; set; }
        public int InternalTeamId { get; set; }
        public byte? MilitaryRankId { get; set; }
    }
}
