using System;
using System.Data;
using AZ.Dapper.LambdaExtension.Adapter;
using AZ.Dapper.LambdaExtension.Helpers;
using AZ.Dapper.LambdaExtension.LambdaSqlBuilder.Adapter;
using Dapper;

namespace AZ.Dapper.LambdaExtension.Extentions
{
    public static class DbConnectionExt
    {
        public static SqlAdapterType GetAdapter(this IDbConnection dbconn)
        {
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

            return SqlAdapterType.SqlServer;
        }


        /// <summary>
        /// 根据实体类创建数据库表
        /// </summary>
        /// <typeparam name="T">实体类类型</typeparam>
        /// <param name="db"></param>
        public static void CreateTable<T>(this IDbConnection db)
        {
           var dbAdapter= AdapterFactory.GetAdapterInstance(db.GetAdapter());
 
            var entityDef = EntityHelper.GetEntityDefine<T>();

            var createTableSql = dbAdapter.CreateTable(entityDef.Item1, entityDef.Item2);

            db.Execute(createTableSql);

        }

 
    }






}
