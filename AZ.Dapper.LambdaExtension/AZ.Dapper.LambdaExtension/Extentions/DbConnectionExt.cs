using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Dapper.LambdaExtension.Helpers;
using Dapper.LambdaExtension.LambdaSqlBuilder.Adapter;
using Dapper.LambdaExtension.LambdaSqlBuilder.Attributes;
using Dapper.LambdaExtension.LambdaSqlBuilder.Entity;

using Dapper;
using Dapper.LambdaExtension.LambdaSqlBuilder;


namespace Dapper.LambdaExtension.Extentions
{
    public static partial class DbConnectionExt
    {
        private static bool _typeRegistered = false;
        static DbConnectionExt()
        {

        }

        public static IDbTransaction BeginTransaction(this IDbConnection db, IDbTransaction trans)
        {
            if (trans == null)
            {
                return db.BeginTransaction();
            }
            else
            {
                return trans;
            }
        }

        public static SqlAdapterType GetAdapter(this IDbConnection dbconn)
        {
            if (!_typeRegistered)
            {
                PreApplicationStart.RegisterTypeMaps();
                _typeRegistered = true;
            }

            var typeName = dbconn.GetType().Name;
            if (typeName.Contains("MySqlConnection"))
            {
                return SqlAdapterType.MySql;
            }

            if (typeName.Contains("SqlConnection"))
            {
                return SqlAdapterType.SqlServer;
            }
            if (typeName.Contains("SQLiteConnection"))
            {
                return SqlAdapterType.Sqlite;
            }
            if (typeName.Contains("SqliteConnection"))
            {
                return SqlAdapterType.Sqlite;
            }
            if (typeName.Contains("OracleConnection"))
            {
                return SqlAdapterType.Oracle;
            }

            if (typeName.Contains("NpgsqlConnection"))
            {
                return SqlAdapterType.Postgres;
            }
            if (typeName.Contains("SAConnection"))
            {
                return SqlAdapterType.SqlAnyWhere;
            }
            if (typeName.Contains("SqlCeConnection"))
            {
                return SqlAdapterType.SqlServerCE;
            }
            return SqlAdapterType.SqlServer;
        }


        public static SqlExp<T> GetSqlExp<T>(this IDbConnection db ,bool forCount=false)
        {
            return new SqlExp<T>(db.GetAdapter(),forCount);
        }

        /// <summary>
        /// 根据实体类创建数据库表
        /// </summary>
        /// <typeparam name="T">实体类类型</typeparam>
        /// <param name="db"></param>
        /// <param name="transaction"></param>
        public static void CreateTable<T>(this IDbConnection db, IDbTransaction transaction = null)
        {
            var dbAdapter = AdapterFactory.GetAdapterInstance(db.GetAdapter());

            var entityDef = EntityHelper.GetEntityDefine<T>();

            var createTableSql = dbAdapter.CreateTableSql(entityDef.Item1, entityDef.Item2);

            ExecuteSql(db, transaction, createTableSql);
        }


        /// <summary>
        /// 根据实体类创建数据库表
        /// </summary>
        /// <typeparam name="T">实体类类型</typeparam>
        /// <param name="db"></param>
        /// <param name="transaction"></param>
        public static void CreateTableIfNotExist<T>(this IDbConnection db, IDbTransaction transaction = null)
        {
            if (!db.TableExist<T>(transaction))
            {
                var dbAdapter = AdapterFactory.GetAdapterInstance(db.GetAdapter());

                var entityDef = EntityHelper.GetEntityDefine<T>();

                var createTableSql = dbAdapter.CreateTableSql(entityDef.Item1, entityDef.Item2);

                ExecuteSql(db, transaction, createTableSql);
            }
        }


        public static void CreateTable(this IDbConnection db, SqlTableDefine tableDefine, List<SqlColumnDefine> columnList, IDbTransaction transaction = null)
        {
            var dbAdapter = AdapterFactory.GetAdapterInstance(db.GetAdapter());

            var createTableSql = dbAdapter.CreateTableSql(tableDefine, columnList);

             

            ExecuteSql(db, transaction, createTableSql);
        }

        public static void CreateTableIfNotExist(this IDbConnection db, SqlTableDefine tableDefine, List<SqlColumnDefine> columnList, IDbTransaction transaction = null)
        {
            if (!db.TableExist(tableDefine, transaction))
            {
                var dbAdapter = AdapterFactory.GetAdapterInstance(db.GetAdapter());

                var createTableSql = dbAdapter.CreateTableSql(tableDefine, columnList);



                db.ExecuteSql(transaction, createTableSql);
            }
        }


      

        public static bool TableExist<T>(this IDbConnection db, IDbTransaction transaction = null)
        {
            var entityDef = EntityHelper.GetEntityDefine<T>();

            return db.TableExist(entityDef.Item1, transaction);
        }

        public static bool TableExist(this IDbConnection db, SqlTableDefine tableDefine, IDbTransaction transaction = null)
        {
            var tableName = tableDefine.Name;
            var tableSchema = "";

            if (!string.IsNullOrEmpty(tableDefine.TableAttribute?.Name))
            {
                tableName = tableDefine.TableAttribute.Name;
                tableSchema = tableDefine.TableAttribute.Schema;
            }
            return db.TableExist(tableName, tableSchema, transaction);
        }

        public static bool TableExist(this IDbConnection db, string tableName, string tableSchema = null, IDbTransaction transaction = null)
        {
            var dbAdapter = AdapterFactory.GetAdapterInstance(db.GetAdapter());

            var tableExistSql = dbAdapter.TableExistSql(tableName, tableSchema);

            return db.ExecuteScalar<int>(tableExistSql, transaction: transaction) > 0;
        }

        /// <summary>
        /// 根据实体类删除数据表
        /// </summary>
        /// <typeparam name="T">实体类类型</typeparam>
        /// <param name="db"></param>
        public static void DropTable<T>(this IDbConnection db, IDbTransaction transaction = null)
        {

            var tableDefine = EntityHelper.GetEntityDefine<T>().Item1;

            db.DropTable(tableDefine, transaction);

        }

        public static void DropTable(this IDbConnection db, SqlTableDefine tableDefine, IDbTransaction transaction = null)
        {
            var tableSchema = string.Empty;
            var tableName = tableDefine.Name;

            if (!string.IsNullOrEmpty(tableDefine.TableAttribute?.Name))
            {
                tableName = tableDefine.TableAttribute.Name;
                tableSchema = tableDefine.TableAttribute.Schema;
            }
            db.DropTable(tableName, tableSchema, transaction);
        }

        public static void DropTable(this IDbConnection db, string tableName, string tableSchema, IDbTransaction transaction = null)
        {
            var dbAdapter = AdapterFactory.GetAdapterInstance(db.GetAdapter());

            var sql = dbAdapter.DropTableSql(tableName, tableSchema);

            db.Execute(sql, transaction: transaction);
        }
        /// <summary>
        /// 根据实体类删除数据表
        /// </summary>
        /// <typeparam name="T">实体类类型</typeparam>
        /// <param name="db"></param>
        public static void DropTableIfExist<T>(this IDbConnection db, IDbTransaction transaction = null)
        {

            var tableDefine = EntityHelper.GetEntityDefine<T>().Item1;

            db.DropTableIfExist(tableDefine, transaction);

        }

        public static void DropTableIfExist(this IDbConnection db, SqlTableDefine tableDefine, IDbTransaction transaction = null)
        {
            var tableSchema = string.Empty;
            var tableName = tableDefine.Name;

            if (!string.IsNullOrEmpty(tableDefine.TableAttribute?.Name))
            {
                tableName = tableDefine.TableAttribute.Name;
                tableSchema = tableDefine.TableAttribute.Schema;
            }
            db.DropTableIfExist(tableName, tableSchema, transaction);
        }

        public static void DropTableIfExist(this IDbConnection db, string tableName, string tableSchema, IDbTransaction transaction = null)
        {
            var dbAdapter = AdapterFactory.GetAdapterInstance(db.GetAdapter());

            var sql = dbAdapter.DropTableIfExistSql(tableName, tableSchema);

            db.Execute(sql, transaction: transaction);
        }

        /// <summary>
        /// 根据实体类删除数据表
        /// </summary>
        /// <typeparam name="T">实体类类型</typeparam>
        /// <param name="db"></param>
        /// <param name="transaction"></param>
        public static void TruncateTable<T>(this IDbConnection db, IDbTransaction transaction = null)
        {

            var tableDefine = EntityHelper.GetEntityDefine<T>().Item1;

            db.TruncateTable(tableDefine, transaction);

        }

        public static void TruncateTable(this IDbConnection db, SqlTableDefine tableDefine, IDbTransaction transaction = null)
        {
            var tableSchema = string.Empty;
            var tableName = tableDefine.Name;

            if (!string.IsNullOrEmpty(tableDefine.TableAttribute?.Name))
            {
                tableName = tableDefine.TableAttribute.Name;
                tableSchema = tableDefine.TableAttribute.Schema;
            }
            db.TruncateTable(tableName, tableSchema, transaction);
        }

        public static void TruncateTable(this IDbConnection db, string tableName, string tableSchema, IDbTransaction transaction = null)
        {
            var dbAdapter = AdapterFactory.GetAdapterInstance(db.GetAdapter());

            var sql = dbAdapter.TruncateTableSql(tableName, tableSchema);

            db.Execute(sql, transaction: transaction);
        }
        
        public static bool SchemaExist<T>(this IDbConnection db, IDbTransaction transaction = null)
        {
            var entityDef = EntityHelper.GetEntityDefine<T>();

            return db.SchemaExist(entityDef.Item1, transaction);
        }

        public static bool SchemaExist(this IDbConnection db, SqlTableDefine tableDefine, IDbTransaction transaction = null)
        {
           
            var tableSchema = "";

            if (!string.IsNullOrEmpty(tableDefine.TableAttribute?.Name))
            {
                
                tableSchema = tableDefine.TableAttribute.Schema;
            }
            return db.SchemaExist( tableSchema, transaction);
        }

        public static bool SchemaExist(this IDbConnection db, string tableSchema , IDbTransaction transaction = null)
        {
            var dbAdapter = AdapterFactory.GetAdapterInstance(db.GetAdapter());

            var schemaExistSql = dbAdapter.SchemaExistsSql(tableSchema);

            if (string.IsNullOrEmpty(schemaExistSql))
            {
                return true;
            }

            return db.ExecuteScalar<int>(schemaExistSql, transaction: transaction) > 0;
        }




        /// <summary>
        /// 根据实体类创建schema
        /// </summary>
        /// <typeparam name="T">实体类类型</typeparam>
        /// <param name="db"></param>
        public static void CreateSchema<T>(this IDbConnection db, IDbTransaction transaction = null)
        {

            var tableDefine = EntityHelper.GetEntityDefine<T>().Item1;

            db.CreateSchema(tableDefine, transaction);

        }

        public static void CreateSchema(this IDbConnection db, SqlTableDefine tableDefine, IDbTransaction transaction = null)
        {
            var tableSchema = string.Empty;
     
            if (!string.IsNullOrEmpty(tableDefine.TableAttribute?.Name))
            {
                
                tableSchema = tableDefine.TableAttribute.Schema;
            }
            db.CreateSchema( tableSchema, transaction);
        }

        public static void CreateSchema(this IDbConnection db, string tableSchema, IDbTransaction transaction = null)
        {
            var dbAdapter = AdapterFactory.GetAdapterInstance(db.GetAdapter());

            var sql = dbAdapter.CreateSchemaSql(tableSchema);

            if (string.IsNullOrEmpty(sql))
            {
                return ;
            }
            db.Execute(sql, transaction: transaction);
        }


        /// <summary>
        /// 根据实体类创建schema
        /// </summary>
        /// <typeparam name="T">实体类类型</typeparam>
        /// <param name="db"></param>
        public static void CreateSchemaIfNotExists<T>(this IDbConnection db, IDbTransaction transaction = null)
        {

            var tableDefine = EntityHelper.GetEntityDefine<T>().Item1;

            db.CreateSchemaIfNotExists(tableDefine, transaction);

        }

        public static void CreateSchemaIfNotExists(this IDbConnection db, SqlTableDefine tableDefine, IDbTransaction transaction = null)
        {
            var tableSchema = string.Empty;
     
            if (!string.IsNullOrEmpty(tableDefine.TableAttribute?.Name))
            {
                
                tableSchema = tableDefine.TableAttribute.Schema;
            }
            db.CreateSchemaIfNotExists( tableSchema, transaction);
        }

        public static void CreateSchemaIfNotExists(this IDbConnection db, string tableSchema, IDbTransaction transaction = null)
        {
            var dbAdapter = AdapterFactory.GetAdapterInstance(db.GetAdapter());

            var sql = dbAdapter.CreateSchemaIfNotExistsSql(tableSchema);

            if (string.IsNullOrEmpty(sql))
            {
                return ;
            }
            db.Execute(sql, transaction: transaction);
        }
        private static void ExecuteSql(this IDbConnection db, IDbTransaction transaction, string createTableSql)
        {
            DapperLambdaExt.DebuggingSqlString(createTableSql);

            if (transaction == null)
            {
                var trans = db.BeginTransaction();
                try
                {
                    db.Execute(createTableSql, transaction: trans);

                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();

                    DapperLambdaExt.DebuggingException(ex, createTableSql);
                    throw new DapperLamException(ex.Message, ex, createTableSql);
                }
            }
            else
            {
                try
                {
                    db.Execute(createTableSql, transaction: transaction);
                }
                catch (Exception ex)
                {
                    DapperLambdaExt.DebuggingException(ex, createTableSql);
                    throw new DapperLamException(ex.Message, ex, createTableSql);
                }
            }
        }

        
    }
}
