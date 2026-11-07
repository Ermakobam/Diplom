using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using IPriziv.Server.Data.Models.Dictionaries;

namespace IPriziv.Server.Data.Models.ConstantDictionaries
{

    [Table("S_FederalSubject")]
    public class FederalSubject
    {

        [Key]
        public int Id { get; set; }
        [Required(AllowEmptyStrings = false), MaxLength(100)]
        public string Name { get; set; }

        [ForeignKey("FederalSubjectType")]
        public int TypeId { get; set; }

        public FederalSubjectType FederalSubjectType { get; set; }

    }
}
