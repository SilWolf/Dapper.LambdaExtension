using System.Data;
using AZ.Dapper.LambdaExtension.Adapter;

namespace AZ.Dapper.LambdaExtension.Extentions
{
    public static class DbConnectionExt
    {
        public static SqlAdapter GetAdapter(this IDbConnection dbconn)
        {
            var typeName = dbconn.GetType().Name;
            if (typeName.Contains("MySqlConnection"))
            {
                return SqlAdapter.MySql;
            }

            if (typeName.Contains("SqlConnection"))
            {
                return SqlAdapter.SqlServer2005;
            }
            if (typeName.Contains("SQLiteConnection"))
            {
                return SqlAdapter.Sqlite3;
            }
            if (typeName.Contains("SqliteConnection"))
            {
                return SqlAdapter.Sqlite3;
            }
            if (typeName.Contains("OracleConnection"))
            {
                return SqlAdapter.Oracle;
            }

            if (typeName.Contains("NpgsqlConnection"))
            {
                return SqlAdapter.Postgres;
            }
            if (typeName.Contains("SAConnection"))
            {
                return SqlAdapter.SqlAnyWhere;
            }

            return SqlAdapter.SqlServer2005;
        }
    }
}
