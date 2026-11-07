using System.ComponentModel.DataAnnotations;

namespace IPriziv.Server.DTOs.Income
{
    public class IncomeCreateControlTelegram
    {
        [Required]
        public DateTime? CalledAt { get; set; }

        [Required]
        public DateTime? InformBefore { get; set; }

        [Required]
        public DateTime? OverridedCreatedAt { get; set; }

        [Required]
        public List<int> ControlPersonList { get; set; }
    }
}
