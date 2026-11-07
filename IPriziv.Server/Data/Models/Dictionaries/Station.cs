using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using IPriziv.Server.Data.Enumerations;
using IPriziv.Server.Data.Models.ConstantDictionaries;

namespace IPriziv.Server.Data.Models.Dictionaries
{
    [Table("S_Station")]
    public class Station
    {
        [Key]
        [Column("ID_Station")]
        public short Id { get; set; }
        public string Name { get; set; }
        public byte DaysInRoad { get; set; }
        public TeamTransportType TransportType { get; set; }


        [ForeignKey("FederalDistrict")]
        public int? FederalDistrictId { get; set; }
        [Column("ID_Railroad"), ForeignKey("Railroad")]
        public byte? RailroadId { get; set; }
        [ForeignKey("TransportSourcePoint")]
        public int? TransportSourcePointId { get; set; }
        [ForeignKey("ConnectedStation")]
        public short? ConnectedStationId { get; set; }


        public Station ConnectedStation { get; set; }
        public FederalDistrict FederalDistrict { get; set; }
        public Railroad Railroad { get; set; }
        public TransportSourcePoint TransportSourcePoint { get; set; }
    }
}
