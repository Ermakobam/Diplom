namespace IPriziv.Server.DTOs
{
    public record MilitaryUnitModel
    {
        public short Id { get; set; }
        public string Name { get; set; }
        public short StationId { get; set; }
        public string Station { get; set; }
        public short RailroadId { get; set; }
        public string Railroad { get; set; }
    }
}
