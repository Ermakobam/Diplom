using System.ComponentModel.DataAnnotations;

namespace IPriziv.Server.DTOs.Income
{
    public class IncomeHealth
    {
        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; }
        public string HealthCategoryGroupName { get; set; }
    }
}
