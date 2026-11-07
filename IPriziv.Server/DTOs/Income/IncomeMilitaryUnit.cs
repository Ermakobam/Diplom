using System.ComponentModel.DataAnnotations;

namespace IPriziv.Server.DTOs.Income
{
    public class IncomeMilitaryUnit
    {
        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; }
        public string Disposition { get; set; }
        public bool IsEducational { get; set; }
        public bool IsCATE { get; set; }

        public int MilitarySubjectId { get; set; }
        public byte MilitaryDistrictId { get; set; }
        public short StationId { get; set; }
        public string StationName { get; set; }
        public List<IncomeAlternativeName> AlternativeNames { get; set; }
    }
    public class IncomeAlternativeName
    {
        public string Name { get; set; }
    }
}
