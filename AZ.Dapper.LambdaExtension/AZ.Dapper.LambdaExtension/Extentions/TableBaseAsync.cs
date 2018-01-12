using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Dapper.LambdaExtension.LambdaSqlBuilder;
using Dapper.LambdaExtension.LambdaSqlBuilder.Adapter;

namespace Dapper.LambdaExtension.Extentions
{
    public abstract partial  class TableBase<T,TDbFactory> where T:class where TDbFactory : IDbFactory,new ()
    {


        #region extend table async opreation methods
 
        public static Task<int> InsertAsync(T entity,IDbTransaction trans = null)
        {

            using (var db = DbFactory.OpenDbConnection())
            {
                return db.InsertAsync<T>(entity,trans);
            }
        }

        public static Task<int> InsertAsync(List<T> entities, IDbTransaction trans = null)
        {

            using (var db = DbFactory.OpenDbConnection())
            {
               return db.InsertListAsync<T>(entities, trans);
            }
        }

        public static Task<int> DeleteAsync(T entity, IDbTransaction trans = null) 
        {
            using (var db = DbFactory.OpenDbConnection())
            {
                return db.DeleteAsync<T>(entity, trans:trans);
            }
        }

        public static Task<int> DeleteAsync(List<T> entityList, IDbTransaction trans = null)
        {
            using (var db = DbFactory.OpenDbConnection())
            {
                return db.DeleteListAsync<T>(entityList, trans: trans);
            }
        }

        public static Task<int> DeleteAsync(Action<SqlExp<T>> action, IDbTransaction trans = null)
        {
            using (var db = DbFactory.OpenDbConnection())
            {
                return db.DeleteAsync<T>(action:action, trans: trans);
            }
        }

        public static Task<int> DeleteAsync(Expression<Func<T,bool>> expression, IDbTransaction trans = null)
        {
            using (var db = DbFactory.OpenDbConnection())
            {
                return db.DeleteAsync<T>(expression, trans: trans);
            }
        }

        public static Task<int> UpdateAsync(T entity, IDbTransaction trans = null)
        {
            using (var db = DbFactory.OpenDbConnection())
            {
                return db.UpdateAsync<T>(entity, trans: trans);
            }
        }

        public static Task<int> UpdateAsync(List<T> entityList, IDbTransaction trans = null)
        {
            using (var db = DbFactory.OpenDbConnection())
            {
                return db.UpdateListAsync<T>(entityList, trans: trans);
            }
        }

        public static Task<int> UpdateAsync(Action<SqlExp<T>> action,IDbTransaction trans=null)
        {
            using (var db = DbFactory.OpenDbConnection())
            {
                return db.UpdateAsync<T>(action,trans);
            }
        }



        public static Task<IEnumerable<T>> QueryAsync(Expression<Func<T, bool>> expression, IDbTransaction trans=null)
        {
            using (var db = DbFactory.OpenDbConnection())
            {
                return db.QueryAsync<T>(expression, trans);
            }
        }

        public static Task<IEnumerable<T>> QueryAsync(Action<SqlExp<T>> action, IDbTransaction trans=null)
        {
            using (var db = DbFactory.OpenDbConnection())
            {
                return db.QueryAsync<T>(action, trans);
            }
        }

        public static Task<PagedResult<T>> PagedQueryAsync(int pageSize,int pageNumber, Expression<Func<T, bool>> expression=null, Expression<Func<T,object>> orderby=null, IDbTransaction trans=null)
        {
            using (var db = DbFactory.OpenDbConnection())
            {
                return db.PagedQueryAsync<T>(pageSize,pageNumber,expression,orderbyExpression:orderby,trans: trans);
            }
        }

        public static Task<PagedResult<T>> PagedQueryAsync(int pageSize, int pageNumber, Action<SqlExp<T>> action, IDbTransaction trans)
        {
            using (var db = DbFactory.OpenDbConnection())
            {
                return db.PagedQueryAsync<T>(pageSize,pageNumber,action, trans);
            }
        }


        public static Task<T> QueryFirstOrDefaultAsync(Expression<Func<T, bool>> expression, IDbTransaction trans = null)
        {
            using (var db = DbFactory.OpenDbConnection())
            {
                return db.QueryFirstOrDefaultAsync<T>(expression, trans);
            }
        }

        public static Task<T> QueryFirstOrDefaultAsync(Action<SqlExp<T>> action, IDbTransaction trans = null)
        {
            using (var db = DbFactory.OpenDbConnection())
            {
                return db.QueryFirstOrDefaultAsync<T>(action, trans);
            }
        }


        public static Task<bool> TableExistsAsync(IDbTransaction trans = null)
        {
            using (var db = DbFactory.OpenDbConnection())
            {
                return db.TableExistAsync<T>(trans);
            }
        }

        public static Task CreateTableAsync(IDbTransaction trans = null)
        {
            using (var db = DbFactory.OpenDbConnection())
            {
                  return db.CreateTableAsync<T>(trans);
            }
        }

        public static Task CreateTableIfNotExistsAsync(IDbTransaction trans = null)
        {
            using (var db = DbFactory.OpenDbConnection())
            {
               return db.CreateTableIfNotExistAsync<T>(trans);
            }
        }
        #endregion
    }
}
