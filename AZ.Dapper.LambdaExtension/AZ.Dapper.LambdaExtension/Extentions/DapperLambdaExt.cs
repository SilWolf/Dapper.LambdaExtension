﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;

using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dapper.LambdaExtension.LambdaSqlBuilder;
using Dapper.LambdaExtension.LambdaSqlBuilder.Adapter;
using Dapper.LambdaExtension.LambdaSqlBuilder.Entity;

using Dapper;

namespace Dapper.LambdaExtension.Extentions
{
    public static partial class DapperLambdaExt
    {

        //private static string LastSql { get; set; }

        public static bool PrintSql { get; set; }

        internal static ConcurrentBag<string> LastSqlStack { get; set; }

        static DapperLambdaExt()
        {
            //PreApplicationStart.RegisterTypeMaps();    
        }

#region  base methods
        private static void AddSql(string sql)
        {
            Task.Run(() =>
            {
                if (LastSqlStack == null)
                {
                    LastSqlStack = new ConcurrentBag<string>();
                }

                if (LastSqlStack.Count >= 5)
                {
                    LastSqlStack = new ConcurrentBag<string>();
                }

                LastSqlStack.Add(sql);
            });
        }

        public static string GetLastSql()
        {
            var sql = LastSqlStack.LastOrDefault();
            return sql;
        }
        internal static void DebuggingSqlString(string sqlString)
        {
            AddSql(sqlString);
            if (PrintSql)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine(sqlString);
                }
            }
        }
        internal static void DebuggingException(Exception ex, string sqlString)
        {

            if (Debugger.IsAttached)
            {
                Debug.WriteLine(ex.Message + ex.StackTrace);
                Debug.WriteLine(sqlString);
            }

            Console.WriteLine(ex.Message + ex.StackTrace);
            Console.WriteLine(sqlString);
        }


        public static string GetParameterString(IDictionary<string, object> dic)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in dic)
            {
                sb.AppendFormat("Key: {0}, Value: {1}", item.Key, item.Value);
                sb.AppendLine();
            }
            return sb.ToString();
        }

        #endregion


        #region Insert


        public static int Insert<T>(this IDbConnection con, T entity, IDbTransaction trans = null, int? commandTimeout = null)
        {
            var db = trans == null ? con : trans.Connection;
            var sqllam = new SqlExp<T>(db.GetAdapter());

            sqllam = sqllam.Insert();
            var sqlString = sqllam.SqlString;
            try
            {
                DebuggingSqlString(sqlString);
                return db.Execute(sqlString, entity, trans, commandTimeout, CommandType.Text);
            }
            catch (Exception ex)
            {
                DebuggingException(ex, sqlString);
                throw new DapperLamException(ex.Message, ex, sqlString) { Parameters = sqllam.Parameters };
            }
        }

        public static int Insert(this IDbConnection con, SqlTableDefine tableDefine, List<SqlColumnDefine> columnDefines, IEnumerable<object> entity, IDbTransaction trans = null, int? commandTimeout = null)
        {
            var db = trans == null ? con : trans.Connection;
            var sqllam = new SqlExp<object>(tableDefine, columnDefines, db.GetAdapter());

            sqllam = sqllam.Insert(tableDefine, columnDefines);
            var sqlString = sqllam.SqlString;
            try
            {

                DebuggingSqlString(sqlString);
                return db.Execute(sqlString, entity, trans, commandTimeout, CommandType.Text);
            }
            catch (Exception ex)
            {
                DebuggingException(ex, sqlString);
                throw new DapperLamException(ex.Message, ex, sqlString) { Parameters = sqllam.Parameters };
            }
        }

        public static int InsertList<T>(this IDbConnection con, IEnumerable<T> entitys, IDbTransaction trans = null, int? commandTimeout = null)
        {
            var db = trans == null ? con : trans.Connection;
            if (!entitys.Any())
            {
                return 0;
            }

            var sqllam = new SqlExp<T>(db.GetAdapter());

            sqllam = sqllam.Insert();
            var sqlString = sqllam.SqlString;
            try
            {
                DebuggingSqlString(sqlString);

                return db.Execute(sqlString, entitys, trans, commandTimeout, CommandType.Text);
            }
            catch (Exception ex)
            {
                DebuggingException(ex, sqlString);
                throw new DapperLamException(ex.Message, ex, sqlString) { Parameters = sqllam.Parameters };
            }
        }

        #endregion

        #region Update


        public static int Update<T>(this IDbConnection con, T entity, IDbTransaction trans = null, int? commandTimeout = null)
        {
            var db = trans == null ? con : trans.Connection;

            var sqllam = new SqlExp<T>(db.GetAdapter());

 
            sqllam = sqllam.Update();
            var sqlString = sqllam.SqlString;
            try
            {
                DebuggingSqlString(sqlString);
                return db.Execute(sqlString, entity, trans, commandTimeout, CommandType.Text);
            }
            catch (Exception ex)
            {
                DebuggingException(ex, sqlString);
                throw new DapperLamException(ex.Message, ex, sqlString) { Parameters = sqllam.Parameters };
            }
        }
        public static int Update<T>(this IDbConnection con, T entity, Expression<Func<T,bool>> whereExpression, IDbTransaction trans = null, int? commandTimeout = null)
        {
            var db = trans == null ? con : trans.Connection;

            var sqllam = new SqlExp<T>(db.GetAdapter());
            
            sqllam = sqllam.Update();

            if (whereExpression != null)
            {
                sqllam.Where(whereExpression);
            }

            var sqlString = sqllam.SqlString;
            try
            {
                DebuggingSqlString(sqlString);
                return db.Execute(sqlString, entity, trans, commandTimeout, CommandType.Text);
            }
            catch (Exception ex)
            {
                DebuggingException(ex, sqlString);
                throw new DapperLamException(ex.Message, ex, sqlString) { Parameters = sqllam.Parameters };
            }
        }

        public static int Update<T>(this IDbConnection con,Action<SqlExp<T>> action, IDbTransaction trans = null, int? commandTimeout = null )
        {
            var db = trans == null ? con : trans.Connection;

            var sqllam = new SqlExp<T>(db.GetAdapter());

           action?.Invoke(sqllam);

            
            var sqlString = sqllam.SqlString;
            try
            {
                DebuggingSqlString(sqlString);
                return db.Execute(sqlString, sqllam.Parameters, trans, commandTimeout, CommandType.Text);
            }
            catch (Exception ex)
            {
                DebuggingException(ex, sqlString);
                throw new DapperLamException(ex.Message, ex, sqlString) { Parameters = sqllam.Parameters };
            }
        }


        public static int UpdateList<T>(this IDbConnection con, IEnumerable<T> entitys, IDbTransaction trans = null, int? commandTimeout = null)
        {
            var db = trans == null ? con : trans.Connection;
            if (!entitys.Any())
            {
                return 0;
            }

            var sqllam = new SqlExp<T>(db.GetAdapter());


            sqllam = sqllam.Update();
            var sqlString = sqllam.SqlString;
            try
            {

                DebuggingSqlString(sqlString);
                return db.Execute(sqlString, entitys, trans, commandTimeout, CommandType.Text);
            }
            catch (Exception ex)
            {
                DebuggingException(ex, sqlString);
                throw new DapperLamException(ex.Message, ex, sqlString) { Parameters = sqllam.Parameters };
            }
        }

        #endregion


        #region Delete

        public static int Delete<T>(this IDbConnection con, T entity, IDbTransaction trans = null, int? commandTimeout = null)
        {
            var db = trans == null ? con : trans.Connection;
            var sqllam = new SqlExp<T>(db.GetAdapter());


            sqllam = sqllam.Delete();
            var sqlString = sqllam.SqlString;
            try
            {

                DebuggingSqlString(sqlString);
                return db.Execute(sqlString, entity, trans, commandTimeout, CommandType.Text);
            }
            catch (Exception ex)
            {
                DebuggingException(ex, sqlString);
                throw new DapperLamException(ex.Message, ex, sqlString) { Parameters = sqllam.Parameters };
            }
        }

        public static int Delete<T>(this IDbConnection con, Expression<Func<T, bool>> deleteExpression, IDbTransaction trans = null, int? commandTimeout = null)
        {
            var db = trans == null ? con : trans.Connection;
            if (deleteExpression == null)
            {
                throw new Exception("delete expression is null!");
            }

            var sqllam = new SqlExp<T>(db.GetAdapter());


            sqllam = sqllam.Delete(deleteExpression);
            var sqlString = sqllam.SqlString;
            try
            {

                DebuggingSqlString(sqlString);
                return db.Execute(sqlString, sqllam.Parameters, trans, commandTimeout, CommandType.Text);
            }
            catch (Exception ex)
            {
                DebuggingException(ex, sqlString);
                throw new DapperLamException(ex.Message, ex, sqlString) { Parameters = sqllam.Parameters };
            }
        }

        public static int Delete<T>(this IDbConnection con, Action<SqlExp<T>> action, IDbTransaction trans = null, int? commandTimeout = null)
        {
            var db = trans == null ? con : trans.Connection;
            var sqllam = new SqlExp<T>(db.GetAdapter());

            sqllam = sqllam.Delete();

            action?.Invoke(sqllam);
            var sqlString = sqllam.SqlString;
            try
            {

                DebuggingSqlString(sqlString);
                return db.Execute(sqlString, sqllam.Parameters, trans, commandTimeout, CommandType.Text);
            }
            catch (Exception ex)
            {
                DebuggingException(ex, sqlString);
                throw new DapperLamException(ex.Message, ex, sqllam.SqlString) { Parameters = sqllam.Parameters };
            }
        }

        public static int DeleteList<T>(this IDbConnection con, IEnumerable<T> entityList, IDbTransaction trans = null, int? commandTimeout = null)
        {
            var db = trans == null ? con : trans.Connection;
            if (!entityList.Any())
            {
                return 0;
            }

            var sqllam = new SqlExp<T>(db.GetAdapter());


            sqllam = sqllam.Delete();
            var sqlString = sqllam.SqlString;
            try
            {

                DebuggingSqlString(sqlString);
                return db.Execute(sqlString, entityList, trans, commandTimeout, CommandType.Text);
            }
            catch (Exception ex)
            {
                DebuggingException(ex, sqlString);
                throw new DapperLamException(ex.Message, ex, sqlString) { Parameters = sqllam.Parameters };
            }
        }

        #endregion


        #region Query

        public static T QueryFirstOrDefault<T>(this IDbConnection con, Expression<Func<T, bool>> wherExpression = null, IDbTransaction trans = null, int? commandTimeout = null,string specialSchema =null)
        {
            var db = trans == null ? con : trans.Connection;
            var sqllam = new SqlExp<T>(db.GetAdapter(),specialSchema:specialSchema);

            if (wherExpression != null)
            {
                sqllam = sqllam.Where(wherExpression);
            }

            var sqlString = sqllam.SqlString;
            try
            {
                DebuggingSqlString(sqlString);
                return db.QueryFirstOrDefault<T>(sqlString, sqllam.Parameters, trans, commandTimeout: commandTimeout);
            }
            catch (Exception ex)
            {
                DebuggingException(ex, sqlString);
                throw new DapperLamException(ex.Message, ex, sqlString) { Parameters = sqllam.Parameters };
            }
        }

        public static T QueryFirstOrDefault<T>(this IDbConnection con, Action<SqlExp<T>> action, IDbTransaction trans = null, int? commandTimeout = null,string specialSchema =null)
        {
            var db = trans == null ? con : trans.Connection;
            var sqllam = new SqlExp<T>(db.GetAdapter(),specialSchema:specialSchema);


            action?.Invoke(sqllam);

            var sqlString = sqllam.SqlString;
            try
            {
                DebuggingSqlString(sqlString);
                return db.QueryFirstOrDefault<T>(sqlString, sqllam.Parameters, trans, commandTimeout: commandTimeout);
            }
            catch (Exception ex)
            {
                DebuggingException(ex, sqlString);
                throw new DapperLamException(ex.Message, ex, sqlString) { Parameters = sqllam.Parameters };
            }
        }

        public static TResult QueryFirstOrDefault<TEntity, TResult>(this IDbConnection con, Action<SqlExp<TEntity>> action = null,
            IDbTransaction trans = null, int? commandTimeout = null,string specialSchema =null) where TEntity : class
        {
            var db = trans == null ? con : trans.Connection;
            var sqllam = new SqlExp<TEntity>(db.GetAdapter(),specialSchema:specialSchema);

            var sqlString = string.Empty;
           
            try
            {
                action?.Invoke(sqllam);
                sqlString = sqllam.SqlString;
                DebuggingSqlString(sqlString);
                return db.QueryFirstOrDefault<TResult>(sqlString, sqllam.Parameters, trans, commandTimeout: commandTimeout);
            }
            catch (Exception ex)
            {
                DebuggingException(ex, sqlString);
                throw new DapperLamException(ex.Message, ex, sqlString) { Parameters = sqllam.Parameters };
            }
        }


        public static IEnumerable<T> Query<T>(this IDbConnection con, Action<SqlExp<T>> action, IDbTransaction trans = null, int? commandTimeout = null,string specialSchema =null)
        {
            var db = trans == null ? con : trans.Connection;
            var sqllam = new SqlExp<T>(db.GetAdapter(),specialSchema:specialSchema);

            var sqlString = string.Empty;
           
            try
            {
                action?.Invoke(sqllam);

                sqlString = sqllam.SqlString;
                DebuggingSqlString(sqlString);
                return db.Query<T>(sqlString, sqllam.Parameters, trans, commandTimeout: commandTimeout);
            }
            catch (Exception ex)
            {
                DebuggingException(ex, sqlString);
                throw new DapperLamException(ex.Message, ex, sqlString) { Parameters = sqllam.Parameters };
            }

        }
        public static IEnumerable<T> Query<T>(this IDbConnection con, Expression<Func<T, bool>> wherExpression = null, IDbTransaction trans = null, int? commandTimeout = null,string specialSchema =null)
        {
            var db = trans == null ? con : trans.Connection;
            var sqllam = new SqlExp<T>(db.GetAdapter(),specialSchema:specialSchema);

            var sqlString = string.Empty;

            
            try
            {if (wherExpression != null)
                {
                    sqllam = sqllam.Where(wherExpression);
                }

                  sqlString = sqllam.SqlString;
                DebuggingSqlString(sqlString);

                return db.Query<T>(sqlString, sqllam.Parameters, trans, commandTimeout: commandTimeout);
            }
            catch (Exception ex)
            {
                DebuggingException(ex, sqlString);
                throw new DapperLamException(ex.Message, ex, sqlString) { Parameters = sqllam.Parameters };
            }
        }
        public static IEnumerable<TResult> Query<TEntity, TResult>(this IDbConnection con, Action<SqlExp<TEntity>> action = null,
            IDbTransaction trans = null, int? commandTimeout = null,string specialSchema =null) where TEntity : class
        {
            var db = trans == null ? con : trans.Connection;
            var sqllam = new SqlExp<TEntity>(db.GetAdapter(),specialSchema:specialSchema);
            var sqlString = string.Empty;
          
            try
            {  action?.Invoke(sqllam);
                  sqlString = sqllam.SqlString;

                DebuggingSqlString(sqlString);
                return db.Query<TResult>(sqlString, sqllam.Parameters, trans, commandTimeout: commandTimeout);
            }
            catch (Exception ex)
            {
                DebuggingException(ex, sqlString);
                throw new DapperLamException(ex.Message, ex, sqlString) { Parameters = sqllam.Parameters };
            }
        }

        public static IEnumerable<TResult> Query<TEntity, TResult>(this IDbConnection con, Action<SqlExp<TResult>> action, Action<SqlExp<TEntity>> subAction, IDbTransaction trans = null, int? commandTimeout = null,string specialSchema =null) where TEntity : class
        {
            var db = trans == null ? con : trans.Connection;
            var sqllamSub = new SqlExp<TEntity>(db.GetAdapter(),specialSchema:specialSchema);

            //action?.Invoke(sqllam);
            var sqlString = string.Empty;
            var sqlLamMain = new SqlExp<TResult>(db.GetAdapter(),specialSchema:specialSchema);
            try
            {
                subAction?.Invoke(sqllamSub);

                //sqllam.SubQuery(action);

            

                sqlLamMain.SubQuery(sqllamSub);

                action?.Invoke(sqlLamMain);
                  sqlString = sqlLamMain.SqlString;

                DebuggingSqlString(sqlString);
                return db.Query<TResult>(sqlString, sqlLamMain.Parameters, trans, commandTimeout: commandTimeout);
            }
            catch (Exception ex)
            {
                DebuggingException(ex, sqlString);
                throw new DapperLamException(ex.Message, ex, sqlString) { Parameters = sqlLamMain.Parameters };
            }
        }

        #endregion


        #region  Paged Query

        

        public static PagedResult<T> PagedQuery<T>(this IDbConnection con, int pageSize, int pageNumber,
            Expression<Func<T, bool>> whereExpression = null, Expression<Func<T, object>> groupByexpression = null,
            IDbTransaction trans = null, int? commandTimeout = null,
            Expression<Func<T, object>> orderbyExpression = null,string specialSchema =null)
            where T : class
        {
            var db = trans == null ? con : trans.Connection;
            var sqllam = new SqlExp<T>(db.GetAdapter(),specialSchema:specialSchema);
            var countSqlam = new SqlExp<T>(db.GetAdapter(),specialSchema:specialSchema);
            if (whereExpression != null)
            {
                sqllam = sqllam.Where(whereExpression);
                countSqlam = countSqlam.Where(whereExpression);
            }

            if (orderbyExpression != null)
            {
                sqllam = sqllam.OrderBy(orderbyExpression);
            }

            if (groupByexpression != null)
            {
                sqllam = sqllam.GroupBy(groupByexpression);
            }

            countSqlam = countSqlam.Count();

            int countRet;
            var sqlString = countSqlam.SqlString;
            try
            {

                DebuggingSqlString(sqlString);
                countRet = db.Query<int>(sqlString, countSqlam.Parameters).FirstOrDefault();
            }
            catch (Exception ex)
            {
                DebuggingException(ex, sqlString);
                throw new DapperLamException(ex.Message, ex, sqlString) { Parameters = countSqlam.Parameters };
            }
            var sqlstring = sqllam.QueryPage(pageSize, pageNumber);

            try
            {
                DebuggingSqlString(sqlstring);
                var retlist = db.Query<T>(sqlstring, sqllam.Parameters, trans, commandTimeout: commandTimeout);

                return new PagedResult<T>(retlist, countRet, pageSize, pageNumber);
            }
            catch (Exception ex)
            {
                DebuggingException(ex, sqlstring);
                throw new DapperLamException(ex.Message, ex, sqlstring) { Parameters = sqllam.Parameters };
            }

        }

       

        public static PagedResult<T> PagedQuery<T>(this IDbConnection con, int pageSize, int pageNumber, Action<SqlExp<T>> action, IDbTransaction trans = null, int? commandTimeout = null,string specialSchema =null) where T : class
        {
            var db = trans == null ? con : trans.Connection;
            var sqllam = new SqlExp<T>(db.GetAdapter(),specialSchema:specialSchema);

            var countSqlam = new SqlExp<T>(db.GetAdapter(), true,specialSchema:specialSchema);

            action?.Invoke(sqllam);

            action?.Invoke(countSqlam);

            countSqlam = countSqlam.Count();

            int countRet;

            var sqlString = countSqlam.SqlString;
            try
            {
                DebuggingSqlString(sqlString);
                countRet = db.Query<int>(sqlString, countSqlam.Parameters, trans, commandTimeout: commandTimeout).FirstOrDefault();
            }
            catch (Exception ex)
            {
                DebuggingException(ex, sqlString);
                throw new DapperLamException(ex.Message, ex, countSqlam.SqlString) { Parameters = countSqlam.Parameters };
            }
            var sqlstring = sqllam.QueryPage(pageSize, pageNumber);

            try
            {
                DebuggingSqlString(sqlstring);
                var retlist = db.Query<T>(sqlstring, sqllam.Parameters, trans, commandTimeout: commandTimeout);
                return new PagedResult<T>(retlist, countRet, pageSize, pageNumber);
            }
            catch (Exception ex)
            {
                DebuggingException(ex, sqlstring);
                throw new DapperLamException(ex.Message, ex, sqlstring) { Parameters = sqllam.Parameters };
            }
        }


        public static PagedResult<TResult> PagedQuery<T, TResult>(this IDbConnection con, int pageSize, int pageNumber, Action<SqlExp<T>> action, IDbTransaction trans = null, int? commandTimeout = null,string specialSchema =null) where T : class where TResult : class
        {
            var db = trans == null ? con : trans.Connection;
            var sqllam = new SqlExp<T>(db.GetAdapter(),specialSchema:specialSchema);

            var countSqlam = new SqlExp<T>(db.GetAdapter(), true,specialSchema:specialSchema);

            action?.Invoke(sqllam);

            action?.Invoke(countSqlam);

            countSqlam = countSqlam.Count();

            int countRet;

            var sqlString = countSqlam.SqlString;
            try
            {

                DebuggingSqlString(sqlString);
                countRet = db.Query<int>(countSqlam.SqlString, countSqlam.Parameters, trans, commandTimeout: commandTimeout).FirstOrDefault();
            }
            catch (Exception ex)
            {
                DebuggingException(ex, sqlString);
                throw new DapperLamException(ex.Message, ex, sqlString) { Parameters = countSqlam.Parameters };
            }

            var sqlstring = sqllam.QueryPage(pageSize, pageNumber);

            try
            {
                DebuggingSqlString(sqlstring);
                var retlist = db.Query<TResult>(sqlstring, sqllam.Parameters, trans, commandTimeout: commandTimeout);
                return new PagedResult<TResult>(retlist, countRet, pageSize, pageNumber);
            }
            catch (Exception ex)
            {
                DebuggingException(ex, sqlstring);
                throw new DapperLamException(ex.Message, ex, sqlstring) { Parameters = sqllam.Parameters };
            }
        }


        public static PagedResult<TResult> PagedQuery<TEntity, TResult>(this IDbConnection con, int pageSize, int pageNumber, Action<SqlExp<TResult>> action, Action<SqlExp<TEntity>> subAction,
            IDbTransaction trans = null, int? commandTimeout = null,string specialSchema =null) where TEntity : class where TResult : class
        {
            var db = trans == null ? con : trans.Connection;
            var sqllam = new SqlExp<TEntity>(db.GetAdapter(),specialSchema:specialSchema);

            subAction?.Invoke(sqllam);


            var sqlLamMain = new SqlExp<TResult>(db.GetAdapter(),specialSchema:specialSchema);

            sqlLamMain.SubQuery(sqllam);

            action?.Invoke(sqlLamMain);

            var countSqlam = new SqlExp<TResult>(db.GetAdapter(), true,specialSchema:specialSchema);

            countSqlam.SubQuery(sqllam);

            action?.Invoke(countSqlam);

            countSqlam = countSqlam.CountSubQuery<TResult>(sqlLamMain);
            int countRet = 0;
            var sqlString = countSqlam.SqlString;
            try
            {
                DebuggingSqlString(sqlString);
                countRet = db.Query<int>(sqlString, countSqlam.Parameters, trans, commandTimeout: commandTimeout).FirstOrDefault();

            }
            catch (Exception ex)
            {
                DebuggingException(ex, sqlString);
                throw new DapperLamException(ex.Message, ex, countSqlam.SqlString) { Parameters = countSqlam.Parameters };
            }

            var sqlstring = sqlLamMain.QuerySubPage(pageSize, pageNumber);
            try
            {
                DebuggingSqlString(sqlstring);
                var retlist = db.Query<TResult>(sqlstring, sqlLamMain.Parameters, trans, commandTimeout: commandTimeout);
                return new PagedResult<TResult>(retlist, countRet, pageSize, pageNumber);
            }
            catch (Exception ex)
            {
                DebuggingException(ex, sqlstring);
                throw new DapperLamException(ex.Message, ex, sqlstring) { Parameters = sqlLamMain.Parameters };
            }
        }


        #endregion


        #region  Execution Sql

        


        public static TResult ExecuteScalar<TEntity, TResult>(this IDbConnection con, Action<SqlExp<TEntity>> action = null,
          IDbTransaction trans = null, int? commandTimeout = null,string specialSchema =null) where TEntity : class
        {
            var db = trans == null ? con : trans.Connection;
            var sqllam = new SqlExp<TEntity>(db.GetAdapter(),specialSchema:specialSchema);

            action?.Invoke(sqllam);
            var sqlString = sqllam.SqlString;
            try
            {

                DebuggingSqlString(sqlString);
                return db.ExecuteScalar<TResult>(sqlString, sqllam.Parameters, trans, commandTimeout);
            }
            catch (Exception ex)
            {
                DebuggingException(ex, sqlString);
                throw new DapperLamException(ex.Message, ex, sqlString) { Parameters = sqllam.Parameters };
            }
        }


        /// <summary>
        /// //扩展方法,为了不缓存要执行的SQL语句,比如大量的拼接插入values类语句,如果要缓存的话,是会造成内存一直增长的问题,使用:flag:Nocache,之后,可避免缓存
        /// </summary>
        /// <param name="con"></param>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandType"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        public static int ExecuteNoCache(this IDbConnection con, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null, CommandFlags flag = CommandFlags.Buffered)
        {
            var db = transaction == null ? con : transaction.Connection;
            CommandDefinition command = new CommandDefinition(sql, param, transaction, commandTimeout, commandType, flag, new CancellationToken());

            return db.Execute(command);
        }


        #endregion
    }
}
