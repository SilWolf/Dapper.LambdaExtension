using System;
using Dapper.LambdaExtension.LambdaSqlBuilder.Entity;

namespace Dapper.LambdaExtension.LambdaSqlBuilder.Adapter
{
    
    class Sqlite3Adapter : AdapterBase
    {
        public override string AutoIncrementDefinition { get; } = "AUTOINCREMENT";
        //public override string StringColumnDefinition { get; } = "VARCHAR(255)";

        //public override string IntColumnDefinition { get; } = "INTEGER";
        //public override string LongColumnDefinition { get; } = "INTEGER";
        //public override string GuidColumnDefinition { get; } = "VARCHAR(48)";
        //public override string BoolColumnDefinition { get; } = "INTEGER";
        //public override string RealColumnDefinition { get; } = "REAL";
        //public override string DecimalColumnDefinition { get; } = "NUMERIC";
        //public override string BlobColumnDefinition { get; } = "BLOB";
        //public override string DateTimeColumnDefinition { get; } = "DATETIME";
        //public override string TimeColumnDefinition { get; } = "DATETIME";

        //public override string StringLengthNonUnicodeColumnDefinitionFormat { get; } = "VARCHAR({0})";
        //public override string StringLengthUnicodeColumnDefinitionFormat { get; } = "NVARCHAR({0})";

        public override string ParamStringPrefix { get; } = "@";

        public override string PrimaryKeyDefinition { get; } = " Primary Key";

        public override string SelectIdentitySql { get; set; } = "select last_insert_rowid()";

        public override string CreateTablePrefix { get; } = "create table if not EXISTS ";

        public Sqlite3Adapter()
            : base(SqlConst.LeftTokens[0], SqlConst.RightTokens[0], SqlConst.ParamPrefixs[0])
        {

        }

        public override string QueryPage(SqlEntity entity)
        {
            int limit = entity.PageSize;
            int offset = limit * (entity.PageNumber - 1);
            return string.Format("SELECT {0} FROM {1} {2} {3} LIMIT {4} OFFSET {5}", entity.Selection, entity.TableName, entity.Conditions, entity.OrderBy, limit, offset);
        }

        public override string Field(string tableName, string fieldName)
        {
            return this.Field(fieldName);
        }

        public override string TableExistSql(string tableName, string tableSchema)
        {
            var table = Table(tableName, tableSchema);
            var sql = $"SELECT COUNT(*) FROM sqlite_master where type='table' and name='{table}'";

            return sql;
        }


        public override string Table(string tableName, string schema)
        {
            if (tableName.StartsWith(_leftToken) && tableName.EndsWith(_rightToken))
            {
                return tableName;
            }
            var tbname = string.Format("{0}{1}{2}", "", tableName, "");
            if (!string.IsNullOrEmpty(schema))
            {
                return _leftToken + schema  + "_" + tbname + _rightToken;
            }
            return tbname;
        }

        protected override string DbTypeBoolean(string fieldLength)
        {
            return "boolean";
        }

        protected override string DbTypeGuid(string fieldLength)
        {
            if (string.IsNullOrEmpty(fieldLength))
            {
                fieldLength = "36";
            }
            return $"CHAR({fieldLength})";
        }

        
    }
}
