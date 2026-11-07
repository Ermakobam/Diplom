namespace IPriziv.Server.DTOs.Outcome
{
    internal class OutcomeRecruitGroupDispatching
    {
        public string Assignment { get; set; }
        public string GeneralStaffTeam { get; set; }
        public string DispatchedAt { get; set; }
        public string MilitaryDebtSpecialityName { get; set; }
        public string HealthCategoryName { get; set; }
        public string AdmissName { get; set; }
        public short InternalTeamId { get; set; }
        public bool IsEducational { get; set; }
        public byte? MilitaryOfficeId { get; set; }
        public string MilitaryTypeName { get; set; }
        public string Note { get; set; }
        public int Count { get; set; }

        public OutcomeRecruitGroupDispatching(string assignment, string generalStaffTeam, string dispatchedAt, string militaryDebtSpecialityName, string healthCategoryName, string admissName, short internalTeamId, bool isEducational, byte? militaryOfficeId, string militaryTypeName, int count)
        {
            Assignment = assignment;
            GeneralStaffTeam = generalStaffTeam;
            DispatchedAt = dispatchedAt;
            MilitaryDebtSpecialityName = militaryDebtSpecialityName;
            HealthCategoryName = healthCategoryName;
            AdmissName = admissName;
            InternalTeamId = internalTeamId;
            IsEducational = isEducational;
            MilitaryOfficeId = militaryOfficeId;
            MilitaryTypeName = militaryTypeName;
            Count = count;
        }
    }
}
