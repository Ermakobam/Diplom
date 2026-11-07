namespace IPriziv.Server.Data.Models
{
    public class ThingUserData
    {
        public List<ThingRecruitData> Recruits { get; set; }
        public int DocType { get; set; }
    }

    public class ThingRecruitData
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string MilitaryOfficeName { get; set; }
        public string BirthDate { get; set; }
    }
}
