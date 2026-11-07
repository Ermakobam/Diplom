using System.ComponentModel.DataAnnotations;

namespace IPriziv.Server.Data.Models
{
    public class ApiKey
    {
        [Key]
        public int Id { get; set; }
        [Required(AllowEmptyStrings = false), MaxLength(36), MinLength(36)]
        public string Key { get; set; }
        public string Description { get; set; }
        public DateTime ValidBefore { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
