using System;
using System.Collections.Generic;
using System.Linq;
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

        /// <summary>
        /// CREATE UNIQUE INDEX index_name ON table_name (column_name or column_names)
        /// </summary>
        public override string CreateIndexFormatter { get; } = "CREATE {0} INDEX if not EXISTS {1} ON {2}({3});";


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
         public override string CreateTableSql(SqlTableDefine tableDefine, List<SqlColumnDefine> columnDefines)
        {
            var sql = CreateTablePrefix;

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

                //if (!string.IsNullOrEmpty(tableDefine.TableAttribute.Schema))
                //{
                //    tableName = (_leftToken + tableDefine.TableAttribute.Schema + _rightToken + ".") + tableName;
                //}
                tableName = Table(tempTableName, tableDefine.TableAttribute.Schema);
            }

           

            sql += tableName;

            sql += " (";

            var indexList=new List<IndexStructure>();

            foreach (var c in columnDefines)
            {
                var tempCname = (c.AliasName ?? c.Name);

                var cname = _leftToken + tempCname + _rightToken;
                // edit by cheery, 2017/2/22 
                // change the datatype method.

                //var datatypestr = "varchar(255)";
                //if (c.DataTypeAttribute != null)
                //{
                //    datatypestr = c.DataTypeAttribute.DataType;
                //}
                //else
                //{
                //    datatypestr = GetColumnDefinition(c.ValueType);
                //}
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


            return sql;
        }

        public override string CreateSchemaSql(string schemaName)
        {
             

            return string.Empty;
        }

        public override  string CreateSchemaIfNotExistsSql(string schemaName)
        {
            
            return string.Empty;
        }

        public override string SchemaExistsSql(string schemaName)
        {
           
            return string.Empty;
           
        }
        
    }
}
