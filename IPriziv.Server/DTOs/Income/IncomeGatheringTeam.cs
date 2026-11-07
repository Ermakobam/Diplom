using System.ComponentModel.DataAnnotations;

namespace IPriziv.Server.DTOs.Income
{
    public class IncomeGatheringTeam
    {
        [Required(AllowEmptyStrings = false)]
        public string Assignment { get; set; }
        [Required]
        public short? ExternalTeamId { get; set; }
        [Required]
        public int? UniformColorId { get; set; }
    }
}
