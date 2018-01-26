using System;
using System.Collections.Generic;
using System.Linq;
using Dapper.LambdaExtension.LambdaSqlBuilder.Attributes;
using Dapper.LambdaExtension.LambdaSqlBuilder.Entity;

namespace Dapper.LambdaExtension.LambdaSqlBuilder.Adapter
{

    class MySqlAdapter : AdapterBase
    {
        public override string AutoIncrementDefinition { get; } = "AUTO_INCREMENT";
        //public override string StringColumnDefinition { get; } = "VARCHAR(255)";

        //public override string IntColumnDefinition { get; } = "int(11)";
        //public override string LongColumnDefinition { get; } = "BIGINT";
        //public override string GuidColumnDefinition { get; } = "char(32)";
        //public override string BoolColumnDefinition { get; } = "tinyint(1)";
        //public override string RealColumnDefinition { get; } = "DOUBLE";
        //public override string DecimalColumnDefinition { get; } = "decimal(38,6)";
        //public override string BlobColumnDefinition { get; } = "VARBINARY(MAX)";
        //public override string DateTimeColumnDefinition { get; } = "DATETIME";
        //public override string TimeColumnDefinition { get; } = "time";

        //public override string StringLengthNonUnicodeColumnDefinitionFormat { get; } = "VARCHAR({0})";
        //public override string StringLengthUnicodeColumnDefinitionFormat { get; } = "NVARCHAR({0})";

        public override string ParamStringPrefix { get; } = "@";

        public override string PrimaryKeyDefinition { get; } = " Primary Key";
        public override string SelectIdentitySql { get; set; } = "SELECT LAST_INSERT_ID()";

        public MySqlAdapter()
            : base(SqlConst.LeftTokens[1], SqlConst.RightTokens[1], SqlConst.ParamPrefixs[0])
        {

        }

        public override string QueryPage(SqlEntity entity)
        {
            int pageSize = entity.PageSize;
            int limit = pageSize * (entity.PageNumber - 1);

            return string.Format("SELECT {0} FROM {1} {2} {3} LIMIT {4},{5}", entity.Selection, entity.TableName, entity.Conditions, entity.OrderBy, limit, pageSize);
        }

        //public override string Field(string filedName)
        //{
        //    return string.Format("{0}{1}{2}", _leftToken, filedName, _rightToken);
        //}

        //public override string Field(string tableName, string fieldName)
        //{
        //    return fieldName;//string.Format("{1}", this.Table(tableName), this.Field(fieldName));
        //}
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
                tmpTablename = string.Format("{0}{1}{2}", _leftToken, tableName, _rightToken);
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


                tmpTablename = tempScheme + "." + tmpTablename;
            }
            return tmpTablename;
        }

        public override string CreateTablePrefix
        {
            get { return "CREATE TABLE if not EXISTS "; }
        }

        /// <summary>
        /// CREATE UNIQUE INDEX index_name ON table_name (column_name or column_names)
        /// </summary>
        public override string CreateIndexFormatter { get; } = "CREATE {0} INDEX if not EXISTS {1} ON {2}({3});";


        protected override string DbTypeBoolean(string fieldLength)
        {
            return "TINYINT(1)";
        }

        public override string DropTableSql(string tableName, string tableSchema)
        {
            var tablename = tableName;
            if (!string.IsNullOrEmpty(tableSchema))
            {
                tablename = $"{tableSchema}.{tablename}";
            }

            return $" DROP TABLE IF EXISTS {tablename}";
        }
    }
}
