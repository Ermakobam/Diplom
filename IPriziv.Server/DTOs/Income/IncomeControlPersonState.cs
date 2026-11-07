using System.ComponentModel.DataAnnotations;

namespace IPriziv.Server.DTOs.Income
{
    public class IncomeControlPersonState
    {
        [MaxLength(100)]
        public string State { get; set; }
        public bool IsConfirmed { get; set; }
        public bool IsExpected { get; set; }
        public DateTime? ExpectedDeliveryDate { get; set; }

        public DateTime? ClarifyBeforeDate { get; set; }
        public byte? RemindPeriod { get; set; }
    }
}
