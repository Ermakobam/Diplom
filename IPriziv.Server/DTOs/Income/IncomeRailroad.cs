using System.ComponentModel.DataAnnotations;

namespace IPriziv.Server.DTOs.Income
{
    public class IncomeRailroad
    {
        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; }
        public string NameGC { get; set; }
    }
}
