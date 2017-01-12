using System;
using Dapper.LambdaExtension.LambdaSqlBuilder.Entity;

namespace Dapper.LambdaExtension.LambdaSqlBuilder.Adapter
{
   
    /// <summary>
    /// 支持Sqlserver 2005及以上
    /// </summary>
    [Serializable]
    class SqlserverAdapter : AdapterBase, ISqlAdapter
    {
        public override string AutoIncrementDefinition { get; } = "IDENTITY(1,1)";
        public override string StringColumnDefinition { get; } = "VARCHAR(8000)";

        public override string IntColumnDefinition { get; } = "INTEGER";
        public override string LongColumnDefinition { get; } = "BIGINT";
        public override string GuidColumnDefinition { get; } = "UniqueIdentifier";
        public override string BoolColumnDefinition { get; } = "BIT";
        public override string RealColumnDefinition { get; } = "DOUBLE";
        public override string DecimalColumnDefinition { get; } = "DECIMAL(38,6)";
        public override string BlobColumnDefinition { get; } = "VARBINARY(MAX)";
        public override string DateTimeColumnDefinition { get; } = "DATETIME";
        public override string TimeColumnDefinition { get; } = "DATETIME";

        public override string StringLengthNonUnicodeColumnDefinitionFormat { get; } = "VARCHAR({0})";
        public override string StringLengthUnicodeColumnDefinitionFormat { get; } = "NVARCHAR({0})";

        public override string ParamStringPrefix { get; } = "@";

        public override string PrimaryKeyDefinition { get; } = " Primary Key";
        public override string SelectIdentitySql { get; set; } = "SELECT SCOPE_IDENTITY()";

        public SqlserverAdapter()
            : base(SqlConst.LeftTokens[0], SqlConst.RightTokens[0], SqlConst.ParamPrefixs[0])
        {
             
        }

        public override string QueryPage(SqlEntity entity)
        {
            int pageSize = entity.PageSize;
            if (entity.PageNumber < 1)
            {
                return string.Format("SELECT TOP({4}) {0} FROM {1} {2} {3}", entity.Selection, entity.TableName, entity.Conditions, entity.OrderBy, pageSize);
            }

            string innerQuery = string.Format("SELECT {0},ROW_NUMBER() OVER ({1}) AS RN FROM {2} {3}",
                                            entity.Selection, entity.OrderBy, entity.TableName, entity.Conditions);
            return string.Format("SELECT TOP {0} * FROM ({1}) InnerQuery WHERE RN > {2} ORDER BY RN",
                                 pageSize, innerQuery, pageSize * entity.PageNumber);
        }
    }
}
