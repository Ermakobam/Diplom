using System.ComponentModel.DataAnnotations;

namespace IPriziv.Server.DTOs.Income
{
    public class IncomeControlPerson
    {
        [Required(AllowEmptyStrings = false), MaxLength(150)]
        public string LastName { get; set; }
        [MaxLength(150)]
        public string FirstName { get; set; }
        [MaxLength(150)]
        public string MiddleName { get; set; }
        public byte? MilitaryOfficeId { get; set; }
        public DateTime? BirthDate { get; set; }

        [MaxLength(100)]
        public string State { get; set; }
        public bool IsConfirmed { get; set; }
        public bool IsExpected { get; set; }
        public bool IsCalled { get; set; }
        public bool IsPersonalFileDeclared { get; set; }

        public DateTime? ExpectedDeliveryDate { get; set; }
        public DateTime? ClarifyBeforeDate { get; set; }
        [Range(0, 30)]
        public byte? RemindPeriod { get; set; }

        [MaxLength(450)]
        public string Note { get; set; }
    }
}
