namespace IPriziv.Server.DTOs.Outcome
{
    public record OutcomeSubTeam
    {
        public short SubId { get; set; }
        public DateTime? OutDate { get; set; }
        public string Regime { get; set; }
        public byte? RostoId { get; set; }
        public short? VusId { get; set; }
        public byte? RegimeId { get; set; }
        public byte? AdmissId { get; set; }
        public byte? HealthId { get; set; }
        public int? SendPlanCount { get; set; }
        public int? SentCount { get; set; }
        public int? ReportedCount { get; set; }
        public int? SentSpecialistsCount { get; set; }
        public string AdditionalRequirements { get; set; }

        public bool IsEducational { get; set; }
        public string HealthCategoryName { get; internal set; }
        public string RegimeName { get; internal set; }
        public string AdmissName { get; internal set; }
        public string MilitaryDebtSpecialityName { get; internal set; }
        public string MilitarySpecialityName { get; internal set; }
    }
}
