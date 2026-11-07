using IPriziv.Server.Data;
using IPriziv.Server.Engine.Providers;
using IPriziv.Server.Engine.Utils;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Serilog;

namespace IPriziv.Server.Engine
{
    public class Migrator : BackgroundService
    {
        private readonly DatabasesProvider _databasesProvider;
        private readonly bool _isDevelopment;
        public Dictionary<string, bool> DatabaseMigrations { get; private set; }
        public Migrator(DatabasesProvider databasesProvider, IWebHostEnvironment environment)
        {
            _databasesProvider = databasesProvider;
            _isDevelopment = environment.IsDevelopment();
            DatabaseMigrations = new Dictionary<string, bool>();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                var databaseNames = await _databasesProvider.GetListAsync();
                databaseNames.ForEach(database => DatabaseMigrations[database] = true);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting database list in Migrator");
            }

            try
            {
                await _databasesProvider.TryMigrateServiceAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error migrating service database");
            }

            var databasesToMigrate = DatabaseMigrations.Keys.ToList();
            foreach (var databaseName in databasesToMigrate)
            {
                try
                {
                    await _databasesProvider.TryMigrateAsync(databaseName, stoppingToken);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, $"Error during migration of {databaseName}");
                }
                DatabaseMigrations[databaseName] = true;
                if (stoppingToken.IsCancellationRequested)
                    return;
            }
        }
    }
}
