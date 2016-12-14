using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using AZ.Dapper.LambdaExtension.Adapter;
using AZ.Dapper.LambdaExtension.Entity;
using AZ.Dapper.LambdaExtension.Resolver;
using AZ.Dapper.LambdaExtension.Resolver.ExpressionTree;

namespace AZ.Dapper.LambdaExtension
{
    [Serializable]
    public class SqlExp<T> : SqlExpBase
    {
        public SqlExp(SqlAdapterType type = SqlAdapterType.SqlServer)
            : base(type, LambdaResolver.GetTableName<T>(),typeof(T))
        {
            //_type = SqlType.Query;
            //GetAdapterInstance(type);
            //_builder = new Builder(_type, LambdaResolver.GetTableName<T>(), _defaultAdapter);
            //_resolver = new LambdaResolver(_builder);
        }

        public SqlExp(Expression<Func<T, bool>> expression)
            : this()
        {
            Where(expression);
        }

        internal SqlExp(Builder.Builder builder, LambdaResolver resolver)
        {
            _builder = builder;
            _resolver = resolver;
        }

        //public SqlLam<T> Clone()
        //{
        //    var lam=new SqlLam<T>(_adapter);


        ////     internal Builder _builder;
        ////internal LambdaResolver _resolver;
        ////internal SqlType _type;
        ////internal SqlAdapter _adapter;

        //    lam._builder = _builder;
        //    lam._resolver = _resolver;
        //    lam._type = _type;
        //}

        #region 修改配置

        public SqlExp<T> UseEntityProperty(bool use)
        {
            _builder.UpdateUseEntityProperty(use);
            return this;
        }

        #endregion

        #region Insert Update Delete 操作

        public SqlExp<T> Insert(T entity, bool key = false)
        {
            _builder.UpdateSqlType(SqlType.Insert);
            _resolver.ResolveInsert<T>(key, entity);
            return this;
        }

        public SqlExp<T> Insert(object obj, bool key = false)
        {
            _builder.UpdateSqlType(SqlType.Insert);
            _resolver.ResolveInsert(key, typeof(T), obj);
            return this;
        }

        public SqlExp<T> Update(T entity)
        {
            _builder.UpdateSqlType(SqlType.Update);
            _resolver.ResolveUpdate<T>(entity);
            return this;
        }

        public SqlExp<T> Update(object obj)
        {
            _builder.UpdateSqlType(SqlType.Update);
            _resolver.ResolveUpdate(typeof(T), obj);
            return this;
        }

        public SqlExp<T> Delete(Expression<Func<T, bool>> expression=null)
        {
            _builder.UpdateSqlType(SqlType.Delete);
            if (expression == null)
            {
                return this;
            }
            return And(expression);
        }

        #endregion

        #region 查询条件

        public SqlExp<T> Where(Expression<Func<T, bool>> expression)
        {
            return And(expression);
        }

        public SqlExp<T> And(Expression<Func<T, bool>> expression)
        {
            _builder.And();
            _resolver.ResolveQuery(expression);
            return this;
        }

        public SqlExp<T> Or(Expression<Func<T, bool>> expression)
        {
            _builder.Or();
            _resolver.ResolveQuery(expression);
            return this;
        }

        public SqlExp<T> WhereIsIn(Expression<Func<T, object>> expression, SqlExpBase sqlQuery)
        {
            _builder.And();
            _resolver.QueryByIsIn(false, expression, sqlQuery);
            return this;
        }

        public SqlExp<T> WhereIsIn(Expression<Func<T, object>> expression, IEnumerable<object> values)
        {
            _builder.And();
            _resolver.QueryByIsIn(false, expression, values);
            return this;
        }

        public SqlExp<T> WhereNotIn(Expression<Func<T, object>> expression, SqlExpBase sqlQuery)
        {
            _builder.And();
            _resolver.QueryByIsIn(true, expression, sqlQuery);
            return this;
        }

        public SqlExp<T> WhereNotIn(Expression<Func<T, object>> expression, IEnumerable<object> values)
        {
            _builder.And();
            _resolver.QueryByIsIn(true, expression, values);
            return this;
        }
        #endregion

        #region 排序

        public SqlExp<T> OrderBy(params Expression<Func<T, object>>[] expressions)
        {
            foreach (var expression in expressions)
                _resolver.OrderBy(expression);
            return this;
        }

        public SqlExp<T> OrderByDescending(params Expression<Func<T, object>>[] expressions)
        {
            foreach (var expression in expressions)
                _resolver.OrderBy(expression, true);
            return this;
        }
        #endregion

        #region 查询
        public SqlExp<T> Select(params Expression<Func<T, object>>[] expressions)
        {
            foreach (var expression in expressions)
                _resolver.Select(expression);
            return this;
        }

        public SqlExp<T> Select(params Expression<Func<T, SqlColumnEntity>>[] expressions)
        {
            foreach (var expression in expressions)
                _resolver.Select(expression);
            return this;
        }

        public SqlExp<T> Distinct(Expression<Func<T, object>> expression)
        {
            _resolver.SelectWithFunction(expression, SelectFunction.DISTINCT, "");
            return this;
        }

        #endregion

        #region 聚合

        public SqlExp<T> Count(Expression<Func<T, object>> expression, string aliasName = "count")
        {
            _resolver.SelectWithFunction(expression, SelectFunction.COUNT, aliasName);
            return this;
        }

        public SqlExp<T> Count( string aliasName = "count")
        {
            _resolver.SelectWithFunction<T>(null, SelectFunction.COUNT, aliasName);
            return this;
        }

        public SqlExp<T> Sum(Expression<Func<T, object>> expression, string aliasName = "")
        {
            _resolver.SelectWithFunction(expression, SelectFunction.SUM, aliasName);
            return this;
        }

        public SqlExp<T> Max(Expression<Func<T, object>> expression, string aliasName = "")
        {
            _resolver.SelectWithFunction(expression, SelectFunction.MAX, aliasName);
            return this;
        }

        public SqlExp<T> Min(Expression<Func<T, object>> expression, string aliasName = "")
        {
            _resolver.SelectWithFunction(expression, SelectFunction.MIN, aliasName);
            return this;
        }

        public SqlExp<T> Average(Expression<Func<T, object>> expression, string aliasName = "")
        {
            _resolver.SelectWithFunction(expression, SelectFunction.AVG, aliasName);
            return this;
        }

        #endregion

        #region 连接 Join

        public SqlExp<TResult> Join<T2, TKey, TResult>(SqlExp<T2> joinQuery,
            Expression<Func<T, TKey>> primaryKeySelector,
            Expression<Func<T2, TKey>> foreignKeySelector,
            Func<T, T2, TResult> selection)
        {
            var query = new SqlExp<TResult>(_builder, _resolver);
            _resolver.Join<T, T2, TKey>(primaryKeySelector, foreignKeySelector);
            return query;
        }

        public SqlExp<T2> Join<T2>(Expression<Func<T, T2, bool>> expression)
        {
            var joinQuery = new SqlExp<T2>(_builder, _resolver);
            _resolver.Join(expression);
            return joinQuery;
        }

        #endregion

        #region 分组

        public SqlExp<T> GroupBy(Expression<Func<T, object>> expression)
        {
            _resolver.GroupBy(expression);
            return this;
        }

        #endregion
    }
}
