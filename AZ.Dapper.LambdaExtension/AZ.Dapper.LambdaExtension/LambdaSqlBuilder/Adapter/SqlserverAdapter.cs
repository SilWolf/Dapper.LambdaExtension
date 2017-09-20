using System;
using Dapper.LambdaExtension.LambdaSqlBuilder.Entity;

namespace Dapper.LambdaExtension.LambdaSqlBuilder.Adapter
{
   
    /// <summary>
    /// 支持Sqlserver 2005及以上
    /// </summary>
    
    class SqlserverAdapter : AdapterBase, ISqlAdapter
    {
        public override string AutoIncrementDefinition { get; } = "IDENTITY(1,1)";
 
        public override string ParamStringPrefix { get; } = "@";

        public override string PrimaryKeyDefinition { get; } = " Primary Key";
        public override string SelectIdentitySql { get; set; } = "SELECT SCOPE_IDENTITY()";

        public SqlserverAdapter()
            : base(SqlConst.LeftTokens[0], SqlConst.RightTokens[0], SqlConst.ParamPrefixs[0])
        {
             
        }

        public override string Field(string filedName)
        {
            return string.Format("{0}{1}{2}", _leftToken, filedName, _rightToken);
        }

        public override string Field(string tableName, string fieldName)
        {
            return string.Format("{0}.{1}", Table(tableName, ""), this.Field(fieldName)); //fieldName;
        }

        public override string Table(string tableName, string schema)
        {
            var tmpTablename = tableName;
            if (tableName.StartsWith(_leftToken) && tableName.EndsWith(_rightToken))
            {
                tmpTablename = tableName;
            }
            else
            {
                tmpTablename=string.Format("{0}{1}{2}", _leftToken, tableName, _rightToken);
            }
 
            if (!string.IsNullOrEmpty(schema))
            {
                return _leftToken + schema + _rightToken + "." + tmpTablename;
            }
            return tmpTablename;
        }

        public override string QueryPage(SqlEntity entity)
        {
            int pageSize = entity.PageSize;
            var pageNumber = entity.PageNumber - 1;
            if (pageNumber < 1)
            {
                return string.Format("SELECT TOP({4}) {0} FROM {1} {2} {3}", entity.Selection, entity.TableName, entity.Conditions, entity.OrderBy, pageSize);
            }
            
            string innerQuery = string.Format("SELECT {0},ROW_NUMBER() OVER ({1}) AS RN FROM {2} {3}",
                                            entity.Selection, entity.OrderBy, entity.TableName, entity.Conditions);
            return string.Format("SELECT TOP {0} * FROM ({1}) InnerQuery WHERE RN > {2} ORDER BY RN",
                                 pageSize, innerQuery, pageSize * pageNumber);
        }

        public override string TableExistSql(string tableName, string tableSchema)
        {
            if (string.IsNullOrEmpty(tableSchema))
            {
                tableSchema = "dbo";
            }
            return base.TableExistSql(tableName, tableSchema);
        }

        protected override string DbTypeGuid(string fieldLength)
        {
            return "uniqueidentifier";
        }
        /// <summary>
        /// DbType.DateTime
        /// A type representing a date and time value.
        /// </summary>
        /// <returns></returns>
        protected override string DbTypeDateTime(string fieldLength)
        {
            return $"DATETIME";
        }

        /// <summary>
        /// DbType.DateTime2
        /// Date and time data. Date value range is from January 1,1 AD through December 31, 9999 AD. Time value range is 00:00:00 through 23:59:59.9999999 with an accuracy of 100 nanoseconds.
        /// </summary>
        /// <returns></returns>
        protected virtual string DbTypeDateTime2(string fieldLength)
        {
            return $"DATETIME2";
        }
    }
}
