using System;
using AZ.Dapper.LambdaExtension.Entity;

namespace AZ.Dapper.LambdaExtension.Adapter
{
    [Serializable]
    class SqlAnyWhereAdapter : AdapterBase
    {
        public override string AutoIncrementDefinition { get; } = "AUTOINCREMENT";
        public override string StringColumnDefinition { get; } = "VARCHAR(255)";

        public override string IntColumnDefinition { get; } = "integer";
        public override string LongColumnDefinition { get; } = "BIGINT";
        public override string GuidColumnDefinition { get; } = "varchar(32)";
        public override string BoolColumnDefinition { get; } = "bit";
        public override string RealColumnDefinition { get; } = "real";
        public override string DecimalColumnDefinition { get; } = "decimal(38,6)";
        public override string BlobColumnDefinition { get; } = "long binary";
        public override string DateTimeColumnDefinition { get; } = "DATETIME";
        public override string TimeColumnDefinition { get; } = "time";

        public override string StringLengthNonUnicodeColumnDefinitionFormat { get; } = "VARCHAR({0})";
        public override string StringLengthUnicodeColumnDefinitionFormat { get; } = "NVARCHAR({0})";

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

        
    }
}
