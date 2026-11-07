using IPriziv.Server.Engine.Providers;
using IPriziv.Server.Engine.Utils;
using Serilog;

namespace IPriziv.Server.Engine
{
    public class Backuper : BackgroundService
    {
        private readonly DatabasesProvider _databasesProvider;
        private readonly TimeSpan _timeToBackup = new(8, 00, 00);
        public Backuper(DatabasesProvider databasesProvider)
        {
            _databasesProvider = databasesProvider;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (true)
            {
                var (isFullBackuped, isDifferBackuped) = _databasesProvider.CheckBuckupExistance();
                var now = DateTime.Now;
                var time = now.TimeOfDay;
                try
                {
                    if (!isFullBackuped)
                        await BackupDatabasesAsync(now.Date, isFull: true, stoppingToken);
                    if (time >= _timeToBackup && !isDifferBackuped)
                        await BackupDatabasesAsync(now.Date, isFull: false, stoppingToken);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error during backup");
                }
                await DelayTillNextStart(stoppingToken);
            }
        }
        private async Task BackupDatabasesAsync(DateTime folderDate, bool isFull, CancellationToken stoppingToken)
        {
            var databaseNames = await _databasesProvider.GetListAsync();
            foreach (var databse in databaseNames)
                await _databasesProvider.BackupByDateAsync(folderDate, databse, isFull, stoppingToken);
            await _databasesProvider.BackupByDateAsync(folderDate, Constants.WebPrizivDatabaseName, isFull, stoppingToken);
        }
        private async Task DelayTillNextStart(CancellationToken stoppingToken)
        {
            var currentTime = DateTime.Now.TimeOfDay;
            if (currentTime > _timeToBackup)
                await Task.Delay(new TimeSpan(23, 59, 59) - currentTime + _timeToBackup, stoppingToken);
            else
                await Task.Delay(_timeToBackup - currentTime, stoppingToken);
        }
    }
}
