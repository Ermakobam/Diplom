using System.ComponentModel.DataAnnotations;

namespace IPriziv.Server.DTOs.Income
{
    public class IncomeControlRecruit
    {
        [Required(AllowEmptyStrings = false)]
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string Note { get; set; }
        public DateTime? ExpectedDeliveryDate { get; set; }
        public DateTime? BirthDate { get; set; }
        public byte? MilitaryOfficeId { get; set; }
        public int? ControlPersonId { get; set; }
        public bool IsCalled { get; set; }
        public bool IsPersonalFileDeclared { get; set; }
    }
}
