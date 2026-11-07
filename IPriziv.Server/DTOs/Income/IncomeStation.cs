using System.ComponentModel.DataAnnotations;
using IPriziv.Server.Data.Enumerations;

namespace IPriziv.Server.DTOs.Income
{
    public class IncomeStation
    {
        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; }
        public byte? RailroadId { get; set; }
        public TeamTransportType TransportType { get; set; }
        public byte DaysInRoad { get; set; }
        [Required]
        public int? FederalDistrictId { get; set; }
        [Required]
        public int? TransportSourcePointId { get; set; }
        public short? ConnectedStationId { get; set; }

    }
}
