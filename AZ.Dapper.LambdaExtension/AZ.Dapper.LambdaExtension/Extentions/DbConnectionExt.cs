using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using Dapper.LambdaExtension.Helpers;
using Dapper.LambdaExtension.LambdaSqlBuilder.Adapter;
using Dapper.LambdaExtension.LambdaSqlBuilder.Attributes;
using Dapper.LambdaExtension.LambdaSqlBuilder.Entity;
 
  using Dapper;
 

namespace Dapper.LambdaExtension.Extentions
{
    public static class DbConnectionExt
    {
        private static bool _typeRegistered = false;
        static DbConnectionExt()
        {
           
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


        /// <summary>
        /// 根据实体类创建数据库表
        /// </summary>
        /// <typeparam name="T">实体类类型</typeparam>
        /// <param name="db"></param>
        /// <param name="transaction"></param>
        public static void CreateTable<T>(this IDbConnection db,IDbTransaction transaction=null)
        {
           var dbAdapter= AdapterFactory.GetAdapterInstance(db.GetAdapter());
 
            var entityDef = EntityHelper.GetEntityDefine<T>();

            var createTableSql = dbAdapter.CreateTableSql(entityDef.Item1, entityDef.Item2);

            db.Execute(createTableSql,transaction:transaction);

        }

        public static void CreateTable(this IDbConnection db,SqlTableDefine tableDefine, List<SqlColumnDefine> columnList, IDbTransaction transaction = null)
        {
            var dbAdapter = AdapterFactory.GetAdapterInstance(db.GetAdapter());

            var createTableSql = dbAdapter.CreateTableSql(tableDefine, columnList);

            db.Execute(createTableSql,transaction:transaction);
        }
        public static bool TableExist<T>(this IDbConnection db, IDbTransaction transaction = null)
        {
            var entityDef = EntityHelper.GetEntityDefine<T>();

            return db.TableExist(entityDef.Item1,transaction);
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
            return db.TableExist(tableName,tableSchema,transaction);
        }

        public static bool TableExist(this IDbConnection db, string tableName, string tableSchema = null,IDbTransaction transaction=null)
        {
            var dbAdapter = AdapterFactory.GetAdapterInstance(db.GetAdapter());

            var tableExistSql = dbAdapter.TableExistSql(tableName, tableSchema);

            return db.ExecuteScalar<int>(tableExistSql,transaction:transaction) > 0;
        }

        /// <summary>
        /// 根据实体类删除数据表
        /// </summary>
        /// <typeparam name="T">实体类类型</typeparam>
        /// <param name="db"></param>
        public static void DropTable<T>(this IDbConnection db, IDbTransaction transaction = null)
        {
 
            var tableDefine = EntityHelper.GetEntityDefine<T>().Item1;
            
            db.DropTable(tableDefine,transaction);

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
            db.DropTable(tableName,tableSchema,transaction);
        }

        public static void DropTable(this IDbConnection db, string tableName,string tableSchema, IDbTransaction transaction = null)
        {
            var dbAdapter = AdapterFactory.GetAdapterInstance(db.GetAdapter());

            var sql = dbAdapter.DropTableSql(tableName, tableSchema);

            db.Execute(sql,transaction:transaction);
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

            db.TruncateTable(tableDefine,transaction);

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
            db.TruncateTable(tableName, tableSchema,transaction);
        }

        public static void TruncateTable(this IDbConnection db, string tableName, string tableSchema, IDbTransaction transaction = null)
        {
            var dbAdapter = AdapterFactory.GetAdapterInstance(db.GetAdapter());

            var sql = dbAdapter.TruncateTableSql(tableName, tableSchema);

            db.Execute(sql,transaction:transaction);
        }
    }
 
}
