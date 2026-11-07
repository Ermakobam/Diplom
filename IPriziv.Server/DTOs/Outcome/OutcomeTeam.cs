namespace IPriziv.Server.DTOs.Outcome
{
    public record OutcomeTeam
    {
        public short Id { get; set; }
        public int? InternalTeamId { get; set; }
        public string MVO { get; set; }
        public int? ShouldSend { get; set; }
        public int? ShouldSendSpecialists { get; set; }
        public int? ReportedCount { get; set; }
        public int? ReportedSpecialistsCount { get; set; }
        public int? SentCount { get; set; }
        public int? SentSpecCount { get; set; }
        public string Note { get; set; }
        public short? StationId { get; set; }
        public byte? DistrictId { get; set; }
        public short MilitaryUnitId { get; set; }
        public bool IsDirectSubmission { get; set; }
        public byte MilitaryTypeId { get; set; }
        public byte? RailroadId { get; set; }
        public string MilitaryTypeName { get; set; }
        public string MilitaryUnitName { get; set; }
        public string StationName { get; internal set; }
        public string RailroadName { get; internal set; }
        public string DistrictName { get; internal set; }
        public string ConnectionMinistryId { get; set; }
    }
}
