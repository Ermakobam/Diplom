using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IPriziv.Server.Data.Models.Dictionaries
{

    [Table("S_FederalSubjectType")]
    public class FederalSubjectType
    {

        [Key]
        public int Id { get; set; }
        [Required(AllowEmptyStrings = false), MaxLength(100)]
        public string Name { get; set; }

        [Required(AllowEmptyStrings = false), MaxLength(100)]
        public string View { get; set; }

    }
}
