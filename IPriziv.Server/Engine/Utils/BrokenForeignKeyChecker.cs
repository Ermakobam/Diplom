using IPriziv.Server.Data;
using IPriziv.Server.Engine.Providers;
using IPriziv.Server.Engine.Utils;
using Serilog;

namespace Athena.Service.Analytic.Engine.Utils
{
    public class BrokenForeignKeyChecker
    {
        private readonly DatabasesProvider _databasesProvider;
        private readonly IServiceProvider _serviceProvider;
        public BrokenForeignKeyChecker(DatabasesProvider databasesProvider, IServiceProvider serviceProvider)
        {
            _databasesProvider = databasesProvider;
            _serviceProvider = serviceProvider;
        }
        public async Task CheckAsync(params Func<AppDbContext, Task<int>>[] functions)
        {
            var databaseNames = await _databasesProvider.GetListAsync();

            foreach (var database in databaseNames)
            {
                using var scope = _serviceProvider.CreateScope();
                var databaseParams = scope.ServiceProvider.GetRequiredService<AppDbContextParameters>();
                databaseParams.DataBase = database;
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var results = new List<int>();
                foreach (var func in functions)
                    try
                    {
                        results.Add(await func(context));
                    }
                    catch
                    {
                        results.Add(0);
                    }
                var firstResult = results.FirstOrDefault();
                var result = results.All(x => firstResult == x);
                if (!result)
                    continue;
            }
        }
    }
}
