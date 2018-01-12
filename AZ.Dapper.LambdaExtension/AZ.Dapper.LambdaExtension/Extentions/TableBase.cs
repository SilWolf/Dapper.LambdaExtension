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
    public abstract partial class TableBase<T, TDbFactory> where T : class where TDbFactory : IDbFactory, new()
    {


        #region extend table opreation methods

        private static TDbFactory _dbfactory;

        private static TDbFactory DbFactory
        {
            get
            {
                if (_dbfactory == null)
                {
                    _dbfactory = new TDbFactory();
                }
                return _dbfactory;
            }
        }

        public static int Insert(T entity, IDbTransaction trans = null)
        {

            using (var db = DbFactory.OpenDbConnection())
            {
                return db.Insert<T>(entity, trans);
            }
        }

        public static int Insert(List<T> entities, IDbTransaction trans = null)
        {
            if (trans != null)
            {
                return trans.Connection.InsertList<T>(entities, trans);
            }
            else
            {

                using (var db = DbFactory.OpenDbConnection())
                {
                    var tr = db.BeginTransaction();
                    try
                    {
                        var ret = db.InsertList<T>(entities, tr);
                        tr.Commit();
                        return ret;
                    }
                    catch (Exception ex)
                    {
                        tr.Rollback();
                        throw ex;
                    }
                }

            }
        }

        public static int Delete(T entity, IDbTransaction trans = null)
        {
            using (var db = DbFactory.OpenDbConnection())
            {
                return db.Delete<T>(entity: entity, trans: trans);
            }
        }

        public static int Delete(List<T> entityList, IDbTransaction trans = null)
        {
            if (trans != null)
            {
                return trans.Connection.DeleteList<T>(entityList, trans);
            }
            else
            {
                using (var db = DbFactory.OpenDbConnection())
                {
                    var tr = db.BeginTransaction();
                    try
                    {
                        var ret= db.DeleteList<T>(entityList, trans: tr);
                        tr.Commit();
                        return ret;
                    }
                    catch (Exception ex)
                    {
                        tr.Rollback();
                        throw ex;
                    }
                }
            }
        }

        public static int Delete(Action<SqlExp<T>> action, IDbTransaction trans = null)
        {
            using (var db = DbFactory.OpenDbConnection())
            {
                return db.Delete<T>(action: action, trans: trans);
            }
        }

        public static int Delete(Expression<Func<T, bool>> expression, IDbTransaction trans = null)
        {
            using (var db = DbFactory.OpenDbConnection())
            {
                return db.Delete<T>(expression, trans: trans);
            }
        }

        public static int Update(T entity, IDbTransaction trans = null)
        {
            using (var db = DbFactory.OpenDbConnection())
            {
                return db.Update<T>(entity, trans: trans);
            }
        }

        public static int Update(List<T> entityList, IDbTransaction trans = null)
        {
            //using (var db = DbFactory.OpenDbConnection())
            //{
            //    return db.UpdateList<T>(entityList, trans: trans);
            //}

            if (trans != null)
            {
                return trans.Connection.UpdateList<T>(entityList, trans);
            }
            else
            {
                using (var db = DbFactory.OpenDbConnection())
                {
                    var tr = db.BeginTransaction();
                    try
                    {
                        var ret = db.UpdateList<T>(entityList, trans: tr);
                        tr.Commit();
                        return ret;
                    }
                    catch (Exception ex)
                    {
                        tr.Rollback();
                        throw ex;
                    }
                }
            }
        }

        public static int Update(Action<SqlExp<T>> action, IDbTransaction trans = null)
        {
            using (var db = DbFactory.OpenDbConnection())
            {
                return db.Update<T>(action, trans);
            }
        }



        public static List<T> Query(Expression<Func<T, bool>> expression, IDbTransaction trans = null)
        {
            using (var db = DbFactory.OpenDbConnection())
            {
                return db.Query<T>(expression, trans).ToList();
            }
        }

        public static List<T> Query(Action<SqlExp<T>> action, IDbTransaction trans = null)
        {
            using (var db = DbFactory.OpenDbConnection())
            {
                return db.Query<T>(action, trans).ToList();
            }
        }

        public static PagedResult<T> PagedQuery(int pageSize, int pageNumber, Expression<Func<T, bool>> expression = null, Expression<Func<T, object>> orderby = null, IDbTransaction trans = null)
        {
            using (var db = DbFactory.OpenDbConnection())
            {
                return db.PagedQuery<T>(pageSize, pageNumber, expression, orderbyExpression: orderby, trans: trans);
            }
        }

        public static PagedResult<T> PagedQuery(int pageSize, int pageNumber, Action<SqlExp<T>> action, IDbTransaction trans=null)
        {
            using (var db = DbFactory.OpenDbConnection())
            {
                return db.PagedQuery<T>(pageSize, pageNumber, action, trans);
            }
        }


        public static T QueryFirstOrDefault(Expression<Func<T, bool>> expression, IDbTransaction trans = null)
        {
            using (var db = DbFactory.OpenDbConnection())
            {
                return db.QueryFirstOrDefault<T>(expression, trans);
            }
        }

        public static T QueryFirstOrDefault(Action<SqlExp<T>> action, IDbTransaction trans = null)
        {
            using (var db = DbFactory.OpenDbConnection())
            {
                return db.QueryFirstOrDefault<T>(action, trans);
            }
        }


        public static bool TableExists(IDbTransaction trans = null)
        {
            using (var db = DbFactory.OpenDbConnection())
            {
                return db.TableExist<T>(trans);
            }
        }

        public static void CreateTable(IDbTransaction trans = null)
        {
            using (var db = DbFactory.OpenDbConnection())
            {
                db.CreateTable<T>(trans);
            }
        }

        public static void CreateTableIfNotExists(IDbTransaction trans = null)
        {
            using (var db = DbFactory.OpenDbConnection())
            {
                db.CreateTableIfNotExist<T>(trans);
            }
        }
        #endregion
    }
}
