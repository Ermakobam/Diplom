namespace IPriziv.Server.DTOs
{
    public record RostoModel
    {
        public byte Id { get; set; }
        public string Name { get; set; }
        public short? VusId { get; set; }
    }
}
