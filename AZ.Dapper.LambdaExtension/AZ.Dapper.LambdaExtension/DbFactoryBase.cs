using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper.LambdaExtension.Extentions;

namespace Dapper.LambdaExtension
{
    public abstract class DbFactoryBase<TDbConnection>:IDbFactory where TDbConnection:IDbConnection, new()
    {
        public virtual  string ConnectionString { get; set; }
        public virtual IDbConnection OpenDbConnection()
        {
            return OpenDbConnection(ConnectionString);
        }

        public virtual IDbConnection OpenDbConnection(string connectionString)
        {
            var conn = new TDbConnection();
            conn.ConnectionString = connectionString;
            conn.Open();
            return conn;
        }
    }
}
