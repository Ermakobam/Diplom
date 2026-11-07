namespace IPriziv.Server.Engine.Controls
{
    /// <summary>Назначение контроля для сверки с командой.</summary>
    public sealed class ControlDestinationRef
    {
        public string DisplayName { get; init; }
        public short? MilitaryUnitId { get; init; }
        public string MilitaryUnitName { get; init; }
        public string Place { get; init; }
        public byte? MilitaryTypeId { get; init; }
        public string MilitaryTypeName { get; init; }
    }
}
