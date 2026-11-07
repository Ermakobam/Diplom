using System.Data;
using Microsoft.EntityFrameworkCore;

namespace IPriziv.Server.Data.Models
{
    public static class RecruitsExtensions
    {
        private static bool? _hasBarcodeColumn;

        public static IQueryable<Recruit> WhereBarcodeIs(this IQueryable<Recruit> query, string barcode, DbContext context)
        {
            if (_hasBarcodeColumn.HasValue)
            {
                var connection = context.Database.GetDbConnection();
                var state = connection.State;
                if (state != ConnectionState.Open)
                    connection.Open();

                using var command = connection.CreateCommand();
                command.CommandText = "SELECT COL_LENGTH('T_People', 'barcode')";
                _hasBarcodeColumn = command.ExecuteScalar() != DBNull.Value;

                if (state != ConnectionState.Open)
                    connection.Close();
            }

            if (_hasBarcodeColumn.Value)
            {
                return query.Where(x => EF.Property<string>(x, "Barcode") == barcode);
            }

            return query.Where(x => false);
        }
    }
}
