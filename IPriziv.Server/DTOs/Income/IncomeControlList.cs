using System.ComponentModel.DataAnnotations;

namespace IPriziv.Server.DTOs.Income
{
    public class IncomeControlList
    {
        [Required]
        public byte? ControlListTypeId { get; set; }
        public string DocumentNumber { get; set; }
        public DateTime? DocumentDate { get; set; }
        public string Place { get; set; }
        public short? MilitaryUnitId { get; set; }
        public byte? MilitaryTypeId { get; set; }
        public string Orderer { get; set; }
        public string Demander { get; set; }
        public string Note { get; set; }
    }
}
