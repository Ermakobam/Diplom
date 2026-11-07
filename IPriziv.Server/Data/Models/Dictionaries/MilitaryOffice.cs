using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IPriziv.Server.Data.Models.Dictionaries
{
    [Table("S_OVK")]
    public class MilitaryOffice
    {
        [Key, Column("ID_OVK")]
        public byte Id { get; set; }
        [Column("Z_indef")]
        public string Identificator { get; set; }
        [Column("ID_District"), ForeignKey("CityDistrict")]
        public byte? CityDistrictId { get; set; }
        public string Name { get; set; }
        [Column("Name_GC")]
        public string NameGC { get; set; }
        [Column("who")]
        public string DistrictTypeGC { get; set; }
        [Column("ovkp")]
        public string DistrictTypeShortGC { get; set; }

        public CityDistrict CityDistrict { get; set; }

        public List<Recruit> Recruits { get; set; }
    }
}
