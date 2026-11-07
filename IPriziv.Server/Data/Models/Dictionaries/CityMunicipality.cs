using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IPriziv.Server.Data.Models.Dictionaries
{
    [Table("S_Municip")]
    public class CityMunicipality
    {
        [Key, Column("ID_Municip")]
        public short Id { get; set; }
        public string Name { get; set; }
        public string NameRR { get; set; }

        [Column("ID_OVK"), ForeignKey("MilitaryOffice")]
        public byte? MilitaryOfficeId { get; set; }

        public MilitaryOffice MilitaryOffice { get; set; }
        public List<Recruit> Recruits { get; set; }
    }
}
