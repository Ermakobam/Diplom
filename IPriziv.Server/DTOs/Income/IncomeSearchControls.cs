using System.Collections;
using DevExtreme.AspNet.Data;

namespace IPriziv.Server.DTOs.Income
{
    public class IncomeSearchControls
    {
        public DataSourceLoadOptions LoadOptions { get; set; }
        public IncomeSearchParams Params { get; set; }
    }
}
