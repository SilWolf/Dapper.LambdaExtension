using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Dapper.LambdaExtension.LambdaSqlBuilder.Adapter;
using Dapper.LambdaExtension.LambdaSqlBuilder.Builder;
using Dapper.LambdaExtension.LambdaSqlBuilder.Entity;
using Dapper.LambdaExtension.LambdaSqlBuilder.Resolver;
using Dapper.LambdaExtension.LambdaSqlBuilder.Resolver.ExpressionTree;

namespace Dapper.LambdaExtension.LambdaSqlBuilder
{
    
    public class SqlExp<T> : SqlExpBase
    {
        private bool useForCount;



        public SqlExp(SqlAdapterType type = SqlAdapterType.SqlServer , bool forCount = false)
            : base(type, typeof(T))
        {
            useForCount = forCount;
            //_type = SqlType.Query;
            //GetAdapterInstance(type);
            //_builder = new Builder(_type, LambdaResolver.GetTableName<T>(), _defaultAdapter);
            //_resolver = new LambdaResolver(_builder);
        }

        public SqlExp(SqlTableDefine tableDefine, List<SqlColumnDefine> columnDefines, SqlAdapterType type = SqlAdapterType.SqlServer, bool forCount = false)
            : base(type,tableDefine, columnDefines)
        {
            useForCount = forCount;
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

        public SqlExp<T> Insert(SqlTableDefine tableDefine, List<SqlColumnDefine> columnDefines, bool key = false)
        {
            _builder.UpdateSqlType(SqlType.Insert);

            _resolver.ResolveInsert(key, tableDefine, columnDefines);
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

        public SqlExp<T> Delete(Expression<Func<T, bool>> expression = null)
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

        public virtual SqlExp<T> OrderBy(Expression<Func<T, object>> expression, bool desc = false)
        {
            if (!useForCount)
            {
                _resolver.OrderBy(expression, desc);
            }
            return this;
        }


        public virtual SqlExp<T> OrderBySubQuery<T2>(string subAlias,Expression<Func<T2, object>> expression, bool desc = false)
        {
            if (!useForCount)
            {
                _resolver.OrderBySubQuery(subAlias, expression, desc);
            }
            return this;
        }

        public SqlExp<T> OrderBy(string columnName, bool desc = false)
        {
            if (!useForCount)
            {
                var pe = Expression.Parameter(typeof(T));
                if (string.IsNullOrEmpty(columnName))
                {
                    columnName = "id";
                }
                var memberExpression = Expression.PropertyOrField(pe, columnName);
                _resolver.OrderBy<T>(memberExpression, desc);
            }
            return this;
        }

        //public SqlExp<T> OrderByDescending(params Expression<Func<T, object>>[] expressions)
        //{
        //    foreach (var expression in expressions)
        //        _resolver.OrderBy(expression, true);
        //    return this;
        //}

        //public SqlExp<T> OrderByDescending(params string[] columnNames)
        //{

        //    foreach (var columnName in columnNames)
        //    {
        //        var pe = Expression.Parameter(typeof(T));
        //        // var member = typeof(T).GetProperty(columnName);
        //        var memberExpression = Expression.PropertyOrField(pe, columnName);

        //        // var memberFunctionExpression = Expression.Lambda<Func<T, object>>(memberExpression, pe);
        //        _resolver.OrderBy<T>(memberExpression, true);

        //    }

        //    return this;
        //}

        #endregion

        #region 查询

        public SqlExp<T> Select(params Expression<Func<T, object>>[] expressions)
        {
            foreach (var expression in expressions)
                _resolver.Select(expression);
            return this;
        }

        public SqlExp<T> SelectSubQuery<T2>(string subQueryAlias,params Expression<Func<T2, object>>[] expressions)
        {
            foreach (var expression in expressions)
                _resolver.SelectSubQuery<T2>(expression,subQueryAlias);

            return this;
        }

        public SqlExp<T> SelectEntity(params Expression<Func<T, SqlColumnEntity>>[] expressions)
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

        public SqlExp<T> Count(string aliasName = "count")
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

        public SqlExp<T2> Join<T2, TKey>(Action<SqlExp<T2>> joinQuery,
            Expression<Func<T, TKey>> primaryKeySelector,
            Expression<Func<T2, TKey>> foreignKeySelector, JoinType joinType = JoinType.Join,
            params Expression<Func<T2, object>>[] selections)
        {
            var query = new SqlExp<T2>(_builder, _resolver);
     
            joinQuery?.Invoke(query);

            query.Select(selections);

            _resolver.Join<T, T2, TKey>(primaryKeySelector, foreignKeySelector, joinType);
            return query;
        }

        public SqlExp<T2> Join<T2>(Expression<Func<T, T2, bool>> expression,JoinType joinType = JoinType.Join)
        {
            var joinQuery = new SqlExp<T2>(_builder, _resolver);
            _resolver.Join(expression, joinType);
           
            return joinQuery;
        }


        public SqlExp<T2> JoinSubQuery<T2, TKey>(Action<SqlExp<T2>> action, Expression<Func<T, T2, bool>> expression,JoinType joinType=JoinType.Join)
        {
            var joinQuery = new SqlExp<T2>(this._adapter);

            action?.Invoke(joinQuery);
            _resolver.JoinSubQuery(joinQuery, expression, joinType);

            return joinQuery;
        }
        public SqlExp<T2> JoinSubQuery<T2, TKey>(Action<SqlExp<T2>> joinQuery,
            Expression<Func<T, TKey>> primaryKeySelector,
            Expression<Func<T2, TKey>> foreignKeySelector, JoinType joinType = JoinType.Join,
            params Expression<Func<T2, object>>[] selections)
        {
            var query = new SqlExp<T2>(_adapter);

            joinQuery?.Invoke(query);

            _resolver.JoinSubQuery<T, T2, TKey>(query, primaryKeySelector, foreignKeySelector, joinType);

           // query.Select(selections);

            this.SelectSubQuery(query.JoinSubAliasTableName, selections);


          
            return query;
        }




        public SqlExp<T2> LeftJoin<T2, TKey>(Action<SqlExp<T2>> joinQuery,
            Expression<Func<T, TKey>> primaryKeySelector,
            Expression<Func<T2, TKey>> foreignKeySelector,
            params Expression<Func<T2, object>>[] selections)
        {
            var query = new SqlExp<T2>(_builder, _resolver);

            joinQuery?.Invoke(query);

            query.Select(selections);

            _resolver.Join<T, T2, TKey>(primaryKeySelector, foreignKeySelector,JoinType.LeftJoin);
            return query;
        }

        public SqlExp<T2> LeftJoin<T2>(Expression<Func<T, T2, bool>> expression)
        {
            var joinQuery = new SqlExp<T2>(_builder, _resolver);
            _resolver.Join(expression,JoinType.LeftJoin);

            return joinQuery;
        }

        public SqlExp<T2> LeftJoinSubQuery<T2, TKey>(Action<SqlExp<T2>> action, Expression<Func<T, T2, bool>> expression)
        {
            return JoinSubQuery<T2, TKey>(action, expression, JoinType.LeftJoin);
        }
        public SqlExp<T2> LeftJoinSubQuery<T2, TKey>(Action<SqlExp<T2>> action,
            Expression<Func<T, TKey>> primaryKeySelector,
            Expression<Func<T2, TKey>> foreignKeySelector,
            params Expression<Func<T2, object>>[] selections)
        {
            return JoinSubQuery(action, primaryKeySelector, foreignKeySelector, JoinType.LeftJoin, selections);
        }



        public SqlExp<T2> InnerJoin<T2, TKey>(Action<SqlExp<T2>> joinQuery,
            Expression<Func<T, TKey>> primaryKeySelector,
            Expression<Func<T2, TKey>> foreignKeySelector,
            params Expression<Func<T2, object>>[] selections)
        {
            var query = new SqlExp<T2>(_builder, _resolver);

            joinQuery?.Invoke(query);

            query.Select(selections);

            _resolver.Join<T, T2, TKey>(primaryKeySelector, foreignKeySelector, JoinType.InnerJoin);
            return query;
        }

        public SqlExp<T2> InnerJoin<T2>(Expression<Func<T, T2, bool>> expression)
        {
            var joinQuery = new SqlExp<T2>(_builder, _resolver);
            _resolver.Join(expression, JoinType.InnerJoin);

            return joinQuery;
        }

        public SqlExp<T2> InnerJoinSubQuery<T2, TKey>(Action<SqlExp<T2>> action, Expression<Func<T, T2, bool>> expression)
        {
            return JoinSubQuery<T2, TKey>(action, expression, JoinType.InnerJoin);
        }
        public SqlExp<T2> InnerJoinSubQuery<T2, TKey>(Action<SqlExp<T2>> action,
            Expression<Func<T, TKey>> primaryKeySelector,
            Expression<Func<T2, TKey>> foreignKeySelector,
            params Expression<Func<T2, object>>[] selections)
        {
            return JoinSubQuery(action, primaryKeySelector, foreignKeySelector, JoinType.InnerJoin, selections);
        }

        #endregion

        #region 分组

        public SqlExp<T> GroupBy(params Expression<Func<T, object>>[] expressions)
        {
            foreach (var expression in expressions)
                _resolver.GroupBy(expression);
            return this;
        }

        public SqlExp<T> GroupBySubQuery<T2>(string subAlias,params Expression<Func<T2, object>>[] expressions)
        {
            foreach (var expression in expressions)
                _resolver.GroupBySubQuery(expression, subAlias);
            return this;
        }

        public SqlExp<T> GroupBy(params string[] columnNames)
        {

            foreach (var columnName in columnNames)
            {
                var pe = Expression.Parameter(typeof(T));
                var memberExpression = Expression.PropertyOrField(pe, columnName);
                _resolver.GroupBy<T>(memberExpression);
            }
            return this;
        }
        #endregion
    }
}
