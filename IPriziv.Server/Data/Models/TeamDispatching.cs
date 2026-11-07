using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IPriziv.Server.Data.Models
{
    [Table("T_Out")]
    public class TeamDispatching
    {
        [Key, Column("ID_Out")]
        public short Id { get; private set; }
        public DateTime OutDate { get; private set; }
        [Column("ID_OrderVKgM")]
        public int? OrderNumber { get; private set; }
        [Column("DryRationDays")]
        public byte FoodCount { get; private set; }
        public bool IsPrinted { get; private set; }

        [Column("IsDeliveried")]
        public bool? IsDeliveried { get; private set; }
        [Column("IsOut")]
        public bool? IsOut { get; private set; }
        [Column("IsSigned")]
        public bool? IsSigned { get; private set; }


        [Column("ID_ComForm"), ForeignKey("GatheredSubTeam")]
        public short GatheredSubTeamId { get; private set; }
        [Column("ID_ComManager"), ForeignKey("GatheredTeamManager")]
        public short? GatheredTeamManagerId { get; private set; }
        [ForeignKey("DocumentPackage")]
        public int DocumentPackageId { get; private set; }

        public DocumentPackage DocumentPackage { get; private set; }
        public GatheredTeamManager GatheredTeamManager { get; private set; }
        public GatheredSubTeam GatheredSubTeam { get; private set; }
        private TeamDispatching() { }
        public TeamDispatching(short id, short externalTeamId, short gatheredSubTeamId, short gatheredTeamManagerId, DateTime outDate, byte foodCount, bool isDefenceMinistry, int orderNumber, int statementNumber)
        {
            Id = id;
            DocumentPackage = new DocumentPackage(externalTeamId, isDefenceMinistry, statementNumber);
            GatheredSubTeamId = gatheredSubTeamId;
            GatheredTeamManagerId = gatheredTeamManagerId;
            OutDate = outDate;
            FoodCount = foodCount;
            OrderNumber = orderNumber;
        }
        public void Edit(DateTime date, int orderNumber, byte foodCount, short gatheredTeamManagerId)
        {
            OutDate = date;
            OrderNumber = orderNumber;
            FoodCount = foodCount;
            GatheredTeamManagerId = gatheredTeamManagerId;
        }
        public void SetIsPrinted(bool state)
        {
            IsPrinted = state;
        }

        public void SetIsDeliveried(bool state)
        {
            IsDeliveried = state;
        }
        public void SetIsOut(bool state)
        {
            IsOut = state;
        }
        public void SetIsSigned(bool state)
        {
            IsSigned = state;
        }
    }
}
