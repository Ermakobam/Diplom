using System.ComponentModel.DataAnnotations;

namespace IPriziv.Server.DTOs.Income
{
    public class IncomeMonitor
    {
        [Required]
        public int? DocsOnModule { get; set; }
        [Required]
        public int? MilDocs { get; set; }
        [Required]
        public int? RecruitsOnClothes { get; set; }
        [Required]
        public int? DocsOnBanks { get; set; }
    }
}
