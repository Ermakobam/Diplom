using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IPriziv.Server.Data.Models.Dictionaries
{
    [Table("S_MilUnit")]
    public class MilitaryUnit
    {
        [Key]
        [Column("ID_MilUnit")]
        public short Id { get; set; }
        public string Name { get; set; }
        public string Disposition { get; set; }
        //Закрытое административно-территориальное образование (ЗАТО)
        public bool IsCATE { get; set; }
        public bool IsEducational { get; set; }

        [Column("ID_MilSub")]
        [ForeignKey("MilitarySubject")]
        public int? MilitarySubjectId { get; set; }

        [Column("ID_MilDist")]
        [ForeignKey("MilitaryDistrict")]
        public byte? MilitaryDistrictId { get; set; }
        [Column("ID_Station")]
        [ForeignKey("Station")]
        public short? StationId { get; set; }


        public List<MilitaryUnitAlternativeName> AlternativeNames { get; set; }
        public MilitaryDistrict MilitaryDistrict { get; set; }
        public MilitarySubject MilitarySubject { get; set; }
        public Station Station { get; set; }
    }
}
