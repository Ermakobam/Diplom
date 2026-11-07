using IPriziv.Server.Data;
using IPriziv.Server.Engine.Providers;
using IPriziv.Server.Engine.Utils;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace IPriziv.Server.Engine
{
    public class Initializator : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        public Initializator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (true)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var databases = scope.ServiceProvider.GetRequiredService<DatabasesProvider>();
                    var databaseNames = await databases.GetListAsync();

                    foreach (var databaseName in databaseNames)
                        TryInitialize(databaseName);
                    break;
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error during database list getting...");
                    await Task.Delay(5000, stoppingToken);
                }
            }
        }

        private bool TryInitialize(string databaseName)
        {
            try
            {
                using var databaseScope = _serviceProvider.CreateScope();
                var databaseParams = databaseScope.ServiceProvider.GetRequiredService<AppDbContextParameters>();
                databaseParams.DataBase = databaseName;
                var context = databaseScope.ServiceProvider.GetRequiredService<AppDbContext>();
                var _ = context.Model;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error during initialization");
                return false;
            }
            return true;

        }
    }
}
