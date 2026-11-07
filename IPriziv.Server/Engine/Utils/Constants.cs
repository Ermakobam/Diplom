using System.Text.RegularExpressions;

namespace IPriziv.Server.Engine.Utils
{
    public static class Constants
    {
        public static readonly Regex DatabaseYearRegex = new(@"\d+");
        public const string XlsContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        public const string WebPrizivDatabaseName = "webpriziv";
    }
}
