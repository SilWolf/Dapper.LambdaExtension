using System;
using System.Collections.Generic;
using System.Linq;
using Dapper.LambdaExtension.LambdaSqlBuilder.Entity;

namespace Dapper.LambdaExtension.LambdaSqlBuilder.Adapter
{
    
    class PostgresAdapter : AdapterBase
    {
        public override string AutoIncrementDefinition { get; } = "serial";

        //public override string StringColumnDefinition { get; } = "VARCHAR(255)";

        //public override string IntColumnDefinition { get; } = "integer";
        //public override string LongColumnDefinition { get; } = "BIGINT";
        //public override string GuidColumnDefinition { get; } = "uuid";
        //public override string BoolColumnDefinition { get; } = "boolean";
        //public override string RealColumnDefinition { get; } = "double precision";
        //public override string DecimalColumnDefinition { get; } = "numeric(38,6)";
        //public override string BlobColumnDefinition { get; } = "bytea";
        //public override string DateTimeColumnDefinition { get; } = "timestamp";
        //public override string TimeColumnDefinition { get; } = "time";

        //public override string StringLengthNonUnicodeColumnDefinitionFormat { get; } = "VARCHAR({0})";
        //public override string StringLengthUnicodeColumnDefinitionFormat { get; } = "NVARCHAR({0})";

        public override string ParamStringPrefix { get; } = ":";

        public override string PrimaryKeyDefinition { get; } = " Primary Key";
        public override string SelectIdentitySql { get; set; } = "RETURNING";

        public PostgresAdapter()
            : base(SqlConst.LeftTokens[2], SqlConst.RightTokens[2], SqlConst.ParamPrefixs[0])
        {

        }

        public override string QueryPage(SqlEntity entity)
        {
            int pageSize = entity.PageSize;
            int limit = pageSize * (entity.PageNumber - 1);

            return string.Format("SELECT {0} FROM {1} {2} {3} LIMIT {4} offset {5}", entity.Selection, entity.TableName, entity.Conditions, entity.OrderBy, pageSize,limit );
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
            var tbname = string.Format("{0}{1}{2}", _leftToken, tableName, _rightToken);
            if (!string.IsNullOrEmpty(schema))
            {
                return _leftToken + schema + _rightToken + "."+tbname;
            }
            return tbname;
        }

        

        public override string LikeStagement()
        {
            return "~*";
        }

        public override string LikeChars()
        {
            return ".*";
        }

        public override string CreateTablePrefix
        {
            get { return "CREATE TABLE if not EXISTS "; }
        }

        /// <summary>
        /// CREATE UNIQUE INDEX index_name ON table_name (column_name or column_names)
        /// </summary>
        public override string CreateIndexFormatter { get; } = "CREATE {0} INDEX if not EXISTS {1} ON {2}({3});";



        /// <summary>
        /// 同样,获取最后一条插入数据的id,在postgresql中也有点特殊.
        /// </summary>
        /// <param name="incrementColumnName"></param>
        /// <returns></returns>
        public override string GetIdentitySql(string incrementColumnName)
        {
            if (!string.IsNullOrEmpty(incrementColumnName))
            {
                return SelectIdentitySql + " " + incrementColumnName + ";";
            }

            return string.Empty;
        }

        public override string FormatColumnDefineSql(string columName, string dataTypestr, string nullstr, string primaryStr, string incrementStr)
        {
            //postgres 的自增字段比较特殊,是用一个特定的数据类型来标识的.
            if (!string.IsNullOrEmpty(incrementStr))
            {
                dataTypestr = incrementStr;
            }

            return $" {columName} {dataTypestr} {nullstr} {primaryStr},";
        }

        public override string TableExistSql(string tableName, string tableSchema)
        {
            if (string.IsNullOrEmpty(tableSchema))
            {
                tableSchema = "public";
            }
            //var sql = "SELECT COUNT(*) FROM pg_class WHERE relname = {0}" //this is return all of schemas table count.
            var sql =
                $"SELECT COUNT(*) FROM pg_class JOIN pg_catalog.pg_namespace n ON n.oid = pg_class.relnamespace WHERE relname = '{tableName}' AND nspname = '{tableSchema}'";
            return sql;
        }


        protected override string DbTypeDateTime(string fieldLength)
        {
            if (string.IsNullOrEmpty(fieldLength))
            {
                fieldLength = "3";
            }
            else
            {
                var length = 3;

                if (!int.TryParse(fieldLength, out length))
                {
                    length = 3;
                }
                if (length > 6)
                {
                    length = 6;
                }
                fieldLength=length.ToString();
            }
            return $"TIMESTAMP({fieldLength})";
        }

        protected override string DbTypeDateTime2(string fieldLength)
        {
            if (string.IsNullOrEmpty(fieldLength))
            {
                fieldLength = "6";
            }
            else
            {
                var length = 6;

                if (!int.TryParse(fieldLength, out length))
                {
                    length = 6;
                }
                if (length > 6)
                {
                    length = 6;
                }
                fieldLength = length.ToString();
            }
            return $"TIMESTAMP({fieldLength})";
        }

        protected override string DbTypeTime(string fieldLength)
        {
            if (string.IsNullOrEmpty(fieldLength))
            {
                fieldLength = "3";
            }
            else
            {
                var length = 3;

                if (!int.TryParse(fieldLength, out length))
                {
                    length = 3;
                }
                if (length > 6)
                {
                    length = 6;
                }
                fieldLength = length.ToString();
            }
            return $"TIME({fieldLength})";
        }

        protected override string DbTypeBinary(string fieldLength)
        {
            return "BYTEA";//bytea
        }

        protected override string DbTypeDecimal(string fieldLength)
        {
            return base.DbTypeVarNumeric(fieldLength);
        }

        protected override string DbTypeGuid(string fieldLength)
        {
            return "uuid";
        }

        protected override string DbTypeBoolean(string fieldLength)
        {
            return "boolean";
        }
    }
}
