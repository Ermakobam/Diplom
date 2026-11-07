namespace IPriziv.Server.DTOs.Income
{
    public class IncomeSearchParams
    {
        public Guid OperationGuid { get; set; }
        public Dictionary<string, string> Distance { get; set; } = new Dictionary<string, string>();
        public List<FieldFilter> Filters { get; set; } = new List<FieldFilter>();
    }
    public class FieldFilter
    {
        public string DataField { get; set; }
        public string Value { get; set; }
        public string Operation { get; set; }
    }
}
