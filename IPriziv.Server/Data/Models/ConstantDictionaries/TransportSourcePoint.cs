using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using IPriziv.Server.Data.Enumerations;

namespace IPriziv.Server.Data.Models.ConstantDictionaries
{
    [Table("S_TransportSourcePoint")]
    public class TransportSourcePoint
    {
        [Key]
        public int Id { get; set; }
        [Required(AllowEmptyStrings = false), MaxLength(50)]
        public string Name { get; set; }
        public TeamTransportType TransportType { get; set; }
    }
}
