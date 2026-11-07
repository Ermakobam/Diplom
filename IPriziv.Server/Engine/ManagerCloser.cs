
using IPriziv.Server.Data;
using IPriziv.Server.Engine.Providers;
using IPriziv.Server.Engine.Utils;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace IPriziv.Server.Engine
{
    public class ManagerCloser : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        public Dictionary<string, bool> DatabaseMigrations { get; private set; }
        public ManagerCloser(IServiceProvider serviceProvider, IWebHostEnvironment environment)
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
                        TryCloseManager(databaseName, stoppingToken);
                    break;

                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error during database list getting...");
                }
                await Task.Delay((int)new TimeSpan(12, 0, 0).TotalMilliseconds, stoppingToken);
            }
        }

        private async void TryCloseManager(string databaseName, CancellationToken stoppingToken)
        {
            try
            {
                using var databaseScope = _serviceProvider.CreateScope();
                var databaseParams = databaseScope.ServiceProvider.GetRequiredService<AppDbContextParameters>();
                databaseParams.DataBase = databaseName;
                var context = databaseScope.ServiceProvider.GetRequiredService<AppDbContext>();

                var managers = await context.GatheredTeamManagers
                        .Where(x => x.InDate <= DateTime.Today.AddDays(-7) && x.OutDate == null || x.InDate == null).ToListAsync();
                foreach (var manager in managers)
                {
                    if (manager.InDate != null)
                        manager.OutDate = manager.InDate.Value.AddDays(4);
                    else
                    {
                        manager.OutDate = DateTime.MinValue.AddYears(1900);
                        manager.InDate = DateTime.MinValue.AddYears(1900);
                    }
                }
                context.GatheredTeamManagers.UpdateRange(managers);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error during manager closing");
            }
        }
    }
}
