using System.Data;
using FastMember;

namespace IPriziv.Server.Engine.Utils
{
    public static class DataSetUtils
    {
        public static DataSet ToDataSet<T>(this IEnumerable<T> recruits, string tableName = "list")
        {
            var recruitsDataSet = new DataSet();
            recruitsDataSet.Tables.Add(recruits.ToDataTable(tableName));
            return recruitsDataSet;
        }
        public static DataTable ToDataTable<T>(this IEnumerable<T> recruits, string tableName = "list")
        {
            var table = new DataTable
            {
                TableName = tableName
            };
            using var reader = ObjectReader.Create(recruits);
            table.Load(reader);
            return table;
        }
    }
}
