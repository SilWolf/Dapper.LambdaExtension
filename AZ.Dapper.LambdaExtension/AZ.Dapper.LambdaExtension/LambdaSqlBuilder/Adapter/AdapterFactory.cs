using System;

namespace Dapper.LambdaExtension.LambdaSqlBuilder.Adapter
{
    public static class AdapterFactory
    {
        public static ISqlAdapter GetAdapterInstance(SqlAdapterType adapter)
        {
            switch (adapter)
            {
                case SqlAdapterType.SqlServer:
                    return new SqlserverAdapter();
                case SqlAdapterType.Sqlite:
                    return new Sqlite3Adapter();
                case SqlAdapterType.Oracle:
                    return new OracleAdapter();
                case SqlAdapterType.MySql:
                    return new MySqlAdapter();
                case SqlAdapterType.Postgres:
                    return new PostgresAdapter();
                case SqlAdapterType.SqlAnyWhere:
                    return new SqlAnyWhereAdapter();
                    case SqlAdapterType.SqlServerCE:
                        return new SqlserverCEAdapter();
                default:
                    throw new ArgumentException("The specified Sql Adapter was not recognized");
            }
        }
    }
}
