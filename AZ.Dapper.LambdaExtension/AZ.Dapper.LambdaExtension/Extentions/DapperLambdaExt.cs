using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;

using System.Text;
using System.Threading;
using Dapper.LambdaExtension.LambdaSqlBuilder;
using Dapper.LambdaExtension.LambdaSqlBuilder.Adapter;
using Dapper.LambdaExtension.LambdaSqlBuilder.Entity;

using Dapper;

namespace Dapper.LambdaExtension.Extentions
{
    public static class DapperLambdaExt
    {
        static DapperLambdaExt()
        {
            //PreApplicationStart.RegisterTypeMaps();    
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
            try
            {
                return db.Query<T>(sqllam.SqlString, sqllam.Parameters, trans, commandTimeout: commandTimeout);
            }
            catch (Exception ex)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine(ex.Message + ex.StackTrace);
                    Debug.WriteLine(sqllam.SqlString);
                }

                Console.WriteLine(ex.Message + ex.StackTrace);
                Console.WriteLine(sqllam.SqlString);
                throw new DapperLamException(ex.Message, ex, sqllam.SqlString) { Parameters = sqllam.Parameters };
            }
        }


        public static T QueryFirstOrDefault<T>(this IDbConnection db, Expression<Func<T, bool>> wherExpression = null, IDbTransaction trans = null, int? commandTimeout = null)
        {

            var sqllam = new SqlExp<T>(db.GetAdapter());

            if (wherExpression != null)
            {
                sqllam = sqllam.Where(wherExpression);
            }
            try
            {
                return db.QueryFirstOrDefault<T>(sqllam.SqlString, sqllam.Parameters, trans, commandTimeout: commandTimeout);
            }
            catch (Exception ex)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine(ex.Message + ex.StackTrace);
                    Debug.WriteLine(sqllam.SqlString);
                }

                Console.WriteLine(ex.Message + ex.StackTrace);
                Console.WriteLine(sqllam.SqlString);
                throw new DapperLamException(ex.Message, ex, sqllam.SqlString) { Parameters = sqllam.Parameters };
            }
        }

        public static int Insert<T>(this IDbConnection db, T entity, IDbTransaction trans = null, int? commandTimeout = null)
        {

            var sqllam = new SqlExp<T>(db.GetAdapter());


            sqllam = sqllam.Insert();

            try
            {
                return db.Execute(sqllam.SqlString, entity, trans, commandTimeout, CommandType.Text);
            }
            catch (Exception ex)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine(ex.Message + ex.StackTrace);
                    Debug.WriteLine(sqllam.SqlString);
                }

                Console.WriteLine(ex.Message + ex.StackTrace);
                Console.WriteLine(sqllam.SqlString);
                throw new DapperLamException(ex.Message, ex, sqllam.SqlString) { Parameters = sqllam.Parameters };
            }
        }

        public static int Insert(this IDbConnection db, SqlTableDefine tableDefine, List<SqlColumnDefine> columnDefines, IEnumerable<object> entity, IDbTransaction trans = null, int? commandTimeout = null)
        {

            var sqllam = new SqlExp<object>(tableDefine, columnDefines, db.GetAdapter());

            sqllam = sqllam.Insert(tableDefine, columnDefines);
            try
            {
                return db.Execute(sqllam.SqlString, entity, trans, commandTimeout, CommandType.Text);
            }
            catch (Exception ex)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine(ex.Message + ex.StackTrace);
                    Debug.WriteLine(sqllam.SqlString);
                }

                Console.WriteLine(ex.Message + ex.StackTrace);
                Console.WriteLine(sqllam.SqlString);
                throw new DapperLamException(ex.Message, ex, sqllam.SqlString) { Parameters = sqllam.Parameters };
            }
        }

        public static int InsertList<T>(this IDbConnection db, IEnumerable<T> entitys, IDbTransaction trans = null, int? commandTimeout = null)
        {
            if (!entitys.Any())
            {
                return 0;
            }

            var sqllam = new SqlExp<T>(db.GetAdapter());


            sqllam = sqllam.Insert();
            try
            {
                return db.Execute(sqllam.SqlString, entitys, trans, commandTimeout, CommandType.Text);
            }
            catch (Exception ex)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine(ex.Message + ex.StackTrace);
                    Debug.WriteLine(sqllam.SqlString);
                }

                Console.WriteLine(ex.Message + ex.StackTrace);
                Console.WriteLine(sqllam.SqlString);
                throw new DapperLamException(ex.Message, ex, sqllam.SqlString) { Parameters = sqllam.Parameters };
            }
        }
        public static int Update<T>(this IDbConnection db, T entity, IDbTransaction trans = null, int? commandTimeout = null)
        {

            var sqllam = new SqlExp<T>(db.GetAdapter());


            sqllam = sqllam.Update();

            try
            {
                return db.Execute(sqllam.SqlString, entity, trans, commandTimeout, CommandType.Text);
            }
            catch (Exception ex)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine(ex.Message + ex.StackTrace);
                    Debug.WriteLine(sqllam.SqlString);
                }

                Console.WriteLine(ex.Message + ex.StackTrace);
                Console.WriteLine(sqllam.SqlString);
                throw new DapperLamException(ex.Message, ex, sqllam.SqlString) { Parameters = sqllam.Parameters };
            }
        }




        public static int UpdateList<T>(this IDbConnection db, IEnumerable<T> entitys, IDbTransaction trans = null, int? commandTimeout = null)
        {
            if (!entitys.Any())
            {
                return 0;
            }

            var sqllam = new SqlExp<T>(db.GetAdapter());


            sqllam = sqllam.Update();

            try
            {
                return db.Execute(sqllam.SqlString, entitys, trans, commandTimeout, CommandType.Text);
            }
            catch (Exception ex)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine(ex.Message + ex.StackTrace);
                    Debug.WriteLine(sqllam.SqlString);
                }

                Console.WriteLine(ex.Message + ex.StackTrace);
                Console.WriteLine(sqllam.SqlString);
                throw new DapperLamException(ex.Message, ex, sqllam.SqlString) { Parameters = sqllam.Parameters };
            }
        }



        public static int DeleteList<T>(this IDbConnection db, IEnumerable<T> engityList, IDbTransaction trans = null, int? commandTimeout = null)
        {
            if (!engityList.Any())
            {
                return 0;
            }

            var sqllam = new SqlExp<T>(db.GetAdapter());


            sqllam = sqllam.Delete();

            try
            {
                return db.Execute(sqllam.SqlString, engityList, trans, commandTimeout, CommandType.Text);
            }
            catch (Exception ex)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine(ex.Message + ex.StackTrace);
                    Debug.WriteLine(sqllam.SqlString);
                }

                Console.WriteLine(ex.Message + ex.StackTrace);
                Console.WriteLine(sqllam.SqlString);
                throw new DapperLamException(ex.Message, ex, sqllam.SqlString) { Parameters = sqllam.Parameters };
            }
        }



        public static int Delete<T>(this IDbConnection db, T engity, IDbTransaction trans = null, int? commandTimeout = null)
        {

            var sqllam = new SqlExp<T>(db.GetAdapter());


            sqllam = sqllam.Delete();
            try
            {
                return db.Execute(sqllam.SqlString, engity, trans, commandTimeout, CommandType.Text);
            }
            catch (Exception ex)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine(ex.Message + ex.StackTrace);
                    Debug.WriteLine(sqllam.SqlString);
                }

                Console.WriteLine(ex.Message + ex.StackTrace);
                Console.WriteLine(sqllam.SqlString);
                throw new DapperLamException(ex.Message, ex, sqllam.SqlString) { Parameters = sqllam.Parameters };
            }
        }

        public static int Delete<T>(this IDbConnection db, Expression<Func<T, bool>> deleteExpression, IDbTransaction trans = null, int? commandTimeout = null)
        {
            if (deleteExpression == null)
            {
                throw new Exception("delete expression is null!");
            }

            var sqllam = new SqlExp<T>(db.GetAdapter());


            sqllam = sqllam.Delete(deleteExpression);

            try
            {
                return db.Execute(sqllam.SqlString, sqllam.Parameters, trans, commandTimeout, CommandType.Text);
            }
            catch (Exception ex)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine(ex.Message + ex.StackTrace);
                    Debug.WriteLine(sqllam.SqlString);
                }

                Console.WriteLine(ex.Message + ex.StackTrace);
                Console.WriteLine(sqllam.SqlString);
                throw new DapperLamException(ex.Message, ex, sqllam.SqlString) { Parameters = sqllam.Parameters };
            }
        }

        public static int Delete<T>(this IDbConnection db, Action<SqlExp<T>> action, IDbTransaction trans = null, int? commandTimeout = null)
        {

            var sqllam = new SqlExp<T>(db.GetAdapter());

            sqllam = sqllam.Delete();

            action?.Invoke(sqllam);

            try
            {
                return db.Execute(sqllam.SqlString, sqllam.Parameters, trans, commandTimeout, CommandType.Text);
            }
            catch (Exception ex)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine(ex.Message + ex.StackTrace);
                    Debug.WriteLine(sqllam.SqlString);
                }

                Console.WriteLine(ex.Message + ex.StackTrace);
                Console.WriteLine(sqllam.SqlString);
                throw new DapperLamException(ex.Message, ex, sqllam.SqlString) { Parameters = sqllam.Parameters };
            }
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

            try { 
              countRet = db.Query<int>(countSqlam.SqlString, countSqlam.Parameters).FirstOrDefault();
            }
            catch (Exception ex)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine(ex.Message + ex.StackTrace);
                    Debug.WriteLine(countSqlam.SqlString);
                }

                Console.WriteLine(ex.Message + ex.StackTrace);
                Console.WriteLine(countSqlam.SqlString);
                throw new DapperLamException(ex.Message, ex, countSqlam.SqlString) { Parameters = countSqlam.Parameters };
            }
            var sqlstring = sqllam.QueryPage(pageSize, pageNumber);

            try
            {

                var retlist = db.Query<T>(sqlstring, sqllam.Parameters, trans, commandTimeout: commandTimeout);

                return new PagedResult<T>(retlist, countRet, pageSize, pageNumber);
            }
            catch (Exception ex)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine(ex.Message + ex.StackTrace);
                    Debug.WriteLine(sqlstring);
                }

                Console.WriteLine(ex.Message + ex.StackTrace);
                Console.WriteLine(sqlstring);
                throw new DapperLamException(ex.Message, ex, sqlstring) { Parameters = sqllam.Parameters };
            }

        }
        public static IEnumerable<T> Query<T>(this IDbConnection db, Action<SqlExp<T>> action, IDbTransaction trans = null, int? commandTimeout = null)
        {

            var sqllam = new SqlExp<T>(db.GetAdapter());

            action?.Invoke(sqllam);

            try
            {
                return db.Query<T>(sqllam.SqlString, sqllam.Parameters, trans, commandTimeout: commandTimeout);
            }
            catch (Exception ex)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine(ex.Message + ex.StackTrace);
                    Debug.WriteLine(sqllam.SqlString);
                }

                Console.WriteLine(ex.Message + ex.StackTrace);
                Console.WriteLine(sqllam.SqlString);
                throw new DapperLamException(ex.Message, ex, sqllam.SqlString) { Parameters = sqllam.Parameters };
            }

        }
 

        public static PagedResult<T> PagedQuery<T>(this IDbConnection db, int pageSize, int pageNumber, Action<SqlExp<T>> action, IDbTransaction trans = null, int? commandTimeout = null) where T : class
        {

            var sqllam = new SqlExp<T>(db.GetAdapter());

            var countSqlam = new SqlExp<T>(db.GetAdapter(), true);

            action?.Invoke(sqllam);

            action?.Invoke(countSqlam);

            countSqlam = countSqlam.Count();

            int countRet;
            try
            {
                countRet = db.Query<int>(countSqlam.SqlString, countSqlam.Parameters, trans, commandTimeout: commandTimeout).FirstOrDefault();

            }
            catch (Exception ex)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine(ex.Message + ex.StackTrace);
                    Debug.WriteLine(countSqlam.SqlString);
                }

                Console.WriteLine(ex.Message + ex.StackTrace);
                Console.WriteLine(countSqlam.SqlString);
                throw new DapperLamException(ex.Message, ex, countSqlam.SqlString){Parameters = countSqlam.Parameters};
            }
            var sqlstring = sqllam.QueryPage(pageSize, pageNumber);

            try
            {
                var retlist = db.Query<T>(sqlstring, sqllam.Parameters, trans, commandTimeout: commandTimeout);
                return new PagedResult<T>(retlist, countRet, pageSize, pageNumber);
            }
            catch (Exception ex)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine(ex.Message + ex.StackTrace);
                    Debug.WriteLine(sqlstring);
                }

                Console.WriteLine(ex.Message + ex.StackTrace);
                Console.WriteLine(sqlstring);
                throw new DapperLamException(ex.Message, ex, sqlstring) { Parameters = sqllam.Parameters };
            }
        }

        public static PagedResult<TResult> PagedQuery<T,TResult>(this IDbConnection db, int pageSize, int pageNumber, Action<SqlExp<T>> action, IDbTransaction trans = null, int? commandTimeout = null) where T : class  where TResult:class
        {

            var sqllam = new SqlExp<T>(db.GetAdapter());

            var countSqlam = new SqlExp<T>(db.GetAdapter(), true);

            action?.Invoke(sqllam);

            action?.Invoke(countSqlam);

            countSqlam = countSqlam.Count();

            int countRet;
            try
            {
                countRet = db.Query<int>(countSqlam.SqlString, countSqlam.Parameters, trans, commandTimeout: commandTimeout).FirstOrDefault();
            }
            catch (Exception ex)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine(ex.Message + ex.StackTrace);
                    Debug.WriteLine(countSqlam.SqlString);
                }

                Console.WriteLine(ex.Message + ex.StackTrace);
                Console.WriteLine(countSqlam.SqlString);
                throw new DapperLamException(ex.Message, ex, countSqlam.SqlString) { Parameters = countSqlam.Parameters };
            }

            var sqlstring = sqllam.QueryPage(pageSize, pageNumber);

            try
            {
                var retlist = db.Query<TResult>(sqlstring, sqllam.Parameters, trans, commandTimeout: commandTimeout);
                return new PagedResult<TResult>(retlist, countRet, pageSize, pageNumber);
            }
            catch (Exception ex)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine(ex.Message + ex.StackTrace);
                    Debug.WriteLine(sqlstring);
                }

                Console.WriteLine(ex.Message + ex.StackTrace);
                Console.WriteLine(sqlstring);
                throw new DapperLamException(ex.Message, ex,sqlstring) { Parameters = sqllam.Parameters };
            }
        }


        public static PagedResult<TResult> PagedQuery<TEntity, TResult>(this IDbConnection db, int pageSize, int pageNumber, Action<SqlExp<TResult>> action , Action<SqlExp<TEntity>> subAction ,
            IDbTransaction trans = null, int? commandTimeout = null) where TEntity : class where TResult:class
        {

            var sqllam = new SqlExp<TEntity>(db.GetAdapter());

            //var sqllam = new SqlExp<T>(db.GetAdapter());

            var countSqlam = new SqlExp<TEntity>(db.GetAdapter(), true);

            subAction?.Invoke(sqllam);

            //subAction?.Invoke(countSqlam);

   
            //action?.Invoke(sqllam);

            //subAction?.Invoke(sqllamSub);

            //sqllam.SubQuery(action);

            var sqlLamMain = new SqlExp<TResult>(db.GetAdapter());

            sqlLamMain.SubQuery(sqllam);

            action.Invoke(sqlLamMain);


            countSqlam = countSqlam.Count();
            int countRet;
            try
            {

                  countRet = db.Query<int>(countSqlam.SqlString, countSqlam.Parameters, trans, commandTimeout: commandTimeout).FirstOrDefault();

            }
            catch (Exception ex)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine(ex.Message + ex.StackTrace);
                    Debug.WriteLine(countSqlam.SqlString);
                }

                Console.WriteLine(ex.Message + ex.StackTrace);
                Console.WriteLine(countSqlam.SqlString);
                throw new DapperLamException(ex.Message, ex, countSqlam.SqlString){Parameters = countSqlam.Parameters};
            }

            var sqlstring = sqlLamMain.QueryPage(pageSize, pageNumber);
            try
            {
                var retlist = db.Query<TResult>(sqlstring, sqllam.Parameters, trans, commandTimeout: commandTimeout);
                return new PagedResult<TResult>(retlist, countRet, pageSize, pageNumber);
            }
            catch (Exception ex)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine(ex.Message + ex.StackTrace);
                    Debug.WriteLine(sqlLamMain.SqlString);
                }

                Console.WriteLine(ex.Message + ex.StackTrace);
                Console.WriteLine(sqlLamMain.SqlString);
                throw new DapperLamException(ex.Message, ex, sqlLamMain.SqlString) { Parameters = sqlLamMain.Parameters };
            }
        }

        public static IEnumerable<TResult> Query<TEntity, TResult>(this IDbConnection db, Action<SqlExp<TEntity>> action = null,
            IDbTransaction trans = null, int? commandTimeout = null) where TEntity : class
        {

            var sqllam = new SqlExp<TEntity>(db.GetAdapter());

            action?.Invoke(sqllam);

            try
            {
                return db.Query<TResult>(sqllam.SqlString, sqllam.Parameters, trans, commandTimeout: commandTimeout);
            }
            catch (Exception ex)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine(ex.Message + ex.StackTrace);
                    Debug.WriteLine(sqllam.SqlString);
                }

                Console.WriteLine(ex.Message + ex.StackTrace);
                Console.WriteLine(sqllam.SqlString);
                throw new DapperLamException(ex.Message, ex, sqllam.SqlString) { Parameters = sqllam.Parameters };
            }
        }

        public static IEnumerable<TResult> Query<TEntity, TResult>(this IDbConnection db, Action<SqlExp<TResult>> action,Action<SqlExp<TEntity>> subAction,IDbTransaction trans = null, int? commandTimeout = null) where TEntity : class
        {

            var sqllamSub = new SqlExp<TEntity>(db.GetAdapter());

         

            //action?.Invoke(sqllam);

            subAction?.Invoke(sqllamSub);

            //sqllam.SubQuery(action);

            var sqlLamMain=new SqlExp<TResult>(db.GetAdapter());

            sqlLamMain.SubQuery(sqllamSub);

            action?.Invoke(sqlLamMain);

             

            try
            {
                return db.Query<TResult>(sqlLamMain.SqlString, sqlLamMain.Parameters, trans, commandTimeout: commandTimeout);
            }
            catch (Exception ex)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine(ex.Message + ex.StackTrace);
                    Debug.WriteLine(sqlLamMain.SqlString);
                }

                Console.WriteLine(ex.Message + ex.StackTrace);
                Console.WriteLine(sqlLamMain.SqlString);
                throw new DapperLamException(ex.Message,ex,sqlLamMain.SqlString) { Parameters = sqlLamMain.Parameters };
            }
        }


        public static TResult ExecuteScalar<TEntity, TResult>(this IDbConnection db, Action<SqlExp<TEntity>> action = null,
          IDbTransaction trans = null, int? commandTimeout = null) where TEntity : class
        {
            var sqllam = new SqlExp<TEntity>(db.GetAdapter());

            action?.Invoke(sqllam);

            try
            {
                return db.ExecuteScalar<TResult>(sqllam.SqlString, sqllam.Parameters, trans, commandTimeout);
            }
            catch (Exception ex)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine(ex.Message + ex.StackTrace);
                    Debug.WriteLine(sqllam.SqlString);
                }

                Console.WriteLine(ex.Message + ex.StackTrace);
                Console.WriteLine(sqllam.SqlString);
                throw new DapperLamException(ex.Message, ex, sqllam.SqlString) { Parameters = sqllam.Parameters };
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
