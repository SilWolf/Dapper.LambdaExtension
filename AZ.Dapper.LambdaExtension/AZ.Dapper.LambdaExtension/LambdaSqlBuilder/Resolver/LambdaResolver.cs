using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Dapper.LambdaExtension.LambdaSqlBuilder.Attributes;

namespace Dapper.LambdaExtension.LambdaSqlBuilder.Resolver
{
    [Serializable]
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
            var column = member.Member.GetCustomAttributes(false).OfType<DBColumnAttribute>().FirstOrDefault();
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
            var column = type.GetCustomAttributes(false).OfType<DBTableAttribute>().FirstOrDefault();
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

        private   string GetTableName(MemberExpression expression)
        {
            return GetTableName(expression.Member.DeclaringType);
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
