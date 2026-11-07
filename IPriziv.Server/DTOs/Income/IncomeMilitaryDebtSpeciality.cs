using System.ComponentModel.DataAnnotations;

namespace IPriziv.Server.DTOs.Income
{
    public class IncomeMilitaryDebtSpeciality
    {
        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
