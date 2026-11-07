using System.ComponentModel.DataAnnotations;
using IPriziv.Server.Data.Enumerations;
using Newtonsoft.Json;

namespace IPriziv.Server.DTOs.Income
{
    public class IncomeRecruit
    {
        public string UniqueKey { get; set; }
        [JsonProperty("militaryOffice.id")]
        public byte? MilitaryOfficeId { get; set; }
        [JsonProperty("cityMunicipality.id")]
        public short? CityMunicipalityId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public DateTime? expectedDate { get; set; }
        [Required]
        public DateTime? BirthDate { get; set; }
        public short? AlphabetNumber { get; set; }
        public bool IsConvict { get; set; }
        public bool hasZIC { get; set; }
        public bool isBandit { get; set; }
        public string SNILS { get; set; }
        public string Address { get; set; }
        [JsonProperty("admiss.id")]
        public byte? AdmissId { get; set; }
        [JsonProperty("education.level.id")]
        public byte? EducationLevelId { get; set; }
        [JsonProperty("education.speciality.id")]
        public string EductaionSpecialityId { get; set; }
        [JsonProperty("education.militaryDebtSpeciality.id")]
        public byte? MilitaryDebtSpecialityId { get; set; }
        [JsonProperty("family.childCount")]
        public byte ChildCount { get; set; }
        [JsonProperty("family.statusId")]
        public byte? FamilyStatusId { get; set; }
        [JsonProperty("family.situation")]
        public FamilySituation? FamilySituation { get; set; }
        [JsonProperty("medical.healthCategory.id")]
        public byte? HealthCategoryId { get; set; }
        [JsonProperty("medical.weight")]
        public byte? Weight { get; set; }
        [JsonProperty("medical.height")]
        public byte? Height { get; set; }
        [JsonProperty("medical.healthParagraph_0")]
        public string HealthParagraph_0 { get; set; }
        [JsonProperty("medical.healthParagraph_1")]
        public string HealthParagraph_1 { get; set; }
        [JsonProperty("medical.doctorId_0")]
        public short? DoctorId_0 { get; set; }
        [JsonProperty("medical.doctorId_1")]
        public short? DoctorId_1 { get; set; }
        [JsonProperty("passport.number")]
        public string PasportNumber { get; set; }
        [JsonProperty("passport.series")]
        public string PasportSeries { get; set; }
        [JsonProperty("passport.source")]
        public string PasportSource { get; set; }
        [JsonProperty("passport.creationDate")]
        public DateTime? PasportCreationDate { get; set; }

        [JsonProperty("passport.code")]
        public string PassportCode { get; set; }
        [JsonProperty("passport.bornCity")]
        public string BornCity { get; set; }
        [JsonProperty("militaryDocument.number")]
        public string MilitaryDocumentNumber { get; set; }
        [JsonProperty("militaryDocument.series")]
        public string MilitaryDocumentSeries { get; set; }
        [JsonProperty("militaryDocument.creationDate")]
        public DateTime? MilitaryDocumentCreationDate { get; set; }
        [JsonProperty("militaryDocument.militaryBadgeNumber")]
        public string MilitaryBadgeNumber { get; set; }
        [JsonProperty("militaryDocument.comissionDecisionDate")]
        public DateTime? ComissionDecisionDate { get; set; }
    }
}
