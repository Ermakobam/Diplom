using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using IPriziv.Server.Data.Models.Dictionaries;

namespace IPriziv.Server.Data.Models
{
    [Table("T_Control")]
    public class ControlList
    {
        [Key, Column("ID_Control")]
        public short Id { get; set; }

        [MaxLength(15)]
        public string DocumentNumber { get; set; }
        public DateTime? DocumentDate { get; set; }
        public DateTime CreatedAt { get; set; }
        [MaxLength(450)]
        public string Note { get; set; }
        [MaxLength(150)]
        public string Orderer { get; set; }
        [MaxLength(150)]
        public string Demander { get; set; }
        [MaxLength(150)]
        public string Place { get; set; }

        /// <summary>
        /// Изменение численности после последнего импорта докладов/допусков (текущее − до обновления).
        /// </summary>
        [Column("import_count_delta")]
        public int? ImportCountDelta { get; set; }

        [Column("ID_MilUnit"), ForeignKey("MilitaryUnit")]
        public short? MilitaryUnitId { get; set; }
        [Column("ID_MilType"), ForeignKey("MilitaryType")]
        public byte? MilitaryTypeId { get; set; }
        [Column("ID_ConType"), ForeignKey("ControlListType")]
        public int ControlListTypeId { get; set; }

        public ControlListType ControlListType { get; set; }

        public MilitaryUnit MilitaryUnit { get; set; }
        public MilitaryType MilitaryType { get; set; }

        // NOTE: вот это в бан!!!
        public List<ControlRecruit> ControlRecruits { get; set; }
    }
}
