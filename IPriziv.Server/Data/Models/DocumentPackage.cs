using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IPriziv.Server.Data.Models
{
    [Table("T_DocumentPackage")]
    public class DocumentPackage
    {
        [Key]
        public int Id { get; private set; }
        public bool IsDefenceMinistry { get; private set; }
        public int Number { get; private set; }
        public DateTime CreatedAt { get; private set; }

        [ForeignKey("ExternalTeam")]
        public short? ExternalTeamId { get; private set; }

        public ExternalTeam ExternalTeam { get; private set; }
        public TeamDispatching TeamDispatching { get; private set; }
        private DocumentPackage() { }
        public DocumentPackage(short externalTeamId, bool isDefenceMinistry, int number)
        {
            ExternalTeamId = externalTeamId;
            IsDefenceMinistry = isDefenceMinistry;
            Number = number;
            CreatedAt = DateTime.Now;
        }
    }
}
