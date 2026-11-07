using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace IPriziv.Server.Engine.HealthChecks
{
    internal class MigrationHealthCheck : IHealthCheck
    {
        private readonly Migrator _migrator;

        public MigrationHealthCheck(Migrator migrator)
        {
            _migrator = migrator;
        }
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            HealthCheckResult result;
            var unavailableDatabases = _migrator.DatabaseMigrations.Where(x => !x.Value).ToList();
            if (unavailableDatabases.Count == 0)
                result = HealthCheckResult.Healthy();
            else
                result = HealthCheckResult.Degraded($"Ряд баз данных недоступен ({unavailableDatabases.Select(x => x.Key)})");
            return Task.FromResult(result);
        }
    }
}
