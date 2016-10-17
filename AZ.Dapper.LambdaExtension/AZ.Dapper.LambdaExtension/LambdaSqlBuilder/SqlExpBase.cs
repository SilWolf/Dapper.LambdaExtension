using System;
using System.Collections.Generic;
using AZ.Dapper.LambdaExtension.Adapter;
using AZ.Dapper.LambdaExtension.Entity;
using AZ.Dapper.LambdaExtension.Resolver;

namespace AZ.Dapper.LambdaExtension
{
    [Serializable]
    public abstract class SqlExpBase
    {
        internal Builder.Builder _builder;
        internal LambdaResolver _resolver;
        internal SqlType _type;
        internal SqlAdapter _adapter;

        public Builder.Builder SqlBuilder { get { return _builder; } }

        public SqlType SqlType { get { return _type; } }

        public SqlExpBase()
        {

        }

        public SqlExpBase(SqlAdapter adater, string tableName)
        {
            _type = SqlType.Query;
            _adapter = adater;
            _builder = new Builder.Builder(_type, tableName, GetAdapterInstance(_adapter));
            _resolver = new LambdaResolver(_builder);
        }

        public string SqlString
        {
            get
            {
                return _builder.SqlString();
            }
        }

        public string QueryPage(int pageSize, int? pageNumber = null)
        {
            return _builder.QueryPage(pageSize, pageNumber);
        }

        public IDictionary<string, object> Parameters
        {
            get
            {
                return _builder.Parameters; 
                
            }
        }

        /// <summary>
        /// 主要给Dapper用
        /// </summary>
        public string[] SplitColumns
        {
            get { return _builder.SplitColumns.ToArray(); }
        }

        #region update

        public void Clear()
        {
            _builder.Clear();
        }

        #endregion

        public void SetAdapter(SqlAdapter adapter)
        {
            _builder.Adapter = GetAdapterInstance(adapter);
        }

        private ISqlAdapter GetAdapterInstance(SqlAdapter adapter)
        {
            switch (adapter)
            {
                case SqlAdapter.SqlServer:
                    return new SqlserverAdapter();
                case SqlAdapter.Sqlite:
                    return new Sqlite3Adapter();
                case SqlAdapter.Oracle:
                    return new OracleAdapter();
                case SqlAdapter.MySql:
                    return new MySqlAdapter();
                case SqlAdapter.Postgres:
                    return new PostgresAdapter();
                case SqlAdapter.SqlAnyWhere:
                    return new SqlAnyWhereAdapter();
                default:
                    throw new ArgumentException("The specified Sql Adapter was not recognized");
            }
        }

        //public static SqlAdapter GetAdapterByDb(IDbConnection dbconnection)
        //{
        //    SqlAdapter adapter = SqlAdapter.SqlServer2005;


        //}

       
    }
}
