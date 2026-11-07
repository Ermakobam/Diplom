using System.ComponentModel.DataAnnotations;

namespace IPriziv.Server.DTOs.Income
{
    public class IncomeSubTeam
    {
        public string Name { get; set; }
        [Required(ErrorMessage = "Поле должно быть заполнено")]
        public byte? HealthId { get; set; }
        public byte? RostoId { get; set; }
        public string AdditionalRequirements { get; set; }
        [Range(0, short.MaxValue)]
        public short SendPlanCount { get; set; }
        public byte? RegimeId { get; set; }
        public byte? AdmissId { get; set; }
        public int? CivilSpecialityId { get; set; }
        public bool IsEducational { get; set; }
        public bool IsSecretCommunication { get; set; }
        public int? UniformColorId { get; set; }
        [Required(ErrorMessage = "Поле должно быть заполнено")]
        public DateTime OutDate { get; set; }
    }
}
