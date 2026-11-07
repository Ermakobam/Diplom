using System.ComponentModel.DataAnnotations;
using IPriziv.Server.Data.Enumerations;

namespace IPriziv.Server.DTOs.Income
{
    public class IncomeControlListType
    {
        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; }
        [Required(AllowEmptyStrings = false)]
        public string DisplayName { get; set; }
        public string DefaultDocumentNumber { get; set; }
        public ControlSourceType ControlSourceType { get; set; }
        public bool ReportRequired { get; set; }
    }
}
