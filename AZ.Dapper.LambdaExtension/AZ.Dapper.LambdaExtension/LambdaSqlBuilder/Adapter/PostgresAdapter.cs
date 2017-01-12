using System;
using Dapper.LambdaExtension.LambdaSqlBuilder.Entity;

namespace Dapper.LambdaExtension.LambdaSqlBuilder.Adapter
{
    [Serializable]
    class PostgresAdapter : AdapterBase
    {
        public override string AutoIncrementDefinition { get; } = "serial";
        public override string StringColumnDefinition { get; } = "VARCHAR(255)";

        public override string IntColumnDefinition { get; } = "integer";
        public override string LongColumnDefinition { get; } = "BIGINT";
        public override string GuidColumnDefinition { get; } = "uuid";
        public override string BoolColumnDefinition { get; } = "boolean";
        public override string RealColumnDefinition { get; } = "double precision";
        public override string DecimalColumnDefinition { get; } = "numeric(38,6)";
        public override string BlobColumnDefinition { get; } = "bytea";
        public override string DateTimeColumnDefinition { get; } = "timestamp";
        public override string TimeColumnDefinition { get; } = "time";

        public override string StringLengthNonUnicodeColumnDefinitionFormat { get; } = "VARCHAR({0})";
        public override string StringLengthUnicodeColumnDefinitionFormat { get; } = "NVARCHAR({0})";

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
            var tbname = string.Format("{0}{1}{2}", "", tableName, "");
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
    }
}
