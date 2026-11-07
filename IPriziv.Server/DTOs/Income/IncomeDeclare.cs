using System.ComponentModel.DataAnnotations;

namespace IPriziv.Server.DTOs.Income
{
    public class IncomeDeclare
    {
        [MaxLength(150)]
        public string Reason { get; set; }
        public bool IsAccidentCorrection { get; set; }
    }
}
