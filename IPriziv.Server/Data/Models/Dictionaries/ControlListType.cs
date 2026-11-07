using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using IPriziv.Server.Data.Enumerations;

namespace IPriziv.Server.Data.Models.Dictionaries
{
    [Table("S_ConType")]
    public class ControlListType
    {
        [Key, Column("ID_ConType")]
        public int Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string DefaultDocumentNumber { get; set; }
        public bool ReportRequired { get; set; }
        public ControlSourceType ControlSourceType { get; set; }
    }
}
