using IPriziv.Server.Data;
using Microsoft.EntityFrameworkCore;

namespace IPriziv.Server.Engine.Controls
{
    /// <summary>Военкомат по умолчанию для контролей без указанного ОВК.</summary>
    public static class ControlMilitaryOffice
    {
        public const string AbsentName = "Отсутствует";

        public static async Task<byte> GetAbsentIdAsync(AppDbContext context, CancellationToken cancellationToken = default)
        {
            var id = await context.MilitaryOffices
                .AsNoTracking()
                .Where(x => x.Name == AbsentName)
                .Select(x => (byte?)x.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (!id.HasValue)
                throw new InvalidOperationException($"В справочнике военкоматов не найден «{AbsentName}»");

            return id.Value;
        }

        /// <summary>Пустой / нулевой id заменяется на «Отсутствует».</summary>
        public static async Task<byte> ResolveOrAbsentAsync(
            AppDbContext context,
            byte? militaryOfficeId,
            CancellationToken cancellationToken = default)
        {
            if (militaryOfficeId.HasValue && militaryOfficeId.Value != 0)
                return militaryOfficeId.Value;

            return await GetAbsentIdAsync(context, cancellationToken);
        }
    }
}
