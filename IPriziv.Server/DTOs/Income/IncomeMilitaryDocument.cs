using System.ComponentModel.DataAnnotations;
using Microsoft.VisualBasic;

namespace IPriziv.Server.DTOs.Income
{
    public class IncomeMilitaryDocument
    {

        public string militaryDocumentSeries { get; set; }
        public string militaryDocumentNumber { get; set; }
        public byte militaryDocumentIssuedId { get; set; }
        public string militaryBadgeNumber { get; set; }
        public DateTime militaryDocumentCreationDate { get; set; }
        public DateTime comissionDecisionDate { get; set; }

    }
}
