using System;
using System.Collections.Generic;
using System.Linq;
using Dapper.LambdaExtension.LambdaSqlBuilder.Entity;

namespace Dapper.LambdaExtension.LambdaSqlBuilder.Adapter
{
   
    /// <summary>
    /// 支持Sqlserver 2005及以上
    /// </summary>
    
    class SqlserverAdapter : AdapterBase, ISqlAdapter
    {
        public override string AutoIncrementDefinition { get; } = "IDENTITY(1,1)";
        //public override string StringColumnDefinition { get; } = "VARCHAR(8000)";

        //public override string IntColumnDefinition { get; } = "INTEGER";
        //public override string LongColumnDefinition { get; } = "BIGINT";
        //public override string GuidColumnDefinition { get; } = "UniqueIdentifier";
        //public override string BoolColumnDefinition { get; } = "BIT";
        //public override string RealColumnDefinition { get; } = "DOUBLE";
        //public override string DecimalColumnDefinition { get; } = "DECIMAL(38,6)";
        //public override string BlobColumnDefinition { get; } = "VARBINARY(MAX)";
        //public override string DateTimeColumnDefinition { get; } = "DATETIME";
        //public override string TimeColumnDefinition { get; } = "DATETIME";

        //public override string StringLengthNonUnicodeColumnDefinitionFormat { get; } = "VARCHAR({0})";
        //public override string StringLengthUnicodeColumnDefinitionFormat { get; } = "NVARCHAR({0})";

        public override string ParamStringPrefix { get; } = "@";

        public override string PrimaryKeyDefinition { get; } = " Primary Key";
        public override string SelectIdentitySql { get; set; } = "SELECT SCOPE_IDENTITY()";

        public SqlserverAdapter()
            : base(SqlConst.LeftTokens[0], SqlConst.RightTokens[0], SqlConst.ParamPrefixs[0])
        {
             
        }

        public override string Field(string filedName)
        {
            return string.Format("{0}{1}{2}", _leftToken, filedName, _rightToken);
        }

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
                tmpTablename=string.Format("{0}{1}{2}", _leftToken, tableName, _rightToken);
            }
 
            if (!string.IsNullOrEmpty(schema))
            {
                return _leftToken + schema + _rightToken + "." + tmpTablename;
            }
            return tmpTablename;
        }

        public override string QueryPage(SqlEntity entity)
        {
            int pageSize = entity.PageSize;
            var pageNumber = entity.PageNumber - 1;
            if (pageNumber < 1)
            {
                return string.Format("SELECT TOP({4}) {0} FROM {1} {2} {3}", entity.Selection, entity.TableName, entity.Conditions, entity.OrderBy, pageSize);
            }
            
            string innerQuery = string.Format("SELECT {0},ROW_NUMBER() OVER ({1}) AS RN FROM {2} {3}",
                                            entity.Selection, entity.OrderBy, entity.TableName, entity.Conditions);
            return string.Format("SELECT TOP {0} * FROM ({1}) InnerQuery WHERE RN > {2} ORDER BY RN",
                                 pageSize, innerQuery, pageSize * pageNumber);
        }


        public override string CreateTablePrefix
        {
            get { return "CREATE TABLE "; }
        }

        public override string DropTableIfNotExistPrefix
        {
            get { return "DROP TABLE "; }
        }

        public override string TableExistSql(string tableName, string tableSchema)
        {
            if (string.IsNullOrEmpty(tableSchema))
            {
                tableSchema = "dbo";
            }
            return base.TableExistSql(tableName, tableSchema);
        }

        public override string CreateTableSql(SqlTableDefine tableDefine, List<SqlColumnDefine> columnDefines)
        {
            

            var tableName = tableDefine.Name;

            var tempTableName = tableName;

            var tempSchemaName = string.Empty;

            if (tableDefine.TableAttribute != null)
            {
                if (!string.IsNullOrEmpty(tableDefine.TableAttribute.Name))
                {
                    tempTableName = tableDefine.TableAttribute.Name;// _leftToken + tableDefine.TableAttribute.Name + _rightToken;
                }

                tempSchemaName = tableDefine.TableAttribute.Schema;

             

                if (string.IsNullOrEmpty(tempSchemaName))
                {
                    tempSchemaName = "dbo";
                }

                tableName = Table(tempTableName, tempSchemaName);
            }

           
            var sql = $"IF NOT EXISTS  (SELECT * FROM INFORMATION_SCHEMA.TABLES where table_schema='{tempSchemaName}' and table_name='{tempTableName}') BEGIN  ";

            sql += CreateTablePrefix;

            sql += tableName;

            sql += " (";

            var indexList=new List<IndexStructure>();

            foreach (var c in columnDefines)
            {
                var tempCname = (c.AliasName ?? c.Name);

                var cname = _leftToken + tempCname + _rightToken;
       
                var datatypestr = GetColumnDefinition(c);


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

                var nullStr = "";
                if (!c.NullAble)
                {
                    nullStr = "not null";
                }

                if (isprimary || isincrement)
                {
                    nullStr = "not null";
                }


                if (c.IndexAttribute != null)
                {
                    if (string.IsNullOrEmpty(c.IndexAttribute.IndexName))
                    {
                        var schemaSuffix =tempSchemaName;
                        if (!string.IsNullOrEmpty(tempSchemaName))
                        {
                            schemaSuffix += "_";
                        }

                        c.IndexAttribute.IndexName="lidx_"+ schemaSuffix + tempTableName + "_" + tempCname;
                    }

                    if (indexList.Exists(p => p.IndexName == c.IndexAttribute.IndexName))
                    {
                        var ind = indexList.FirstOrDefault(v => v.IndexName == c.IndexAttribute.IndexName);
                        //if (ind != null)
                        //{
                        ind?.Columns.Add(new IndexColumnStructure() { ColumnName = cname, Asc = c.IndexAttribute.Asc });
                        //}
                    }
                    else
                    {
                        indexList.Add(new IndexStructure()
                        {
                            IndexName = c.IndexAttribute.IndexName,
                            Unique = c.IndexAttribute.Unique,
                            TableName = tableName,
                            Columns = new List<IndexColumnStructure>() {new IndexColumnStructure(){ColumnName = cname,Asc = c.IndexAttribute.Asc} }
                        });
                    }

                    //var str = string.Format(CreateIndexFormatter, uniqueStr, c.IndexAttribute.IndexName, tableName,cname);

                }


                var columnDefStr = FormatColumnDefineSql(cname, datatypestr, nullStr, primary, increment); //$" {cname} {datatypestr} {nullStr} {primary} {increment},");
                sql += columnDefStr;
            }

            sql = sql.TrimEnd(',');

            sql += " );";


            //process index create sql 
            //var indexSb = new StringBuilder();

            foreach (var indexItem in indexList  )          {

                var uniqueStr = string.Empty;
                if (indexItem.Unique)
                {
                    uniqueStr = "UNIQUE";
                }

                var columnlist=new List<string>();

                var columnstr = string.Empty;

                foreach (var column in indexItem.Columns)
                {
                    //var tmpStr = column.ColumnName + " " + (column.Asc? "ASC" : "DESC")+",";
                    columnlist.Add(column.ColumnName + " " + (column.Asc ? "ASC" : "DESC"));
                }

                columnstr = string.Join(",", columnlist);

                var str = string.Format(CreateIndexFormatter, uniqueStr, indexItem.IndexName, indexItem.TableName, columnstr);

                //indexSb.AppendLine(str);
                sql += str;
            }

            //sql += indexSb.ToString();

            sql += " END;";

            return sql;
        }

        public override string DropTableSql(string tableName, string tableSchema)
        {
            var tablename = tableName;

            var schemaname = tableSchema;
            if (!string.IsNullOrEmpty(tableSchema))
            {
                tablename = $"{tableSchema}.{tablename}";
            }

            if (string.IsNullOrEmpty(schemaname))
            {
                schemaname = "dbo";
            }



            var sql = $"IF EXISTS  (SELECT * FROM INFORMATION_SCHEMA.TABLES where table_schema='{schemaname}' and table_name='{tablename}') BEGIN  ";
                
                sql+= $" DROP TABLE {tablename}   END;";
           
            return sql;
        }
 

        public override  string CreateSchemaIfNotExistsSql(string schemaName)
        {
            if (!string.IsNullOrEmpty(schemaName))
            {
                var sql = $"IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.schemata where schema_name='{schemaName}') BEGIN ";
                sql+= $" exec('CREATE SCHEMA {schemaName}')  END ;";
                return sql;
            }

            return string.Empty;
        }

        protected override string DbTypeGuid(string fieldLength)
        {
            return "uniqueidentifier";
        }
        /// <summary>
        /// DbType.DateTime
        /// A type representing a date and time value.
        /// </summary>
        /// <returns></returns>
        protected override string DbTypeDateTime(string fieldLength)
        {
            return $"DATETIME";
        }

        /// <summary>
        /// DbType.DateTime2
        /// Date and time data. Date value range is from January 1,1 AD through December 31, 9999 AD. Time value range is 00:00:00 through 23:59:59.9999999 with an accuracy of 100 nanoseconds.
        /// </summary>
        /// <returns></returns>
        protected override string DbTypeDateTime2(string fieldLength)
        {
            return $"DATETIME2";
        }
    }
}
