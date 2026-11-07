using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using DocumentFormat.OpenXml.Office2016.Excel;
using IPriziv.Server.Data.Constants;
using IPriziv.Server.Data.Enumerations;
using IPriziv.Server.Data.Models.ConstantDictionaries;
using IPriziv.Server.Data.Models.Dictionaries;
using IPriziv.Server.DTOs.Enumerations;
using IPriziv.Server.Engine.Appearance;
using IPriziv.Server.Engine.Recruits;
//using IPriziv.Server.Migrations.Conscription;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using Microsoft.VisualBasic;

namespace IPriziv.Server.Data.Models
{
    [Table("T_People")]
    public class Recruit
    {
        [Key, Column("ID_People")]
        public int Id { get; private set; }

        [Column("FirstN"), MaxLength(50)]
        public string FirstName { get; private set; }
        [Column("LastN"), MaxLength(50)]
        public string LastName { get; private set; }
        [Column("MiddleN"), MaxLength(50)]
        public string MiddleName { get; private set; }
        public DateTime? BirthDate { get; private set; }
        [Column("isConvict")]
        public bool IsConvict { get; private set; }
        [Column("hasZIC")]
        public bool HasZIC { get; private set; }
        [Column("hasDocsModule")]
        public bool HasDocsModule { get; private set; }
        [Column("isBandit")]
        public bool IsBandit { get; private set; }
        [Column("SNILS")]
        public string SNILS { get; private set; }
        public byte Height { get; private set; }
        public byte Weight { get; private set; }
        [MaxLength(250)]
        public string Address { get; private set; }
        [Column("Uniq"), MaxLength(16)]
        public string UniqueKey { get; private set; }
        [Column("ChildNum")]
        public byte ChildCount { get; private set; }
        [Column("v_ser"), MaxLength(12)]
        public string MilitaryDocumentSeries { get; private set; }
        [Column("v_num"), MaxLength(12)]
        public string MilitaryDocumentNumber { get; private set; }
        [Column("v_date")]
        public DateTime? MilitaryDocumentCreationDate { get; private set; }
        public DateTime? DocsOnModuleDate { get; private set; }
        public DateTime? ComissionDecisionDate { get; private set; }
        [Column("NOM_LI"), MaxLength(50)]
        public string MilitaryBadgeNumber { get; private set; }
        [Column("born"), MaxLength(255)]
        public string BornCity { get; private set; }
        [Column("pass_ser"), MaxLength(8)]
        public string PassportSeries { get; private set; }
        [Column("pass_num"), MaxLength(10)]
        public string PassportNumber { get; private set; }
        [Column("pass_who"), MaxLength(250)]
        public string PassportSource { get; private set; }
        [Column("pass_when"), MaxLength(4)]
        public DateTime? PassportCreationDate { get; private set; }
        public string PassportCode { get; private set; }
        [Column("Paragraph1"), MaxLength(8)]
        public string HealthParagraph_0 { get; private set; }
        [Column("Paragraph2"), MaxLength(8)]
        public string HealthParagraph_1 { get; private set; }
        /// <summary>Статьи/пункты заболеваний из плана явки (полное значение).</summary>
        [Column("disease_articles"), MaxLength(250)]
        public string DiseaseArticles { get; private set; }
        [Column("DateArriveToSPGM")]
        public DateTime? ArrivedAt { get; private set; }
        [Column("FamSituat_ID")]
        public FamilySituation? FamilySituation { get; private set; }
        public RecruitState State { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        public DateTime CreatedAt { get; private set; }
        [Column("expectedDate")]
        public DateTime? ExpectedDate { get; private set; }
        [Column("barcode")]
        public string? Barcode { get; set; }
        [Column("health_status")]
        public int HealthStatus { get; set; }
        /// <summary>0 — на беседе, 1 — закреплён за командой.</summary>
        [Column("team_pin_status")]
        public int TeamPinStatus { get; set; }
        /// <summary>НПУ: 6–10, либо не задано.</summary>
        [Column("npu")]
        public byte? Npu { get; private set; }
        [Column("bag_deliveried_at")]
        public DateTime? DeliveryBagDate { get; set; }
        [Column("bag_status")]
        public int? BagStatus { get; set; }


        [Column("ID_Municip"), ForeignKey("CityMunicipality")]
        public short? CityMunicipalityId { get; private set; }
        [Column("ID_OVK"), ForeignKey("MilitaryOffice")]
        public byte? MilitaryOfficeId { get; private set; }
        [Column("MilitaryDocumentIssuedId"), ForeignKey("MilitaryDocumentIssued")]
        public byte? MilitaryDocumentIssuedId { get; private set; }
        [Column("ID_Health"), ForeignKey("HealthCategory")]
        public byte? HealthCategoryId { get; private set; }
        [Column("ID_Doctor1"), ForeignKey("Doctor_0")]
        public short? DoctorId_0 { get; private set; }
        [Column("ID_Doctor2"), ForeignKey("Doctor_1")]
        public short? DoctorId_1 { get; private set; }
        [Column("ID_Family"), ForeignKey("FamilyStatus")]
        public byte? FamilyStatusId { get; private set; }
        [Column("ID_Admiss"), ForeignKey("Admiss")]
        public byte? AdmissId { get; private set; }
        [Column("ID_Education"), ForeignKey("EducationLevel")]
        public byte? EducationLevelId { get; private set; }
        [Column("ID_ROSTO"), ForeignKey("MilitaryDebtSpeciality")]
        public byte? MilitaryDebtSpecialityId { get; private set; }
        [Column("EducationSpecID"), ForeignKey("EducationSpeciality")]
        public string EductaionSpecialityId { get; private set; }
        public string TeamAssignment { get; private set; }
        [ForeignKey("TeamMiliaryDebtSpeciality")]
        public byte? TeamMilitaryDebtSpecialityId { get; private set; }
        [ForeignKey("GatheredSubTeam")]
        public short? GatheredSubTeamId { get; private set; }

        public GatheredSubTeam GatheredSubTeam { get; private set; }
        public MilitaryDebtSpeciality TeamMilitaryDebtSpeciality { get; private set; }
        public CityMunicipality CityMunicipality { get; private set; }
        public MilitaryOffice MilitaryOffice { get; private set; }
        public MilitaryOffice MilitaryDocumentIssued { get; private set; }
        public HealthCategory HealthCategory { get; private set; }
        public EducationSpeciality EducationSpeciality { get; private set; }
        public MilitaryDebtSpeciality MilitaryDebtSpeciality { get; private set; }
        public EducationLevel EducationLevel { get; private set; }
        public Admiss Admiss { get; private set; }
        public FamilyStatus FamilyStatus { get; private set; }
        public Doctor Doctor_0 { get; private set; }
        public Doctor Doctor_1 { get; private set; }
        public Biometric Biometric { get; private set; }

        public List<ControlPerson> ControlPersons
        {
            get => _loader.Load(this, ref _controlPersons);
            private set => _controlPersons = value;
        }
        public List<RecruitStatusLog> RecruitStatusLogs
        {
            get => _loader.Load(this, ref _recruitStatusLogs);
            private set => _recruitStatusLogs = value;
        }
        public List<RecruitLog> RecruitLogs
        {
            get => _loader.Load(this, ref _recruitLogs);
            private set => _recruitLogs = value;
        }
        public List<TeamLog> TeamLogs
        {
            get => _loader.Load(this, ref _teamLogs);
            private set => _teamLogs = value;
        }

        private readonly ILazyLoader _loader;

        private List<ControlPerson> _controlPersons;
        private List<RecruitStatusLog> _recruitStatusLogs;
        private List<RecruitLog> _recruitLogs;
        private List<TeamLog> _teamLogs;

        [SuppressMessage("CodeQuality", "IDE0051:Удалите неиспользуемые закрытые члены", Justification = "Необходимо для работы отложенной загрузки")]
        private Recruit(ILazyLoader loader)
        {
            _loader = loader;
        }
        public Recruit(string firstName, string lastName, string middleName, DateTime? birthDate, bool isConvict, bool hasZIC, byte height, byte weight, string address, string uniqueKey, byte childCount, string militaryDocumentSeries, string militaryDocumentNumber, DateTime? militaryDocumentCreationDate, string militaryBadgeNumber, DateTime? commissionDecisionDate, string bornCity, string passportSeries, string passportNumber, string passportSource, DateTime? passportCreationDate, string passportCode, string healthParagraph_0, string healthParagraph_1, FamilySituation? familySituation, short? cityMunicipalityId, byte? militaryOfficeId, byte? healthCategoryId, short? doctorId_0, short? doctorId_1, byte? familyStatusId, byte? admissId, byte? educationLevelId, byte? militaryDebtSpecialityId, string eductaionSpecialityId, DateTime? expectedDate, bool hasDocsModule = false, bool isBandit = false, string snils = "", string barcode = "")
        {
            FirstName = firstName;
            LastName = lastName;
            MiddleName = middleName;
            BirthDate = birthDate;
            IsConvict = isConvict;
            HasZIC = hasZIC;
            Height = height;
            Weight = weight;
            Address = address;
            UniqueKey = uniqueKey;
            ChildCount = childCount;
            MilitaryDocumentSeries = militaryDocumentSeries;
            MilitaryDocumentNumber = militaryDocumentNumber;
            MilitaryBadgeNumber = militaryBadgeNumber;
            ComissionDecisionDate = commissionDecisionDate;
            BornCity = bornCity;
            PassportSeries = passportSeries;
            PassportNumber = passportNumber;
            PassportSource = passportSource;
            PassportCode = passportCode;
            PassportCreationDate = passportCreationDate;
            HealthParagraph_0 = healthParagraph_0;
            HealthParagraph_1 = healthParagraph_1;
            FamilySituation = familySituation;
            CityMunicipalityId = cityMunicipalityId;
            MilitaryOfficeId = militaryOfficeId;
            HealthCategoryId = healthCategoryId;
            DoctorId_0 = doctorId_0;
            DoctorId_1 = doctorId_1;
            FamilyStatusId = familyStatusId;
            AdmissId = admissId;
            EducationLevelId = educationLevelId;
            MilitaryDebtSpecialityId = militaryDebtSpecialityId;
            EductaionSpecialityId = eductaionSpecialityId;
            UpdatedAt = DateTime.Now;
            ExpectedDate = expectedDate;
            HasDocsModule = hasDocsModule;
            IsBandit = isBandit;
            SNILS = snils;
            Barcode = barcode;
            HealthStatus = 0;
            TeamPinStatus = 0;

            State = RecruitState.Undefined;
            CreatedAt = DateTime.Now;
            ArrivedAt = null;
            TeamAssignment = null;
            TeamMilitaryDebtSpecialityId = null;
            GatheredSubTeamId = null;
            RecruitStatusLogs = new();
            RecruitLogs = new();
            ControlPersons = new();
            TeamLogs = new();
        }

        public void EditRosto(byte rosto)
        {
            MilitaryDebtSpecialityId = rosto;
            UpdatedAt = DateTime.Now;
        }

        /// <summary>
        /// Обновляет медданные и ТСП из плана явки: не очищает существующие значения пустыми из файла.
        /// </summary>
        public void UpdateFromAppearancePlan(
            byte? healthCategoryId,
            string diseaseArticles,
            FamilySituation? familySituation = null)
        {
            var changed = false;

            if (healthCategoryId.HasValue && HealthCategoryId != healthCategoryId)
            {
                HealthCategoryId = healthCategoryId;
                changed = true;
            }

            if (!string.IsNullOrWhiteSpace(diseaseArticles))
            {
                var articles = diseaseArticles.Trim();
                if (!string.Equals(DiseaseArticles, articles, StringComparison.Ordinal))
                {
                    DiseaseArticles = articles.Length > 250 ? articles[..250] : articles;
                    changed = true;
                }

                // Синхронизируем короткий Paragraph1 для существующей медкарточки (лимит 8 символов).
                var shortArticle = articles.Length > 8 ? articles[..8] : articles;
                if (!string.Equals(HealthParagraph_0, shortArticle, StringComparison.Ordinal))
                {
                    HealthParagraph_0 = shortArticle;
                    changed = true;
                }
            }

            var mergedFamily = AppearanceFamilySituationRules.Merge(FamilySituation, familySituation);
            if (mergedFamily.HasValue && FamilySituation != mergedFamily)
            {
                FamilySituation = mergedFamily;
                changed = true;
            }

            if (changed)
                UpdatedAt = DateTime.Now;
        }

        public void Edit(string firstName, string lastName, string middleName, DateTime? birthDate, bool isConvict, bool hasZIC, byte height, byte weight, string address, string uniqueKey, byte childCount, string militaryDocumentSeries, string militaryDocumentNumber, DateTime? militaryDocumentCreationDate, DateTime? comissionDecisionDate, byte? militaryDocumentIssuedId, string militaryBadgeNumber, string bornCity, string passportSeries, string passportNumber, string passportSource, DateTime? passportCreationDate, string passsportCode, string healthParagraph_0, string healthParagraph_1, FamilySituation? familySituation, short? cityMunicipalityId, byte? militaryOfficeId, byte? healthCategoryId, short? doctorId_0, short? doctorId_1, byte? familyStatusId, byte? admissId, byte? educationLevelId, byte? militaryDebtSpecialityId, string eductaionSpecialityId, DateTime? expectedDate, bool hasDocsModule = false, bool isBandit = false, string snils = "")
        {
            FirstName = firstName;
            LastName = lastName;
            MiddleName = middleName;
            BirthDate = birthDate;
            IsConvict = isConvict;
            HasZIC = hasZIC;
            Height = height;
            Weight = weight;
            Address = address;
            UniqueKey = uniqueKey;
            ChildCount = childCount;
            MilitaryDocumentSeries = militaryDocumentSeries;
            MilitaryDocumentNumber = militaryDocumentNumber;
            MilitaryDocumentCreationDate = militaryDocumentCreationDate;
            ComissionDecisionDate = comissionDecisionDate;
            MilitaryDocumentIssuedId = militaryDocumentIssuedId;
            MilitaryBadgeNumber = militaryBadgeNumber;
            BornCity = bornCity;
            PassportSeries = passportSeries;
            PassportNumber = passportNumber;
            PassportSource = passportSource;
            PassportCreationDate = passportCreationDate;
            PassportCode = passsportCode;
            HealthParagraph_0 = healthParagraph_0;
            HealthParagraph_1 = healthParagraph_1;
            FamilySituation = familySituation;
            CityMunicipalityId = cityMunicipalityId;
            MilitaryOfficeId = militaryOfficeId;
            HealthCategoryId = healthCategoryId;
            DoctorId_0 = doctorId_0;
            DoctorId_1 = doctorId_1;
            FamilyStatusId = familyStatusId;
            AdmissId = admissId;
            EducationLevelId = educationLevelId;
            MilitaryDebtSpecialityId = militaryDebtSpecialityId;
            EductaionSpecialityId = eductaionSpecialityId;
            ExpectedDate = expectedDate;
            DocsOnModuleDate = hasDocsModule ? DateTime.Now : DocsOnModuleDate;
            HasDocsModule = hasDocsModule;
            IsBandit = isBandit;
            SNILS = snils;

            UpdatedAt = DateTime.Now;
        }
        private void SetState(RecruitState state, string operatorIp, object obj = null, bool isAccidentCorrection = false, short? gatheredSubTeamId = null)
        {
            State = state;

            if (state == RecruitState.DeliverDeclared)
                ArrivedAt = null;

            UpdatedAt = DateTime.Now;
            RecruitLogs.Add(new RecruitLog(state, operatorIp, obj, isAccidentCorrection, gatheredSubTeamId));
        }
        public void DeclareDeliver(string operatorIp)
        {
            SetState(RecruitState.DeliverDeclared, operatorIp);

            RecruitStatusLogs.Clear();
            RecruitStatusLogs.Add(new RecruitStatusLog(RecruitStates.DeliverDeclared, operatorIp));
        }
        public void Deliver(string operatorIp)
        {
            if (State != RecruitState.DeliverDeclared)
                throw new ArgumentException("Призывник находится не в статусе заявки");

            SetState(RecruitState.Delivered, operatorIp);
            ArrivedAt = DateTime.Now;

            RecruitStatusLogs.Add(new RecruitStatusLog(RecruitStates.Delivered, operatorIp));
        }
        public void ChangeHealthStatus(int status)
        {
            //if (State != RecruitState.DeliverDeclared) throw new ArgumentException("Призывник находится не в статусе заявки");
            // if (status != 0 || status == 1 || status != 2) throw new ArgumentException("Выбран неправильный статус здоровья призывника");
            HealthStatus = status;
        }

        public void ChangeTeamPinStatus(int status)
        {
            TeamPinStatus = status;
        }

        public void SetNpu(byte? npu)
        {
            if (!RecruitNpuRules.IsValid(npu))
                throw new ArgumentOutOfRangeException(nameof(npu), RecruitNpuRules.InvalidMessage);

            Npu = npu;
            UpdatedAt = DateTime.Now;
        }

        /// <summary>
        /// Установка ТСП с отбора: false → Нет, true → Да (конкретный тип не затирается).
        /// </summary>
        public void SetTsp(bool hasTsp)
        {
            if (!hasTsp)
            {
                if (FamilySituation != Enumerations.FamilySituation.Normal)
                {
                    FamilySituation = Enumerations.FamilySituation.Normal;
                    UpdatedAt = DateTime.Now;
                }
                return;
            }

            if (FamilySituation is null or Enumerations.FamilySituation.Normal)
            {
                FamilySituation = Enumerations.FamilySituation.Present;
                UpdatedAt = DateTime.Now;
            }
        }

        public void ChangeBagStatus(DateTime? state)
        {
            //if (State != RecruitState.DeliverDeclared) throw new ArgumentException("Призывник находится не в статусе заявки");
            // if (status != 0 || status == 1 || status != 2) throw new ArgumentException("Выбран неправильный статус здоровья призывника");
            DeliveryBagDate = state;
        }

        public void PassSelection(IEnumerable<ControlPerson> persons, string operatorIp)
        {
            if (State != RecruitState.Delivered)
                throw new ArgumentException("Призывник находится не в доставленных");

            SetState(RecruitState.SelectionPassed, operatorIp);

            ControlPersons.AddRange(persons);

            RecruitStatusLogs.Add(new RecruitStatusLog(persons.Any() ? RecruitStates.Controled : RecruitStates.UnControled, operatorIp));
        }
        public void UnpassSelection(string operatorIp, bool isAccidentCorrection = true)
        {
            if (State != RecruitState.SelectionPassed)
                throw new ArgumentException("Призывник находится не в свободных");

            SetState(RecruitState.Delivered, operatorIp, isAccidentCorrection);

            ControlPersons.Clear();
            RecruitStatusLogs.RemoveAll(x => x.State == RecruitStates.Controled || x.State == RecruitStates.UnControled);
        }

        public void Complect(short teamId, string operatorIp, short? gatheredSubTeamId)
        {
            if (State != RecruitState.SelectionPassed)
                throw new ArgumentException("Призывник находится не в свободных");

            SetState(RecruitState.Complected, operatorIp, gatheredSubTeamId: gatheredSubTeamId);
            GatheredSubTeamId = teamId;
            // После переноса — «На беседе»; медстатус (кровь) не сбрасываем.
            TeamPinStatus = 0;
            TeamLogs.Add(new TeamLog(this.Id, operatorIp, gatheredSubTeamId, null));

            RecruitStatusLogs.Add(new RecruitStatusLog(RecruitStates.Complected, operatorIp, gatheredSubTeamId: teamId));
        }
        public void Uncomplect(string operatorIp, bool isAccidentCorrection = true)
        {
            if (State != RecruitState.Complected)
                throw new ArgumentException("Призывник находится не в комплектующейся команде");

            SetState(RecruitState.SelectionPassed, operatorIp, isAccidentCorrection);
            SetPrintParams(assignment: null, militaryDebtSpecialityId: null);
            TeamLogs.Add(new TeamLog(this.Id, operatorIp, null, GatheredSubTeamId));
            GatheredSubTeamId = null;
            TeamPinStatus = 0;

            //this.Edit(FirstName, LastName, MiddleName, BirthDate, IsConvict, HasZIC, Height, Weight, Address, UniqueKey, ChildCount, MilitaryDocumentSeries, MilitaryDocumentNumber, MilitaryDocumentCreationDate, ComissionDecisionDate, MilitaryDocumentIssuedId, MilitaryBadgeNumber, BornCity, PassportSeries, PassportNumber, PassportSource, PassportCreationDate, PassportCode, HealthParagraph_0, HealthParagraph_1, FamilySituation, CityMunicipalityId, MilitaryOfficeId, HealthCategoryId, DoctorId_0, DoctorId_1, FamilyStatusId, AdmissId, EducationLevelId, MilitaryDebtSpecialityId, EductaionSpecialityId, ExpectedDate, false);

            RecruitStatusLogs.RemoveAll(x => x.State == RecruitStates.Complected);
        }
        public void Dispatch(short dispatchingId, string operatorIp, short? gatheredSubTeamId)
        {
            if (State != RecruitState.Complected || GatheredSubTeamId == null)
                throw new ArgumentException("Призывник находится не в команде");

            SetState(RecruitState.Dispatched, operatorIp, gatheredSubTeamId: gatheredSubTeamId);

            RecruitStatusLogs.Add(new RecruitStatusLog(RecruitStates.Sent, operatorIp, gatheredSubTeamId: GatheredSubTeamId, dispatchingId));
        }
        public void Undispatch(string operatorIp, bool isAccidentCorrection = true)
        {
            if (State != RecruitState.Dispatched || GatheredSubTeamId == null)
                throw new ArgumentException("Призывник не отправлен");

            SetState(RecruitState.Complected, operatorIp, isAccidentCorrection);

            RecruitStatusLogs.RemoveAll(x => x.State == RecruitStates.Sent);
        }
        public void ForcedDeliverDeclare(string reason, bool isAccidentCorrection, string operatorIp)
        {
            if (State == RecruitState.Dispatched)
                Undispatch(operatorIp, isAccidentCorrection);
            if (State == RecruitState.Complected)
                Uncomplect(operatorIp, isAccidentCorrection);
            if (State == RecruitState.SelectionPassed)
                UnpassSelection(operatorIp, isAccidentCorrection);

            SetState(RecruitState.DeliverDeclared, operatorIp, new
            {
                Reason = reason,
                Type = "Unofficial"
            }, isAccidentCorrection);

            RecruitStatusLogs.RemoveAll(x => x.State != RecruitStates.DeliverDeclared);
        }
        public void Return(string doctorName, string operatorIp)
        {
            if (State == RecruitState.Dispatched)
                Undispatch(operatorIp);
            if (State == RecruitState.Complected)
                Uncomplect(operatorIp);

            SetState(RecruitState.Returned, operatorIp, new { DoctorName = doctorName });

            var allowedStatuses = new[]
            {
                RecruitStates.DeliverDeclared,
                RecruitStates.Delivered,
                RecruitStates.Controled,
                RecruitStates.UnControled
            };
            RecruitStatusLogs.RemoveAll(x => !allowedStatuses.Contains(x.State));
            RecruitStatusLogs.Add(new RecruitStatusLog(RecruitStates.ReturnedByMMC, operatorIp));
        }
        public void SetPrintParams(string assignment, byte? militaryDebtSpecialityId)
        {
            if ((!string.IsNullOrEmpty(assignment) || militaryDebtSpecialityId != null) && GatheredSubTeamId == null)
                throw new ArgumentException("Для указания параметров печати, призывник должен быть в команде");

            TeamAssignment = assignment;
            TeamMilitaryDebtSpecialityId = militaryDebtSpecialityId;
        }
    }
}
