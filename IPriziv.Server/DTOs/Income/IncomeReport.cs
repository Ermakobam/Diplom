using System.ComponentModel.DataAnnotations;
using IPriziv.Server.Data.Enumerations;

namespace IPriziv.Server.DTOs.Income
{
    public class IncomeReport
    {
        public List<IncomeReportItem> Items { get; set; }
        public DateTime Date { get; set; }
    }
    public class IncomeReportItem
    {
        public DateTime? CreatedAt { get; set; }

        // нахуя было делать short? умники..
        public int ExternalSubTeamId { get; set; }

        public int SendCount { get; set; }
        public int SendCountDriversC { get; set; }
        public int SendCountDriversD { get; set; }
        public int SendCountDriversE { get; set; }
        public int SendCountHighEducated { get; set; }

        public ReportTeamState State { get; set; }
        [Required]
        public DateTime? SentAt { get; set; }
        [Required]
        public DateTime? ArrivedAt { get; set; }

        public short? TeamDispatchingId { get; set; }
    }
}
