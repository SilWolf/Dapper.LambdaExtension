using System;
using System.Collections.Generic;
using AZ.Dapper.LambdaExtension.Adapter;
using AZ.Dapper.LambdaExtension.Entity;
using AZ.Dapper.LambdaExtension.LambdaSqlBuilder.Adapter;
using AZ.Dapper.LambdaExtension.Resolver;

namespace AZ.Dapper.LambdaExtension
{
    [Serializable]
    public abstract class SqlExpBase
    {
        internal Builder.Builder _builder;
        internal LambdaResolver _resolver;
        internal SqlType _type;
        internal SqlAdapterType _adapter;

        internal Type _entityType;

        public Builder.Builder SqlBuilder { get { return _builder; } }

        public SqlType SqlType { get { return _type; } }

        public SqlExpBase()
        {

        }

        public SqlExpBase(SqlAdapterType adater, string tableName,Type entityType)
        {
            _type = SqlType.Query;
            _adapter = adater;
            _entityType = entityType;
            _builder = new Builder.Builder(_type, tableName,entityType, AdapterFactory.GetAdapterInstance(_adapter));
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

        public void SetAdapter(SqlAdapterType adapter)
        {
            _builder.Adapter =AdapterFactory.GetAdapterInstance(adapter);
        }

      

        //public static SqlAdapter GetAdapterByDb(IDbConnection dbconnection)
        //{
        //    SqlAdapter adapter = SqlAdapter.SqlServer2005;


        //}

       
    }
}
