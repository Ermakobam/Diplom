using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using IPriziv.Server.Data.Models.Dictionaries;

namespace IPriziv.Server.Data.Models
{
    [Table("T_Statistics")]
    public class SendPlanByMilitaryOffice
    {
        [Key, Column("id_municip")]
        public int CityMunicipalityId { get; set; }
        [Column("ToSend")]
        public int Count { get; set; }


        [Column("id_ovk"), ForeignKey("MilitaryOffice")]
        public byte MilitaryOfficeId { get; set; }

        public MilitaryOffice MilitaryOffice { get; set; }
    }
}
