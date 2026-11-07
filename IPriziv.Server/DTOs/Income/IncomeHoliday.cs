using System.ComponentModel.DataAnnotations;
using IPriziv.Server.Data.Enumerations;

namespace IPriziv.Server.DTOs.Income
{
    public class IncomeHoliday
    {
        [Required]
        public DateTime? Date { get; set; }
    }
}
