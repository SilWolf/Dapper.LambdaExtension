using System;
using Dapper.LambdaExtension.LambdaSqlBuilder.Entity;

namespace Dapper.LambdaExtension.LambdaSqlBuilder.Adapter
{
    [Serializable]
    class MySqlAdapter : AdapterBase
    {
        public override string AutoIncrementDefinition { get; } = "AUTO_INCREMENT";
        public override string StringColumnDefinition { get; } = "VARCHAR(255)";

        public override string IntColumnDefinition { get; } = "int(11)";
        public override string LongColumnDefinition { get; } = "BIGINT";
        public override string GuidColumnDefinition { get; } = "char(32)";
        public override string BoolColumnDefinition { get; } = "tinyint(1)";
        public override string RealColumnDefinition { get; } = "DOUBLE";
        public override string DecimalColumnDefinition { get; } = "decimal(38,6)";
        public override string BlobColumnDefinition { get; } = "VARBINARY(MAX)";
        public override string DateTimeColumnDefinition { get; } = "DATETIME";
        public override string TimeColumnDefinition { get; } = "time";

        public override string StringLengthNonUnicodeColumnDefinitionFormat { get; } = "VARCHAR({0})";
        public override string StringLengthUnicodeColumnDefinitionFormat { get; } = "NVARCHAR({0})";

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

        public override string Field(string filedName)
        {
            return string.Format("{0}{1}{2}", _leftToken, filedName, _rightToken);
        }

        public override string Field(string tableName, string fieldName)
        {
            return fieldName;//string.Format("{1}", this.Table(tableName), this.Field(fieldName));
        }

        public override string CreateTablePrefix
        {
            get { return "CREATE TABLE if not EXISTS "; }
        }
    }
}
