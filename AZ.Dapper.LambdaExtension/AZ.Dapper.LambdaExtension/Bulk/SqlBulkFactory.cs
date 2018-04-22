using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper.LambdaExtension.LambdaSqlBuilder.Adapter;

namespace Dapper.LambdaExtension.Bulk
{
    public static class SqlBulkFactory
    {
 

        public static ISqlBulk GetAdapterInstance(SqlAdapterType adapter)
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
