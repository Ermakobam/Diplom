using IPriziv.Server.Data.Enumerations;

namespace IPriziv.Server.DTOs.Income
{
    public class IncomeTeamManagerEscort
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string MilitaryDocumentSeries { get; set; }
        public string MilitaryDocumentNumber { get; set; }
        public string PassportSeries { get; set; }
        public string PassportNumber { get; set; }
        public DateTime BirthDate { get; set; }
        public string BirthPlace { get; set; }
        public byte? MilitaryRankId { get; set; }
        public Sex Sex { get; set; }
        public DateTime? MilitaryDocumentIssuedAt { get; set; }
        public DateTime? PassportIssuedAt { get; set; }
    }
}
