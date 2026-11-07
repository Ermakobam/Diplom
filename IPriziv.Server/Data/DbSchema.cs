using Microsoft.EntityFrameworkCore;

namespace IPriziv.Server.Data
{
    public static class DbSchema
    {
        public static bool HasBarcode { get; private set; }

        public static void Initialize(DbContext context)
        {
            var connection = context.Database.GetDbConnection();
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT COL_LENGTH('T_People', 'barcode')";
            HasBarcode = command.ExecuteScalar() != DBNull.Value;
        }
    }
}
