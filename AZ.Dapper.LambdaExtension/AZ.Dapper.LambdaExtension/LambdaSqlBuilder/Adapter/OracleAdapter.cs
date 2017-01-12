using System;
using Dapper.LambdaExtension.LambdaSqlBuilder.Entity;

namespace Dapper.LambdaExtension.LambdaSqlBuilder.Adapter
{
    [Serializable]
    class OracleAdapter : AdapterBase
    {
        public virtual string AutoIncrementDefinition { get; } = string.Empty;
        public virtual string StringColumnDefinition { get; } = "VARCHAR(255)";

        public virtual string IntColumnDefinition { get; } = "INTEGER";
        public virtual string LongColumnDefinition { get; } = "BIGINT";
        public virtual string GuidColumnDefinition { get; } = "VARCHAR(37)";
        public virtual string BoolColumnDefinition { get; } = "BOOL";
        public virtual string RealColumnDefinition { get; } = "FLOAT";
        public virtual string DecimalColumnDefinition { get; } = "DECIMAL";
        public virtual string BlobColumnDefinition { get; } = "BLOB";
        public virtual string DateTimeColumnDefinition { get; } = "TIMESTAMP";
        public virtual string TimeColumnDefinition { get; } = "TIME";

        public virtual string StringLengthNonUnicodeColumnDefinitionFormat { get; } = "VARCHAR({0})";
        public virtual string StringLengthUnicodeColumnDefinitionFormat { get; } = "NVARCHAR({0})";

        public virtual string ParamStringPrefix { get; } = ":";

        public virtual string PrimaryKeyDefinition { get; } = " Primary Key";


        public OracleAdapter()
            : base(SqlConst.LeftTokens[2], SqlConst.RightTokens[2], SqlConst.ParamPrefixs[1])
        {

        }

        public override string QueryPage(SqlEntity entity)
        {
            int pageSize = entity.PageSize;
            int begin = (entity.PageNumber - 1) * pageSize;
            int end = entity.PageNumber * pageSize;
            return string.Format(@"SELECT * FROM (
SELECT A.*, ROWNUM RN FROM (SELECT {0} FROM {1} {2} {3}) A WHERE ROWNUM <= {5})WHERE RN >{4}", entity.Selection, entity.TableName, entity.Conditions, entity.OrderBy, begin, end);
        }
    }
}
