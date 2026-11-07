using System.ComponentModel.DataAnnotations;

namespace IPriziv.Server.DTOs.Income
{
    public class IncomeApiKey
    {
        [Required(AllowEmptyStrings = false)]
        public string Description { get; set; }
        [Required]
        public DateTime? ValidBefore { get; set; }
    }
}
