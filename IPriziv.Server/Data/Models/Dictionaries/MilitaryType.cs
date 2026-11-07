using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IPriziv.Server.Data.Models.Dictionaries
{
    [Table("S_MilType")]
    public class MilitaryType
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity), Column("ID_MilType")]
        public byte Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        [Column("MO")]
        public bool? IsMD { get; set; }
        //Весной 2022 24 ОБ скрывалось под 14 ГЦС
        public string NameReportOverride { get; set; }
        public short OrderIndex { get; set; }
        public bool IsTotalReportVisible { get; set; }
    }
}
