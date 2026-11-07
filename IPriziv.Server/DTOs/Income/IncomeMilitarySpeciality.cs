using System.ComponentModel.DataAnnotations;

namespace IPriziv.Server.DTOs.Income
{
    public class IncomeMilitarySpeciality
    {
        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
