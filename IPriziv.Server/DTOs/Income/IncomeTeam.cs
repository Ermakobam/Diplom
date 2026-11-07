using System.ComponentModel.DataAnnotations;

namespace IPriziv.Server.DTOs.Income
{
    public class IncomeTeam
    {
        [Required(ErrorMessage = "Поле {0} должно быть заполнено")]
        public short? InternalTeamId { get; set; }
        [Required(ErrorMessage = "Поле {0} должно быть заполнено")]
        public string MVO { get; set; }
        [Required(ErrorMessage = "Поле {0} должно быть заполнено")]
        public short? MilitaryTypeId { get; set; }
        [Required(ErrorMessage = "Поле {0} должно быть заполнено")]
        public short? MilitaryUnitId { get; set; }
        public bool IsDirectSubmission { get; set; }
        public string ConnectionMinistryId { get; set; }
        public string Note { get; set; }
        public string OrderChangeNote { get; set; }
        public DateTime? StartDispatchPeriod { get; set; }
        public DateTime? EndDispatchPeriod { get; set; }
    }
}
