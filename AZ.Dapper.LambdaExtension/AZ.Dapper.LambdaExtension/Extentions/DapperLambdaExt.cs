using System;
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
    public static class DapperLambdaExt
    {

        //private static string LastSql { get; set; }

        public static bool PrintSql { get; set; }

        internal static ConcurrentBag<string> LastSqlStack { get; set; }

        static DapperLambdaExt()
        {
            //PreApplicationStart.RegisterTypeMaps();    
        }


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
        public static IEnumerable<T> Query<T>(this IDbConnection db, Expression<Func<T, bool>> wherExpression = null, IDbTransaction trans = null, int? commandTimeout = null)
        {
            var sqllam = new SqlExp<T>(db.GetAdapter());

            if (wherExpression != null)
            {
                sqllam = sqllam.Where(wherExpression);
            }

            var sqlString = sqllam.SqlString;
            try
            {
                DebuggingSqlString(sqlString);

                return db.Query<T>(sqlString, sqllam.Parameters, trans, commandTimeout: commandTimeout);
            }
            catch (Exception ex)
            {
                DebuggingException(ex, sqlString);
                throw new DapperLamException(ex.Message, ex, sqlString) { Parameters = sqllam.Parameters };
            }
        }

        public static Task<IEnumerable<T>> QueryAsync<T>(this IDbConnection db, Expression<Func<T, bool>> wherExpression = null, IDbTransaction trans = null, int? commandTimeout = null)
        {
           return Task.Run(() =>
           {
               return db.Query<T>(wherExpression,trans,commandTimeout);
           });
        }


        public static T QueryFirstOrDefault<T>(this IDbConnection db, Expression<Func<T, bool>> wherExpression = null, IDbTransaction trans = null, int? commandTimeout = null)
        {

            var sqllam = new SqlExp<T>(db.GetAdapter());

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

        public static Task<T> QueryFirstOrDefaultAsync<T>(this IDbConnection db, Expression<Func<T, bool>> wherExpression = null,
            IDbTransaction trans = null, int? commandTimeout = null)
        {
            return Task.Run(() =>
            {
                return db.QueryFirstOrDefault<T>(wherExpression, trans, commandTimeout);
            });
        }

        public static int Insert<T>(this IDbConnection db, T entity, IDbTransaction trans = null, int? commandTimeout = null)
        {
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

        public static Task<int> InsertAsync<T>(this IDbConnection db, T entity, IDbTransaction trans = null,
            int? commandTimeout = null)
        {
            return Task.Run(() =>
            {
                return db.Insert<T>(entity, trans, commandTimeout);
            });
        }

        public static int Insert(this IDbConnection db, SqlTableDefine tableDefine, List<SqlColumnDefine> columnDefines, IEnumerable<object> entity, IDbTransaction trans = null, int? commandTimeout = null)
        {

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

        public static Task<int> InsertAsync(this IDbConnection db, SqlTableDefine tableDefine, List<SqlColumnDefine> columnDefines,
            IEnumerable<object> entity, IDbTransaction trans = null, int? commandTimeout = null)
        {
            return Task.Run(() =>
            {
                return db.Insert(tableDefine,columnDefines,entity, trans, commandTimeout);
            });
        }

        public static int InsertList<T>(this IDbConnection db, IEnumerable<T> entitys, IDbTransaction trans = null, int? commandTimeout = null)
        {
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

        public static Task<int> InsertListAsync<T>(this IDbConnection db, IEnumerable<T> entitys, IDbTransaction trans = null, int? commandTimeout = null)
        {
            return Task.Run(() =>
            {
                return db.InsertList<T>(entitys,  trans, commandTimeout);
            });
        }

        public static int Update<T>(this IDbConnection db, T entity, IDbTransaction trans = null, int? commandTimeout = null)
        {

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

        public static Task<int> UpdateAsync<T>(this IDbConnection db, T entity, IDbTransaction trans = null,
            int? commandTimeout = null)
        {
            return Task.Run(() =>
            {
                return db.Update<T>(entity, trans, commandTimeout);
            });
        }



        public static int UpdateList<T>(this IDbConnection db, IEnumerable<T> entitys, IDbTransaction trans = null, int? commandTimeout = null)
        {
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

        public static Task<int> UpdateListAsync<T>(this IDbConnection db, IEnumerable<T> entitys, IDbTransaction trans = null,
            int? commandTimeout = null)
        {
            return Task.Run(() =>
            {
                return db.UpdateList<T>(entitys, trans, commandTimeout);
            });
        }

        public static int DeleteList<T>(this IDbConnection db, IEnumerable<T> engityList, IDbTransaction trans = null, int? commandTimeout = null)
        {
            if (!engityList.Any())
            {
                return 0;
            }

            var sqllam = new SqlExp<T>(db.GetAdapter());


            sqllam = sqllam.Delete();
            var sqlString = sqllam.SqlString;
            try
            {

                DebuggingSqlString(sqlString);
                return db.Execute(sqlString, engityList, trans, commandTimeout, CommandType.Text);
            }
            catch (Exception ex)
            {
                DebuggingException(ex, sqlString);
                throw new DapperLamException(ex.Message, ex, sqlString) { Parameters = sqllam.Parameters };
            }
        }

        public static Task<int> DeleteListAsync<T>(this IDbConnection db, IEnumerable<T> engityList, IDbTransaction trans = null,
            int? commandTimeout = null)
        {
            return Task.Run(() =>
            {
                return db.DeleteList<T>(engityList, trans, commandTimeout);
            });
        }

        public static int Delete<T>(this IDbConnection db, T engity, IDbTransaction trans = null, int? commandTimeout = null)
        {

            var sqllam = new SqlExp<T>(db.GetAdapter());


            sqllam = sqllam.Delete();
            var sqlString = sqllam.SqlString;
            try
            {

                DebuggingSqlString(sqlString);
                return db.Execute(sqlString, engity, trans, commandTimeout, CommandType.Text);
            }
            catch (Exception ex)
            {
                DebuggingException(ex, sqlString);
                throw new DapperLamException(ex.Message, ex, sqlString) { Parameters = sqllam.Parameters };
            }
        }

        public static Task<int> DeleteAsync<T>(this IDbConnection db, T engity, IDbTransaction trans = null,
            int? commandTimeout = null)
        {

            return Task.Run(() =>
            {
                return db.Delete<T>(engity, trans, commandTimeout);
            });
        }

        public static int Delete<T>(this IDbConnection db, Expression<Func<T, bool>> deleteExpression, IDbTransaction trans = null, int? commandTimeout = null)
        {
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

        public static Task<int> DeleteAsync<T>(this IDbConnection db, Expression<Func<T, bool>> deleteExpression,
            IDbTransaction trans = null, int? commandTimeout = null)
        {
            return Task.Run(() =>
            {
                return db.Delete<T>(deleteExpression, trans, commandTimeout);
            });
        }

        public static int Delete<T>(this IDbConnection db, Action<SqlExp<T>> action, IDbTransaction trans = null, int? commandTimeout = null)
        {

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

        public static Task<int> DeleteAsync<T>(this IDbConnection db, Action<SqlExp<T>> action, IDbTransaction trans = null,
            int? commandTimeout = null)
        {
            return Task.Run(() =>
            {
                return db.Delete<T>(action, trans, commandTimeout);
            });
        }

        public static PagedResult<T> PagedQuery<T>(this IDbConnection db, int pageSize, int pageNumber,
            Expression<Func<T, bool>> whereExpression = null, Expression<Func<T, object>> groupByexpression = null,
            IDbTransaction trans = null, int? commandTimeout = null,
            Expression<Func<T, object>> orderbyExpression = null)
            where T : class
        {

            var sqllam = new SqlExp<T>(db.GetAdapter());
            var countSqlam = new SqlExp<T>(db.GetAdapter());
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

        public static Task<PagedResult<T>> PagedQueryAsync<T>(this IDbConnection db, int pageSize, int pageNumber,
            Expression<Func<T, bool>> whereExpression = null, Expression<Func<T, object>> groupByexpression = null,
            IDbTransaction trans = null, int? commandTimeout = null,
            Expression<Func<T, object>> orderbyExpression = null)
            where T : class
        {
            return Task.Run(() =>
            {
                return db.PagedQuery<T>(pageSize,pageNumber,whereExpression,groupByexpression, trans, commandTimeout,orderbyExpression);
            });
        }

        public static IEnumerable<T> Query<T>(this IDbConnection db, Action<SqlExp<T>> action, IDbTransaction trans = null, int? commandTimeout = null)
        {

            var sqllam = new SqlExp<T>(db.GetAdapter());

            action?.Invoke(sqllam);

            var sqlString = sqllam.SqlString;
            try
            {

                DebuggingSqlString(sqlString);
                return db.Query<T>(sqlString, sqllam.Parameters, trans, commandTimeout: commandTimeout);
            }
            catch (Exception ex)
            {
                DebuggingException(ex, sqlString);
                throw new DapperLamException(ex.Message, ex, sqlString) { Parameters = sqllam.Parameters };
            }

        }

        public static Task<IEnumerable<T>> QueryAsync<T>(this IDbConnection db, Action<SqlExp<T>> action,
            IDbTransaction trans = null, int? commandTimeout = null)
        {
            return Task.Run(() =>
            {
                return db.Query<T>(action, trans, commandTimeout);
            });
        }

        public static PagedResult<T> PagedQuery<T>(this IDbConnection db, int pageSize, int pageNumber, Action<SqlExp<T>> action, IDbTransaction trans = null, int? commandTimeout = null) where T : class
        {

            var sqllam = new SqlExp<T>(db.GetAdapter());

            var countSqlam = new SqlExp<T>(db.GetAdapter(), true);

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

        public static Task<PagedResult<T>> PagedQueryAsync<T>(this IDbConnection db, int pageSize, int pageNumber,
            Action<SqlExp<T>> action,
            IDbTransaction trans = null, int? commandTimeout = null
            )
            where T : class
        {
            return Task.Run(() =>
            {
                return db.PagedQuery<T>(pageSize, pageNumber, action, trans, commandTimeout );
            });
        }


        public static Task<PagedResult<TResult>> PagedQueryAsync<T, TResult>(this IDbConnection db, int pageSize, int pageNumber,
            Action<SqlExp<T>> action,
            IDbTransaction trans = null, int? commandTimeout = null
        )
            where T : class where TResult : class
        {
            return Task.Run(() =>
            {
                return db.PagedQuery<T,TResult>(pageSize, pageNumber, action, trans, commandTimeout);
            });
        }
        public static PagedResult<TResult> PagedQuery<T, TResult>(this IDbConnection db, int pageSize, int pageNumber, Action<SqlExp<T>> action, IDbTransaction trans = null, int? commandTimeout = null) where T : class where TResult : class
        {

            var sqllam = new SqlExp<T>(db.GetAdapter());

            var countSqlam = new SqlExp<T>(db.GetAdapter(), true);

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



        public static Task<PagedResult<TResult>> PagedQueryAsync<TEntity, TResult>(this IDbConnection db, int pageSize, int pageNumber,
            Action<SqlExp<TResult>> action, Action<SqlExp<TEntity>> subAction,
            IDbTransaction trans = null, int? commandTimeout = null
        )
            where TEntity : class where TResult : class
        {
            return Task.Run(() =>
            {
                return db.PagedQuery<TEntity, TResult>(pageSize, pageNumber, action, subAction,trans, commandTimeout);
            });
        }
        public static PagedResult<TResult> PagedQuery<TEntity, TResult>(this IDbConnection db, int pageSize, int pageNumber, Action<SqlExp<TResult>> action, Action<SqlExp<TEntity>> subAction,
            IDbTransaction trans = null, int? commandTimeout = null) where TEntity : class where TResult : class
        {

            var sqllam = new SqlExp<TEntity>(db.GetAdapter());

            subAction?.Invoke(sqllam);


            var sqlLamMain = new SqlExp<TResult>(db.GetAdapter());

            sqlLamMain.SubQuery(sqllam);

            action?.Invoke(sqlLamMain);

            var countSqlam = new SqlExp<TResult>(db.GetAdapter(), true);

            countSqlam.SubQuery(sqllam);

            action?.Invoke(countSqlam);

            countSqlam = countSqlam.Count<TResult>(sqlLamMain);
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

        public static IEnumerable<TResult> Query<TEntity, TResult>(this IDbConnection db, Action<SqlExp<TEntity>> action = null,
            IDbTransaction trans = null, int? commandTimeout = null) where TEntity : class
        {

            var sqllam = new SqlExp<TEntity>(db.GetAdapter());

            action?.Invoke(sqllam);
            var sqlString = sqllam.SqlString;
            try
            {

                DebuggingSqlString(sqlString);
                return db.Query<TResult>(sqlString, sqllam.Parameters, trans, commandTimeout: commandTimeout);
            }
            catch (Exception ex)
            {
                DebuggingException(ex, sqlString);
                throw new DapperLamException(ex.Message, ex, sqlString) { Parameters = sqllam.Parameters };
            }
        }

        public static Task<IEnumerable<TResult>> QueryAsync<TEntity, TResult>(this IDbConnection db,
            Action<SqlExp<TEntity>> action = null,
            IDbTransaction trans = null, int? commandTimeout = null) where TEntity : class
        {
            return Task.Run(() =>
            {
                return db.Query<TEntity, TResult>(action, trans, commandTimeout);

            });
        }

        public static IEnumerable<TResult> Query<TEntity, TResult>(this IDbConnection db, Action<SqlExp<TResult>> action, Action<SqlExp<TEntity>> subAction, IDbTransaction trans = null, int? commandTimeout = null) where TEntity : class
        {

            var sqllamSub = new SqlExp<TEntity>(db.GetAdapter());



            //action?.Invoke(sqllam);

            subAction?.Invoke(sqllamSub);

            //sqllam.SubQuery(action);

            var sqlLamMain = new SqlExp<TResult>(db.GetAdapter());

            sqlLamMain.SubQuery(sqllamSub);

            action?.Invoke(sqlLamMain);
            var sqlString = sqlLamMain.SqlString;
            try
            {

                DebuggingSqlString(sqlString);
                return db.Query<TResult>(sqlString, sqlLamMain.Parameters, trans, commandTimeout: commandTimeout);
            }
            catch (Exception ex)
            {
                DebuggingException(ex, sqlString);
                throw new DapperLamException(ex.Message, ex, sqlString) { Parameters = sqlLamMain.Parameters };
            }
        }

        public static Task<IEnumerable<TResult>> QueryAsync<TEntity, TResult>(this IDbConnection db,
            Action<SqlExp<TResult>> action, Action<SqlExp<TEntity>> subAction,
            IDbTransaction trans = null, int? commandTimeout = null
        )
            where TEntity : class where TResult : class
        {
            return Task.Run(() =>
            {
                return db.Query<TEntity, TResult>( action, subAction, trans, commandTimeout);
            });
        }


        public static TResult ExecuteScalar<TEntity, TResult>(this IDbConnection db, Action<SqlExp<TEntity>> action = null,
          IDbTransaction trans = null, int? commandTimeout = null) where TEntity : class
        {
            var sqllam = new SqlExp<TEntity>(db.GetAdapter());

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
        /// <param name="cnn"></param>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandType"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        public static int ExecuteNoCache(this IDbConnection cnn, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null, CommandFlags flag = CommandFlags.Buffered)
        {
            CommandDefinition command = new CommandDefinition(sql, param, transaction, commandTimeout, commandType, flag, new CancellationToken());

            return cnn.Execute(command);
        }
    }
}
