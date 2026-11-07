using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IPriziv.Server.Data.Models.ConstantDictionaries
{
    [Table("S_FederalDistrict")]
    public class FederalDistrict
    {
        [Key]
        public int Id { get; set; }
        [Required(AllowEmptyStrings = false), MaxLength(50)]
        public string Name { get; set; }
    }
}
