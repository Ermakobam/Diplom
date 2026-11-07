using System.ComponentModel.DataAnnotations;

namespace IPriziv.Server.DTOs.Income
{
    public class IncomeMilitaryRank
    {
        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; }
        public string NameGC { get; set; }
        [Required(AllowEmptyStrings = false)]
        public string NameShort { get; set; }
        public string NameShortGC { get; set; }
    }
}
