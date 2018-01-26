using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Dapper.LambdaExtension.LambdaSqlBuilder;
using Dapper.LambdaExtension.LambdaSqlBuilder.Entity;

namespace Dapper.LambdaExtension.Extentions
{
    public static partial class DapperLambdaExt
    {

        #region Insert Async

        public static Task<int> InsertAsync<T>(this IDbConnection db, T entity, IDbTransaction trans = null,
            int? commandTimeout = null)
        {
            var sqllam = new SqlExp<T>(db.GetAdapter());

            sqllam = sqllam.Insert();
            var sqlString = sqllam.SqlString;
            try
            {
                DebuggingSqlString(sqlString);
                return db.ExecuteAsync(sqlString, entity, trans, commandTimeout, CommandType.Text);
            }
            catch (Exception ex)
            {
                DebuggingException(ex, sqlString);
                throw new DapperLamException(ex.Message, ex, sqlString) { Parameters = sqllam.Parameters };
            }
        }

        public static Task<int> InsertAsync(this IDbConnection db, SqlTableDefine tableDefine,
            List<SqlColumnDefine> columnDefines,
            IEnumerable<object> entity, IDbTransaction trans = null, int? commandTimeout = null)
        {
            var sqllam = new SqlExp<object>(tableDefine, columnDefines, db.GetAdapter());

            sqllam = sqllam.Insert(tableDefine, columnDefines);
            var sqlString = sqllam.SqlString;
            try
            {

                DebuggingSqlString(sqlString);
                return db.ExecuteAsync(sqlString, entity, trans, commandTimeout, CommandType.Text);
            }
            catch (Exception ex)
            {
                DebuggingException(ex, sqlString);
                throw new DapperLamException(ex.Message, ex, sqlString) { Parameters = sqllam.Parameters };
            }
        }

        public static Task<int> InsertListAsync<T>(this IDbConnection db, IEnumerable<T> entitys,
            IDbTransaction trans = null, int? commandTimeout = null)
        {
            if (!entitys.Any())
            {
                return Task.FromResult(0);
            }

            var sqllam = new SqlExp<T>(db.GetAdapter());

            sqllam = sqllam.Insert();
            var sqlString = sqllam.SqlString;
            try
            {
                DebuggingSqlString(sqlString);

                return db.ExecuteAsync(sqlString, entitys, trans, commandTimeout, CommandType.Text);
            }
            catch (Exception ex)
            {
                DebuggingException(ex, sqlString);
                throw new DapperLamException(ex.Message, ex, sqlString) { Parameters = sqllam.Parameters };
            }
        }

        #endregion


        #region Update Async


        public static Task<int> UpdateAsync<T>(this IDbConnection con, T entity, IDbTransaction trans = null,
            int? commandTimeout = null)
        {
            var db = con;
            if (trans != null)
            {
                db = trans.Connection;
            }

            var sqllam = new SqlExp<T>(db.GetAdapter());

          
            sqllam = sqllam.Update();
            var sqlString = sqllam.SqlString;
            try
            {
                DebuggingSqlString(sqlString);
                return db.ExecuteAsync(sqlString, entity, trans, commandTimeout, CommandType.Text);
            }
            catch (Exception ex)
            {
                DebuggingException(ex, sqlString);
                throw new DapperLamException(ex.Message, ex, sqlString) { Parameters = sqllam.Parameters };
            }
        }
        public static Task<int> UpdateAsync<T>(this IDbConnection con, T entity, Expression<Func<T, bool>> whereExpression, IDbTransaction trans = null, int? commandTimeout = null)
        {
            var db = con;
            if (trans != null)
            {
                db = trans.Connection;
            }
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
                return db.ExecuteAsync(sqlString, entity, trans, commandTimeout, CommandType.Text);
            }
            catch (Exception ex)
            {
                DebuggingException(ex, sqlString);
                throw new DapperLamException(ex.Message, ex, sqlString) { Parameters = sqllam.Parameters };
            }
        }

        public static Task<int> UpdateAsync<T>(this IDbConnection con, Action<SqlExp<T>> action, IDbTransaction trans = null, int? commandTimeout = null)
        {
            var db = con;
            if (trans != null)
            {
                db = trans.Connection;
            }
            var sqllam = new SqlExp<T>(db.GetAdapter());

            action?.Invoke(sqllam);


            var sqlString = sqllam.SqlString;
            try
            {
                DebuggingSqlString(sqlString);
                return db.ExecuteAsync(sqlString, sqllam.Parameters, trans, commandTimeout, CommandType.Text);
            }
            catch (Exception ex)
            {
                DebuggingException(ex, sqlString);
                throw new DapperLamException(ex.Message, ex, sqlString) { Parameters = sqllam.Parameters };
            }
        }

        public static Task<int> UpdateListAsync<T>(this IDbConnection con, IEnumerable<T> entitys,
            IDbTransaction trans = null,
            int? commandTimeout = null)
        {
            var db = con;
            if (trans != null)
            {
                db = trans.Connection;
            }

            if (!entitys.Any())
            {
                return Task.FromResult(0);
            }

         

            var sqllam = new SqlExp<T>(db.GetAdapter());


            sqllam = sqllam.Update();
            var sqlString = sqllam.SqlString;
            try
            {

                DebuggingSqlString(sqlString);
                return db.ExecuteAsync(sqlString, entitys, trans, commandTimeout, CommandType.Text);
            }
            catch (Exception ex)
            {
                DebuggingException(ex, sqlString);
                throw new DapperLamException(ex.Message, ex, sqlString) { Parameters = sqllam.Parameters };
            }
        }

        #endregion


        #region Delete Async



        public static Task<int> DeleteAsync<T>(this IDbConnection con, T entity, IDbTransaction trans = null,
            int? commandTimeout = null)
        {
            var db = con;
            if (trans != null)
            {
                db = trans.Connection;
            }

            var sqllam = new SqlExp<T>(db.GetAdapter());


            sqllam = sqllam.Delete();
            var sqlString = sqllam.SqlString;
            try
            {

                DebuggingSqlString(sqlString);
                return db.ExecuteAsync(sqlString, entity, trans, commandTimeout, CommandType.Text);
            }
            catch (Exception ex)
            {
                DebuggingException(ex, sqlString);
                throw new DapperLamException(ex.Message, ex, sqlString) { Parameters = sqllam.Parameters };
            }
        }

        public static Task<int> DeleteAsync<T>(this IDbConnection con, Expression<Func<T, bool>> deleteExpression,
            IDbTransaction trans = null, int? commandTimeout = null)
        {
            var db = con;
            if (trans != null)
            {
                db = trans.Connection;
            }

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
                return db.ExecuteAsync(sqlString, sqllam.Parameters, trans, commandTimeout, CommandType.Text);
            }
            catch (Exception ex)
            {
                DebuggingException(ex, sqlString);
                throw new DapperLamException(ex.Message, ex, sqlString) { Parameters = sqllam.Parameters };
            }
        }

        public static Task<int> DeleteAsync<T>(this IDbConnection con, Action<SqlExp<T>> action,
            IDbTransaction trans = null,
            int? commandTimeout = null)
        {
            var db = con;
            if (trans != null)
            {
                db = trans.Connection;
            }
            var sqllam = new SqlExp<T>(db.GetAdapter());

            sqllam = sqllam.Delete();

            action?.Invoke(sqllam);
            var sqlString = sqllam.SqlString;
            try
            {

                DebuggingSqlString(sqlString);
                return db.ExecuteAsync(sqlString, sqllam.Parameters, trans, commandTimeout, CommandType.Text);
            }
            catch (Exception ex)
            {
                DebuggingException(ex, sqlString);
                throw new DapperLamException(ex.Message, ex, sqllam.SqlString) { Parameters = sqllam.Parameters };
            }
        }
        public static Task<int> DeleteListAsync<T>(this IDbConnection con, IEnumerable<T> entityList,
            IDbTransaction trans = null,
            int? commandTimeout = null)
        {
            var db = con;
            if (trans != null)
            {
                db = trans.Connection;
            }
            if (!entityList.Any())
            {
                return Task.FromResult(0);
            }

            var sqllam = new SqlExp<T>(db.GetAdapter());


            sqllam = sqllam.Delete();
            var sqlString = sqllam.SqlString;
            try
            {

                DebuggingSqlString(sqlString);
                return db.ExecuteAsync(sqlString, entityList, trans, commandTimeout, CommandType.Text);
            }
            catch (Exception ex)
            {
                DebuggingException(ex, sqlString);
                throw new DapperLamException(ex.Message, ex, sqlString) { Parameters = sqllam.Parameters };
            }
        }

#endregion


        #region Query Async

        public static Task<T> QueryFirstOrDefaultAsync<T>(this IDbConnection con,
            Expression<Func<T, bool>> wherExpression = null,
            IDbTransaction trans = null, int? commandTimeout = null)
        {
            //return Task.Run(() => { return db.QueryFirstOrDefault<T>(wherExpression, trans, commandTimeout); });
            var db = con;
            if (trans != null)
            {
                db = trans.Connection;
            }
            var sqllam = new SqlExp<T>(db.GetAdapter());

            if (wherExpression != null)
            {
                sqllam = sqllam.Where(wherExpression);
            }

            var sqlString = sqllam.SqlString;
            try
            {
                DebuggingSqlString(sqlString);
                return db.QueryFirstOrDefaultAsync<T>(sqlString, sqllam.Parameters, trans, commandTimeout: commandTimeout);
            }
            catch (Exception ex)
            {
                DebuggingException(ex, sqlString);
                throw new DapperLamException(ex.Message, ex, sqlString) { Parameters = sqllam.Parameters };
            }
        }

        public static Task<T> QueryFirstOrDefaultAsync<T>(this IDbConnection con, Action<SqlExp<T>> action, IDbTransaction trans = null, int? commandTimeout = null)
        {
            var db = con;
            if (trans != null)
            {
                db = trans.Connection;
            }
            var sqllam = new SqlExp<T>(db.GetAdapter());


            action?.Invoke(sqllam);

            var sqlString = sqllam.SqlString;
            try
            {
                DebuggingSqlString(sqlString);
                return db.QueryFirstOrDefaultAsync<T>(sqlString, sqllam.Parameters, trans, commandTimeout: commandTimeout);
            }
            catch (Exception ex)
            {
                DebuggingException(ex, sqlString);
                throw new DapperLamException(ex.Message, ex, sqlString) { Parameters = sqllam.Parameters };
            }
        }

        public static Task<TResult> QueryFirstOrDefaultAsync<TEntity, TResult>(this IDbConnection con, Action<SqlExp<TEntity>> action = null,
            IDbTransaction trans = null, int? commandTimeout = null) where TEntity : class
        {
            var db = con;
            if (trans != null)
            {
                db = trans.Connection;
            }
            var sqllam = new SqlExp<TEntity>(db.GetAdapter());

            action?.Invoke(sqllam);
            var sqlString = sqllam.SqlString;
            try
            {

                DebuggingSqlString(sqlString);
                return db.QueryFirstOrDefaultAsync<TResult>(sqlString, sqllam.Parameters, trans, commandTimeout: commandTimeout);
            }
            catch (Exception ex)
            {
                DebuggingException(ex, sqlString);
                throw new DapperLamException(ex.Message, ex, sqlString) { Parameters = sqllam.Parameters };
            }
        }

        public static Task<IEnumerable<T>> QueryAsync<T>(this IDbConnection con,
          Expression<Func<T, bool>> wherExpression = null, IDbTransaction trans = null, int? commandTimeout = null)
        {
            // return Task.Run(() => { return db.Query<T>(wherExpression, trans, commandTimeout); });
            var db = con;
            if (trans != null)
            {
                db = trans.Connection;
            }
            var sqllam = new SqlExp<T>(db.GetAdapter());

            if (wherExpression != null)
            {
                sqllam = sqllam.Where(wherExpression);
            }

            var sqlString = sqllam.SqlString;
            try
            {
                DebuggingSqlString(sqlString);

                return db.QueryAsync<T>(sqlString, sqllam.Parameters, trans, commandTimeout: commandTimeout);
            }
            catch (Exception ex)
            {
                DebuggingException(ex, sqlString);
                throw new DapperLamException(ex.Message, ex, sqlString) { Parameters = sqllam.Parameters };
            }
        }

        public static Task<IEnumerable<T>> QueryAsync<T>(this IDbConnection con, Action<SqlExp<T>> action,
            IDbTransaction trans = null, int? commandTimeout = null)
        {
            var db = con;
            if (trans != null)
            {
                db = trans.Connection;
            }
            var sqllam = new SqlExp<T>(db.GetAdapter());

            action?.Invoke(sqllam);

            var sqlString = sqllam.SqlString;
            try
            {

                DebuggingSqlString(sqlString);
                return db.QueryAsync<T>(sqlString, sqllam.Parameters, trans, commandTimeout: commandTimeout);
            }
            catch (Exception ex)
            {
                DebuggingException(ex, sqlString);
                throw new DapperLamException(ex.Message, ex, sqlString) { Parameters = sqllam.Parameters };
            }
        }

        public static Task<IEnumerable<TResult>> QueryAsync<TEntity, TResult>(this IDbConnection con,
            Action<SqlExp<TEntity>> action = null,
            IDbTransaction trans = null, int? commandTimeout = null) where TEntity : class
        {
            var db = con;
            if (trans != null)
            {
                db = trans.Connection;
            }
            var sqllam = new SqlExp<TEntity>(db.GetAdapter());

            action?.Invoke(sqllam);
            var sqlString = sqllam.SqlString;
            try
            {

                DebuggingSqlString(sqlString);
                return db.QueryAsync<TResult>(sqlString, sqllam.Parameters, trans, commandTimeout: commandTimeout);
            }
            catch (Exception ex)
            {
                DebuggingException(ex, sqlString);
                throw new DapperLamException(ex.Message, ex, sqlString) { Parameters = sqllam.Parameters };
            }
        }

        public static Task<IEnumerable<TResult>> QueryAsync<TEntity, TResult>(this IDbConnection con,
            Action<SqlExp<TResult>> action, Action<SqlExp<TEntity>> subAction,
            IDbTransaction trans = null, int? commandTimeout = null
        )
            where TEntity : class where TResult : class
        {
            var db = con;
            if (trans != null)
            {
                db = trans.Connection;
            }
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
                return db.QueryAsync<TResult>(sqlString, sqlLamMain.Parameters, trans, commandTimeout: commandTimeout);
            }
            catch (Exception ex)
            {
                DebuggingException(ex, sqlString);
                throw new DapperLamException(ex.Message, ex, sqlString) { Parameters = sqlLamMain.Parameters };
            }
        }



        #endregion


        #region Paged Query Async


        public static async Task<PagedResult<T>> PagedQueryAsync<T>(this IDbConnection con, int pageSize, int pageNumber,
            Expression<Func<T, bool>> whereExpression = null, Expression<Func<T, object>> groupByexpression = null,
            IDbTransaction trans = null, int? commandTimeout = null,
            Expression<Func<T, object>> orderbyExpression = null)
            where T : class
        {
     
            var db = trans == null ? con : trans.Connection;

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
                var tret = await db.QueryAsync<int>(sqlString, countSqlam.Parameters);

                countRet = tret.FirstOrDefault();
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
                var retlist = await db.QueryAsync<T>(sqlstring, sqllam.Parameters, trans, commandTimeout: commandTimeout);
 
                return new   PagedResult<T>(retlist, countRet, pageSize, pageNumber);
            }
            catch (Exception ex)
            {
                DebuggingException(ex, sqlstring);
                throw new DapperLamException(ex.Message, ex, sqlstring) { Parameters = sqllam.Parameters };
            }
        }

        public static async Task<PagedResult<T>> PagedQueryAsync<T>(this IDbConnection con, int pageSize, int pageNumber,
            Action<SqlExp<T>> action,
            IDbTransaction trans = null, int? commandTimeout = null
        )
            where T : class
        {
            var db = con;
            if (trans != null)
            {
                db = trans.Connection;
            }
            var sqllam = db.GetSqlExp<T>();

            var countSqlam = db.GetSqlExp<T>( true);

            action?.Invoke(sqllam);

            action?.Invoke(countSqlam);

            countSqlam = countSqlam.Count();

            int countRet;

            var sqlString = countSqlam.SqlString;
            try
            {
                DebuggingSqlString(sqlString);
                //countRet = db.Query<int>(sqlString, countSqlam.Parameters, trans, commandTimeout: commandTimeout).FirstOrDefault();
                var tret = await db.QueryAsync<int>(sqlString, countSqlam.Parameters, trans, commandTimeout: commandTimeout);

                countRet = tret.FirstOrDefault();
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
                var retlist = await db.QueryAsync<T>(sqlstring, sqllam.Parameters, trans, commandTimeout: commandTimeout);
                return new PagedResult<T>(retlist, countRet, pageSize, pageNumber);
            }
            catch (Exception ex)
            {
                DebuggingException(ex, sqlstring);
                throw new DapperLamException(ex.Message, ex, sqlstring) { Parameters = sqllam.Parameters };
            }
        }

        public static async Task<PagedResult<TResult>> PagedQueryAsync<T, TResult>(this IDbConnection con, int pageSize,
            int pageNumber,
            Action<SqlExp<T>> action,
            IDbTransaction trans = null, int? commandTimeout = null
        )
            where T : class where TResult : class
        {
            var db = con;
            if (trans != null)
            {
                db = trans.Connection;
            }

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
                var tret =await db.QueryAsync<int>(countSqlam.SqlString, countSqlam.Parameters, trans, commandTimeout: commandTimeout);
                countRet = tret.FirstOrDefault();
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
                var retlist =  await db.QueryAsync<TResult>(sqlstring, sqllam.Parameters, trans, commandTimeout: commandTimeout);
                return new PagedResult<TResult>(retlist, countRet, pageSize, pageNumber);
            }
            catch (Exception ex)
            {
                DebuggingException(ex, sqlstring);
                throw new DapperLamException(ex.Message, ex, sqlstring) { Parameters = sqllam.Parameters };
            }
        }

        public static async Task<PagedResult<TResult>> PagedQueryAsync<TEntity, TResult>(this IDbConnection con, int pageSize,
            int pageNumber,
            Action<SqlExp<TResult>> action, Action<SqlExp<TEntity>> subAction,
            IDbTransaction trans = null, int? commandTimeout = null
        )
            where TEntity : class where TResult : class
        {
            var db = con;
            if (trans != null)
            {
                db = trans.Connection;
            }
            var sqllam = new SqlExp<TEntity>(db.GetAdapter());

            subAction?.Invoke(sqllam);


            var sqlLamMain = new SqlExp<TResult>(db.GetAdapter());

            sqlLamMain.SubQuery(sqllam);

            action?.Invoke(sqlLamMain);

            var countSqlam = new SqlExp<TResult>(db.GetAdapter(), true);

            countSqlam.SubQuery(sqllam);

            action?.Invoke(countSqlam);

            countSqlam = countSqlam.CountSubQuery<TResult>(sqlLamMain);
            int countRet = 0;
            var sqlString = countSqlam.SqlString;
            try
            {
                DebuggingSqlString(sqlString);
                var tret = await db.QueryAsync<int>(sqlString, countSqlam.Parameters, trans, commandTimeout: commandTimeout);
                countRet = tret.FirstOrDefault();

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
                var retlist = await db.QueryAsync<TResult>(sqlstring, sqlLamMain.Parameters, trans, commandTimeout: commandTimeout);
                return new PagedResult<TResult>(retlist, countRet, pageSize, pageNumber);
            }
            catch (Exception ex)
            {
                DebuggingException(ex, sqlstring);
                throw new DapperLamException(ex.Message, ex, sqlstring) { Parameters = sqlLamMain.Parameters };
            }
        }

        #endregion


        #region Execution Sql Async
        public static Task<TResult> ExecuteScalarAsync<TEntity, TResult>(this IDbConnection con, Action<SqlExp<TEntity>> action = null,
            IDbTransaction trans = null, int? commandTimeout = null) where TEntity : class
        {
            var db = con;
            if (trans != null)
            {
                db = trans.Connection;
            }
            var sqllam = new SqlExp<TEntity>(db.GetAdapter());

            action?.Invoke(sqllam);
            var sqlString = sqllam.SqlString;
            try
            {

                DebuggingSqlString(sqlString);
                return db.ExecuteScalarAsync<TResult>(sqlString, sqllam.Parameters, trans, commandTimeout);
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
        public static Task<int> ExecuteNoCacheAsync(this IDbConnection con, string sql ,object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null, CommandFlags flag = CommandFlags.Buffered, CancellationToken cancellationToken=default(CancellationToken))
        {
            var db = con;
            if (transaction != null)
            {
                db = transaction.Connection;
            }
            CommandDefinition command = new CommandDefinition(sql, param, transaction, commandTimeout, commandType, flag, cancellationToken);

            return db.ExecuteAsync(command);
        }

#endregion
    }
}