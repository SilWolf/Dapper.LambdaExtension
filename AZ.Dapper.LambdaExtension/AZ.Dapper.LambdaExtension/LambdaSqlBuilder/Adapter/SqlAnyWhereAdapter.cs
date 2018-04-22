using System;
using Dapper.LambdaExtension.LambdaSqlBuilder.Entity;

namespace Dapper.LambdaExtension.LambdaSqlBuilder.Adapter
{
    
    class SqlAnyWhereAdapter : AdapterBase
    {
        public override string AutoIncrementDefinition { get; } = "AUTOINCREMENT";
 

        public override string ParamStringPrefix { get; } = "@";

        public override string PrimaryKeyDefinition { get; } = " Primary Key";
        public override string SelectIdentitySql { get; set; } = "select @@identity ";
        public SqlAnyWhereAdapter()
            : base(SqlConst.LeftTokens[2], SqlConst.RightTokens[2], SqlConst.ParamPrefixs[0])
        {
           // AUTOINCREMENT
        }

        public override string QueryPage(SqlEntity entity)
        {
            int pageSize = entity.PageSize;
            int limit = pageSize * (entity.PageNumber - 1);
           // select top 30 start at (N - 1) * 30 + 1 * from customer
            return string.Format("SELECT TOP {4} start at {5} {0} FROM {1} {2} {3} ", entity.Selection, entity.TableName, entity.Conditions, entity.OrderBy, pageSize,limit+1 );
        }
        public override string Field(string filedName)
        {
            return string.Format("{0}{1}{2}", _leftToken, filedName, _rightToken);
        }

        public override string Field(string tableName, string fieldName)
        {
            return string.Format("{0}.{1}", Table(tableName,""), this.Field(fieldName)); //fieldName;
        }

        public override string Table(string tableName,string schema)
        {
            var tbname = tableName;
            if (tableName.StartsWith(_leftToken) && tableName.EndsWith(_rightToken))
            {
                tbname= tableName;
            }
            else
            {
                tbname = string.Format("{0}{1}{2}", _leftToken, tableName, _rightToken);
            }
              
            if (!string.IsNullOrEmpty(schema))
            {
                var tempScheme = schema;

                if (schema.StartsWith(_leftToken) && schema.EndsWith(_rightToken))
                {
                    tempScheme = schema;
                }
                else
                {
                    tempScheme = string.Format("{0}{1}{2}", _leftToken, schema, _rightToken);
                }


                tbname = tempScheme + "." + tbname;
            }
            return tbname;
        }

        public override string TableExistSql(string tableName, string tableSchema)
        {
            var sql = $"SELECT COUNT(*) FROM SYSTAB  JOIN SYSDBSPACE  n ON n.dbspace_id = SYSTAB.dbspace_id WHERE table_name = '{tableName}' ";

            if (!string.IsNullOrEmpty(tableSchema))
            {
                sql += $"AND n.dbspace_name = '{tableSchema}'";
            }

            //todo: this do not test. use it carefully.
            return sql;
        }


        protected override string DbTypeSingle(string fieldLength)
        {
            return "real";
        }

        protected override string DbTypeString(string fieldLength)
        {
            if (int.Parse(fieldLength) > 8000)
            {
                return $"long binary";
            }
            return $"CHARACTER VARYING({fieldLength})";
        }
    }
}
