using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml;
using AZ.Dapper.LambdaExtension.Adapter;
using AZ.Dapper.LambdaExtension.Extentions;
using AZ.Dapper.LambdaExtension.Helpers;
using Dapper;

namespace AZ.Dapper.LambdaExtension
{
    public static class DapperLambdaExt
    {
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
        public static IEnumerable<T> Query<T>(this IDbConnection db, Expression<Func<T, bool>> wherExpression=null, IDbTransaction trans = null, int? commandTimeout = null)
        {

            var sqllam = new SqlExp<T>(db.GetAdapter());

            if (wherExpression != null)
            {
                sqllam = sqllam.Where(wherExpression);
            }

            //var sqlString = sqllam.SqlString;
            //var param = sqllam.Parameters;
            //string parameterString = GetParameterString(sqllam.Parameters);

            return db.Query<T>(sqllam.SqlString, sqllam.Parameters,trans,commandTimeout:commandTimeout);

        }

        public static T QueryFirstOrDefault<T>(this IDbConnection db, Expression<Func<T, bool>> wherExpression = null, IDbTransaction trans = null, int? commandTimeout = null)
        {

            var sqllam = new SqlExp<T>(db.GetAdapter());

            if (wherExpression != null)
            {
                sqllam = sqllam.Where(wherExpression);
            }


            //var sqlString = sqllam.SqlString;
            //var param = sqllam.Parameters;
            //string parameterString = GetParameterString(sqllam.Parameters);

            return db.QueryFirstOrDefault<T>(sqllam.SqlString, sqllam.Parameters, trans, commandTimeout: commandTimeout);

        }

        public static int Insert<T>(this IDbConnection db, T entity,IDbTransaction trans=null,int? commandTimeout=null)
        {

            var sqllam = new SqlExp<T>(db.GetAdapter());

             
                sqllam = sqllam.Insert(entity);
            

            //var sqlString = sqllam.SqlString;
            //var param = sqllam.Parameters;
            //string parameterString = GetParameterString(sqllam.Parameters);

            return db.Execute(sqllam.SqlString,entity,trans,commandTimeout,CommandType.Text);

        }

        public static int InsertList<T>(this IDbConnection db, List<T> entitys, IDbTransaction trans = null, int? commandTimeout = null)
        {

            var sqllam = new SqlExp<T>(db.GetAdapter());


            sqllam = sqllam.Insert(entitys.FirstOrDefault());


            //var sqlString = sqllam.SqlString;
            //var param = sqllam.Parameters;
            //string parameterString = GetParameterString(sqllam.Parameters);

            return db.Execute(sqllam.SqlString, entitys, trans, commandTimeout, CommandType.Text);

        }
        public static int Update<T>(this IDbConnection db, T entity, IDbTransaction trans = null, int? commandTimeout = null)
        {

            var sqllam = new SqlExp<T>(db.GetAdapter());


            sqllam = sqllam.Update(entity);


            //var sqlString = sqllam.SqlString;
            //var param = sqllam.Parameters;
            //string parameterString = GetParameterString(sqllam.Parameters);

            return db.Execute(sqllam.SqlString, entity, trans, commandTimeout, CommandType.Text);

        }


      

        public static int UpdateList<T>(this IDbConnection db, List<T> entitys, IDbTransaction trans = null, int? commandTimeout = null)
        {

            var sqllam = new SqlExp<T>(db.GetAdapter());


            sqllam = sqllam.Update(entitys.FirstOrDefault());


            //var sqlString = sqllam.SqlString;
            //var param = sqllam.Parameters;
            //string parameterString = GetParameterString(sqllam.Parameters);

            return db.Execute(sqllam.SqlString, entitys, trans, commandTimeout, CommandType.Text);

        }



        public static IEnumerable<T> QueryWithAction<T>(this IDbConnection db, Action<SqlExp<T>> action=null, IDbTransaction trans = null, int? commandTimeout = null)
        {

            var sqllam = new SqlExp<T>(db.GetAdapter());

            action?.Invoke(sqllam);

            //var sqlString = sqllam.SqlString;
            //var param = sqllam.Parameters;
            //string parameterString = GetParameterString(sqllam.Parameters);

            return db.Query<T>(sqllam.SqlString, sqllam.Parameters,trans,commandTimeout:commandTimeout);

        }
        public static int Delete<T>(this IDbConnection db, T engity, IDbTransaction trans = null, int? commandTimeout = null)
        {

            var sqllam = new SqlExp<T>(db.GetAdapter());


            sqllam = sqllam.Delete();


            //var sqlString = sqllam.SqlString;
            //var param = sqllam.Parameters;
            //string parameterString = GetParameterString(sqllam.Parameters);

            return db.Execute(sqllam.SqlString, engity, trans, commandTimeout, CommandType.Text);

        }

        public static int DeleteList<T>(this IDbConnection db, List<T> engityList, IDbTransaction trans = null, int? commandTimeout = null)
        {

            var sqllam = new SqlExp<T>(db.GetAdapter());


            sqllam = sqllam.Delete();


            //var sqlString = sqllam.SqlString;
            //var param = sqllam.Parameters;
            //string parameterString = GetParameterString(sqllam.Parameters);

            return db.Execute(sqllam.SqlString, engityList, trans, commandTimeout, CommandType.Text);

        }

        public static int Delete<T>(this IDbConnection db, Expression<Func<T,bool>> deleteExpression, IDbTransaction trans = null, int? commandTimeout = null)
        {

            var sqllam = new SqlExp<T>(db.GetAdapter());


            sqllam = sqllam.Delete(deleteExpression);


            //var sqlString = sqllam.SqlString;
            //var param = sqllam.Parameters;
            //string parameterString = GetParameterString(sqllam.Parameters);

            return db.Execute(sqllam.SqlString, sqllam.Parameters, trans, commandTimeout, CommandType.Text);

        }


        public static PagedResult<T> PagedQuery<T>(this IDbConnection db,int pageSize,int pageNumber, Expression<Func<T, bool>> whereExpression = null, Expression<Func<T, object>> groupByexpression=null, IDbTransaction trans = null, int? commandTimeout = null, params Expression<Func<T, object>>[] orderbyExpressions)
            where T:class
        {

            var sqllam = new SqlExp<T>(db.GetAdapter());
            var countSqlam = new SqlExp<T>(db.GetAdapter());
            if (whereExpression != null)
            {
                sqllam = sqllam.Where(whereExpression);
                countSqlam = countSqlam.Where(whereExpression);
            }

            if (orderbyExpressions != null && orderbyExpressions.Length > 0)
            {
                sqllam = sqllam.OrderBy(orderbyExpressions);
            }

            if (groupByexpression != null)
            {
                sqllam=sqllam.GroupBy(groupByexpression);
            }

            countSqlam = countSqlam.Count();

            var countRet = db.Query<int>(countSqlam.SqlString, countSqlam.Parameters).FirstOrDefault();
           
            //var sqlString = sqllam.SqlString;
            //var param = sqllam.Parameters;
            //string parameterString = GetParameterString(sqllam.Parameters);

            var sqlstring = sqllam.QueryPage(pageSize, pageNumber);

            var retlist = db.Query<T>(sqlstring, sqllam.Parameters,trans,commandTimeout:commandTimeout);

            return new PagedResult<T>(retlist, countRet,pageSize,pageNumber);

        }

        public static PagedResult<T> PagedQueryWithAction<T>(this IDbConnection db, int pageSize, int pageNumber, Action<SqlExp<T>> action=null, IDbTransaction trans = null, int? commandTimeout = null) where T :class
        {

            var sqllam = new SqlExp<T>(db.GetAdapter());
           

            if (action != null)
            {
                action(sqllam);
            }

            

            var countSqlam = Clone(sqllam).Count();

            var countRet =  db.Query<int>(countSqlam.SqlString, countSqlam.Parameters, trans, commandTimeout: commandTimeout).FirstOrDefault(); 
         
            //var sqlString = sqllam.SqlString;
            //var param = sqllam.Parameters;
            //string parameterString = GetParameterString(sqllam.Parameters);
            var sqlstring = sqllam.QueryPage(pageSize, pageNumber);

            var retlist = db.Query<T>(sqlstring, sqllam.Parameters,trans,commandTimeout:commandTimeout);

            // return new Tuple<IEnumerable<T>, int>(retlist,countRet);
            return new PagedResult<T>(retlist, countRet, pageSize, pageNumber);

        }

        public static T Clone<T>(T obj) 

        {
            using (Stream objectStream = new MemoryStream())
            {
                //利用 System.Runtime.Serialization序列化与反序列化完成引用对象的复制  
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(objectStream, obj);
                objectStream.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(objectStream);
            }
        }
    }
}
