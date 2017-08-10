using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Dapper.LambdaExtension.LambdaSqlBuilder.Attributes;
using Dapper.LambdaExtension.LambdaSqlBuilder.Entity;

namespace Dapper.LambdaExtension.LambdaSqlBuilder.Resolver
{
    
    partial class LambdaResolver
    {
        private Dictionary<ExpressionType, string> _operationDictionary = new Dictionary<ExpressionType, string>()
                                                                              {
                                                                                  { ExpressionType.Equal, "="},
                                                                                  { ExpressionType.NotEqual, "!="},
                                                                                  { ExpressionType.GreaterThan, ">"},
                                                                                  { ExpressionType.LessThan, "<"},
                                                                                  { ExpressionType.GreaterThanOrEqual, ">="},
                                                                                  { ExpressionType.LessThanOrEqual, "<="}
                                                                              };

        private Builder.Builder _builder { get; set; }

        public LambdaResolver(Builder.Builder builder)
        {
            _builder = builder;
        }

        #region helpers
        public   string GetColumnName<T>(Expression<Func<T, object>> selector)
        {
            return GetColumnName(GetMemberExpression(selector.Body));
        }

        public   string GetColumnName(Expression expression)
        {
            var member = GetMemberExpression(expression);
#if NETCOREAPP1_0
            var column = member.Member.GetCustomAttributes(false).OfType<DBColumnAttribute>().FirstOrDefault();//.GetCustomAttributes(false).OfType<DBColumnAttribute>().FirstOrDefault();
#else
            var column = member.Member.GetCustomAttributes(false).OfType<DBColumnAttribute>().FirstOrDefault();
#endif
            if (column != null)
                return column.Name;
            else
                return member.Member.Name;
        }

        public   string GetTableName<T>()
        {
            return GetTableName(typeof(T));
        }

        public   string GetTableName(Type type)
        {
#if NETCOREAPP1_0
            var column = type.GetTypeInfo().GetCustomAttributes(false).OfType<DBTableAttribute>().FirstOrDefault();//.GetCustomAttributes(false).OfType<DBColumnAttribute>().FirstOrDefault();
#else
            var column = type.GetCustomAttributes(false).OfType<DBTableAttribute>().FirstOrDefault();
#endif
            if (column != null)
            {
                var tname = column.Name;
                if (string.IsNullOrEmpty(tname))
                {
                    tname = type.Name;
                }

                return _builder.Adapter.Table(tname, column.Schema);
            }
            else
                return type.Name;
        }

        //private   string GetTableName(MemberExpression expression)
        //{
        //    return GetTableName(expression.Member.DeclaringType);
        //}

        public string GetTableName(SqlTableDefine tableDefine)
        {
            if (tableDefine.TableAttribute != null)
            {
                var tname = tableDefine.TableAttribute.Name;
                if (string.IsNullOrEmpty(tname))
                {
                    tname = tableDefine.Name;
                }

                return _builder.Adapter.Table(tname, tableDefine.TableAttribute.Schema);
            }
            else
                return tableDefine.Name;
        }

        private static BinaryExpression GetBinaryExpression(Expression expression)
        {
            if (expression is BinaryExpression)
                return expression as BinaryExpression;

            throw new ArgumentException("Binary expression expected");
        }

        private static MemberExpression GetMemberExpression(Expression expression)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.MemberAccess:
                    return expression as MemberExpression;
                case ExpressionType.Convert:
                    return GetMemberExpression((expression as UnaryExpression).Operand);
            }

            throw new ArgumentException("Member expression expected");
        }

#endregion
    }
}
