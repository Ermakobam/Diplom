using System.Data.Common;
using System.Reflection;
using DbfDataReader;
using Microsoft.EntityFrameworkCore;

namespace IPriziv.Server.Engine.Utils
{
    public static class Extensions
    {
        public static IServiceCollection AddInheritedAsScoped<T>(this IServiceCollection services) where T : class
        {
            Assembly.GetAssembly(typeof(T)).GetTypes()
                            .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(T))).ToList()
                            .ForEach(x => services.AddScoped(x));
            return services;
        }
        public static T GetNullableValue<T>(this DbfRecord record, int ordinal)
        {
            return record.GetValue(ordinal) == null ? default : record.GetValue<T>(ordinal);
        }
        public static (int year, bool isSpring)? GetDatabaseRecruitingInfo(this DbContext context)
        {
            var connectionData = context.Database.GetDbConnection().Database;
            var isCorrect = int.TryParse(Constants.DatabaseYearRegex.Match(connectionData).Value, out var year);
            if (!isCorrect)
                return null;
            return (2000 + year, connectionData.StartsWith("v_"));
        }
    }
}
