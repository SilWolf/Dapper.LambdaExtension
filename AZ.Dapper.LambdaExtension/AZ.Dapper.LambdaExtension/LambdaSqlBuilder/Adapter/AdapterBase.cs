using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using AZ.Dapper.LambdaExtension.Entity;
using AZ.Dapper.LambdaExtension.LambdaSqlBuilder;
using AZ.Dapper.LambdaExtension.LambdaSqlBuilder.Entity;

namespace AZ.Dapper.LambdaExtension.Adapter
{
    [Serializable]
    abstract class AdapterBase : ISqlAdapter
    {
        internal string _leftToken;
        internal string _rightToken;
        internal string _prefix;

        //SqlServer express limit
        public virtual string AutoIncrementDefinition { get; } = "IDENTITY(1,1)";
        public virtual string StringColumnDefinition { get; } = "VARCHAR(255)";

        public virtual string IntColumnDefinition { get; } = "INTEGER";
        public virtual string LongColumnDefinition { get; } = "BIGINT";
        public virtual string GuidColumnDefinition { get; } = "GUID";
        public virtual string BoolColumnDefinition { get; } = "BOOL";
        public virtual string RealColumnDefinition { get; } = "DOUBLE";
        public virtual string DecimalColumnDefinition { get; } = "DECIMAL";
        public virtual string BlobColumnDefinition { get; } = "BLOB";
        public virtual string DateTimeColumnDefinition { get; } = "DATETIME";
        public virtual string TimeColumnDefinition { get; } = "DATETIME";

        public virtual string StringLengthNonUnicodeColumnDefinitionFormat { get; } = "VARCHAR({0})";
        public virtual string StringLengthUnicodeColumnDefinitionFormat { get; } = "NVARCHAR({0})";

        public virtual string ParamStringPrefix { get; } = "@";

        public virtual string PrimaryKeyDefinition { get; } = " Primary Key";

 
        public virtual string SelectIdentitySql { get; set; }


        public AdapterBase(string left, string right, string prefix)
        {
            _leftToken = left;
            _rightToken = right;
            _prefix = prefix;

            InitColumnTypeMap();
        }

        public string Query(SqlEntity entity)
        {
            return string.Format(SqlConst.QuerySQLFormatString, entity.Selection, entity.TableName, entity.Conditions, entity.Grouping, entity.Having, entity.OrderBy);
        }

        public virtual string QueryPage(SqlEntity entity)
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
        public string Insert(bool key, SqlEntity entity)
        {
            string sql = string.Format(SqlConst.InsertSQLFormatString, entity.TableName, entity.Selection, entity.Parameter);
            if (key)
            {
                sql = string.Format("{0};{1}", sql, SqlConst.SqlserverAutoKeySQLString);
            }
            return sql;
        }
        public string Update(SqlEntity entity)
        {
            return string.Format(SqlConst.UpdateSQLFormatString, entity.TableName, entity.Selection, entity.Conditions);
        }
        public string Delete(SqlEntity entity)
        {
            return string.Format(SqlConst.DeleteSQLFormatString, entity.TableName, entity.Conditions);
        }

        public virtual bool SupportParameter { get { return true; } }

        public virtual string Table(string tableName)
        {
            return string.Format("{0}{1}{2}", _leftToken, tableName, _rightToken);
        }

        public virtual string Field(string filedName)
        {
            return string.Format("{0}{1}{2}", _leftToken, filedName, _rightToken);
        }

        public virtual string Field(string tableName, string fieldName)
        {
            return string.Format("{0}.{1}", this.Table(tableName), this.Field(fieldName));
        }

        public virtual string Parameter(string parameterId)
        {
            return string.Format("{0}{1}", _prefix, parameterId);
        }

        public virtual string LikeStagement()
        {
            return "LIKE";
        }

        public virtual string LikeChars()
        {
            return "%";
        }

        #region create table or schema


        public virtual string CreateTablePrefix
        {
            get { return "CREATE TABLE "; }
        }


        public virtual string CreateTable(SqlTableDefine tableDefine,List<SqlColumnDefine> columnDefines)
        {
            var sql= CreateTablePrefix ;

            var tableName = tableDefine.Name;
            if (tableDefine.TableAttribute != null)
            {
                if (tableDefine.TableAttribute.Name != null && !string.IsNullOrEmpty(tableDefine.TableAttribute.Name))
                {
                    tableName = tableDefine.TableAttribute.Name;
                }

                if (tableDefine.TableAttribute.Schema != null)
                {
                    tableName =(tableDefine.TableAttribute.Schema + ".")+tableName;
                }
            }

            sql += _leftToken + tableName + _rightToken;

            sql += " (";

            foreach (var c in columnDefines)
            {
                var cname = _leftToken+(c.AliasName ?? c.Name)+_rightToken;
                var datatypestr = "varchar(255)";
                if (c.DataTypeAttribute != null)
                {
                    datatypestr = c.DataTypeAttribute.DataType;
                }
                else
                {
                    datatypestr = GetColumnDefinition(c.ValueType);
                }

                var primary = "";
                var increment = "";

                bool isprimary = false;
                bool isincrement = false;

                if (c.KeyAttribute != null)
                {
                    primary = PrimaryKeyDefinition;
                    isprimary = true;
                    if (c.KeyAttribute.Increment)
                    {
                        increment = AutoIncrementDefinition;
                        isincrement = true;
                    }
                }

                var nullStr = "null";
                if (!c.NullAble)
                {
                    nullStr = "not null";
                }

                if (isprimary || isincrement)
                {
                    nullStr = "not null";
                }

                // 列名 类型 是否空 其他标记
                var columnDefStr = $" {cname} {datatypestr} {nullStr} {primary} {increment},";
                sql += columnDefStr;
            }

            sql = sql.TrimEnd(',');

            sql += " );";

            return sql;
        }

        #endregion
        protected DbTypeMap DbTypeMap = new DbTypeMap();
        protected void InitColumnTypeMap()
        {
            DbTypeMap.Set<string>(DbType.String, StringColumnDefinition);
            DbTypeMap.Set<char>(DbType.StringFixedLength, StringColumnDefinition);
            DbTypeMap.Set<char?>(DbType.StringFixedLength, StringColumnDefinition);
            DbTypeMap.Set<char[]>(DbType.String, StringColumnDefinition);
            DbTypeMap.Set<bool>(DbType.Boolean, BoolColumnDefinition);
            DbTypeMap.Set<bool?>(DbType.Boolean, BoolColumnDefinition);
            DbTypeMap.Set<Guid>(DbType.Guid, GuidColumnDefinition);
            DbTypeMap.Set<Guid?>(DbType.Guid, GuidColumnDefinition);
            DbTypeMap.Set<DateTime>(DbType.DateTime, DateTimeColumnDefinition);
            DbTypeMap.Set<DateTime?>(DbType.DateTime, DateTimeColumnDefinition);
            DbTypeMap.Set<TimeSpan>(DbType.Time, TimeColumnDefinition);
            DbTypeMap.Set<TimeSpan?>(DbType.Time, TimeColumnDefinition);
            DbTypeMap.Set<DateTimeOffset>(DbType.Time, TimeColumnDefinition);
            DbTypeMap.Set<DateTimeOffset?>(DbType.Time, TimeColumnDefinition);

            DbTypeMap.Set<byte>(DbType.Byte, IntColumnDefinition);
            DbTypeMap.Set<byte?>(DbType.Byte, IntColumnDefinition);
            DbTypeMap.Set<sbyte>(DbType.SByte, IntColumnDefinition);
            DbTypeMap.Set<sbyte?>(DbType.SByte, IntColumnDefinition);
            DbTypeMap.Set<short>(DbType.Int16, IntColumnDefinition);
            DbTypeMap.Set<short?>(DbType.Int16, IntColumnDefinition);
            DbTypeMap.Set<ushort>(DbType.UInt16, IntColumnDefinition);
            DbTypeMap.Set<ushort?>(DbType.UInt16, IntColumnDefinition);
            DbTypeMap.Set<int>(DbType.Int32, IntColumnDefinition);
            DbTypeMap.Set<int?>(DbType.Int32, IntColumnDefinition);
            DbTypeMap.Set<uint>(DbType.UInt32, IntColumnDefinition);
            DbTypeMap.Set<uint?>(DbType.UInt32, IntColumnDefinition);

            DbTypeMap.Set<long>(DbType.Int64, LongColumnDefinition);
            DbTypeMap.Set<long?>(DbType.Int64, LongColumnDefinition);
            DbTypeMap.Set<ulong>(DbType.UInt64, LongColumnDefinition);
            DbTypeMap.Set<ulong?>(DbType.UInt64, LongColumnDefinition);

            DbTypeMap.Set<float>(DbType.Single, RealColumnDefinition);
            DbTypeMap.Set<float?>(DbType.Single, RealColumnDefinition);
            DbTypeMap.Set<double>(DbType.Double, RealColumnDefinition);
            DbTypeMap.Set<double?>(DbType.Double, RealColumnDefinition);

            DbTypeMap.Set<decimal>(DbType.Decimal, DecimalColumnDefinition);
            DbTypeMap.Set<decimal?>(DbType.Decimal, DecimalColumnDefinition);

            DbTypeMap.Set<byte[]>(DbType.Binary, BlobColumnDefinition);

            DbTypeMap.Set<object>(DbType.Object, StringColumnDefinition);
        }

        public DbType GetDbType(Type valueType)
        {
            return DbTypeMap.ColumnDbTypeMap[valueType];
        }

        public string GetColumnDefinition(Type valueType)
        {
            return DbTypeMap.ColumnTypeMap[valueType];
        }
    }
}
