﻿using System;
using System.Linq.Expressions;
using AZ.Dapper.LambdaExtension.Resolver.ExpressionTree;

namespace AZ.Dapper.LambdaExtension.Resolver
{
    partial class LambdaResolver
    {
        void BuildSql(Node node)
        {
            BuildSql((dynamic)node);
        }

        void BuildSql(ValueNode node)
        {
            string fieldName = "1";
            string op = _operationDictionary[ExpressionType.NotEqual];
            bool flag = false;
            bool.TryParse(node.Value.ToString(), out flag);
            if (flag) op = _operationDictionary[ExpressionType.Equal];
            _builder.AddCondition(fieldName, op, fieldName);
        }

        void BuildSql(LikeNode node)
        {
            if (node.Method == LikeMethod.Equals)
            {
                _builder.AddCondition(node.MemberNode.TableName, node.MemberNode.FieldName, _operationDictionary[ExpressionType.Equal], node.Value);
            }
            else
            {
                string value = node.Value;
                switch (node.Method)
                {
                    case LikeMethod.StartsWith:
                        value = node.Value + _builder.Adapter.LikeChars();
                        break;
                    case LikeMethod.EndsWith:
                        value = _builder.Adapter.LikeChars()+ node.Value;
                        break;
                    case LikeMethod.Contains:
                        value = _builder.Adapter.LikeChars() + node.Value + _builder.Adapter.LikeChars();
                        break;
                }
                _builder.AddLikeCondition(node.MemberNode.TableName, node.MemberNode.FieldName, value);
            }
        }

        void BuildSql(OperationNode node)
        {
            BuildSql((dynamic)node.Left, (dynamic)node.Right, node.Operator);
        }

        void BuildSql(MemberNode memberNode)
        {
            _builder.AddCondition(memberNode.TableName, memberNode.FieldName, _operationDictionary[ExpressionType.Equal], true);
        }

        void BuildSql(SingleOperationNode node)
        {
            if (node.Operator == ExpressionType.Not)
                _builder.Not();
            BuildSql(node.Child);
        }

        void BuildSql(MemberNode memberNode, ValueNode valueNode, ExpressionType op)
        {
            if (valueNode.Value == null)
            {
                ResolveNullValue(memberNode, op);
            }
            else
            {
                _builder.AddCondition(memberNode.TableName, memberNode.FieldName, _operationDictionary[op], valueNode.Value);
            }
        }

        void BuildSql(ValueNode valueNode, MemberNode memberNode, ExpressionType op)
        {
            BuildSql(memberNode, valueNode, op);
        }

        void BuildSql(MemberNode leftMember, MemberNode rightMember, ExpressionType op)
        {
            _builder.AddCondition(leftMember.TableName, leftMember.FieldName, _operationDictionary[op], rightMember.TableName, rightMember.FieldName);
        }

        void BuildSql(SingleOperationNode leftMember, Node rightMember, ExpressionType op)
        {
            if (leftMember.Operator == ExpressionType.Not)
                BuildSql(leftMember as Node, rightMember, op);
            else
                BuildSql((dynamic)leftMember.Child, (dynamic)rightMember, op);
        }

        void BuildSql(Node leftMember, SingleOperationNode rightMember, ExpressionType op)
        {
            BuildSql(rightMember, leftMember, op);
        }

        void BuildSql(Node leftNode, Node rightNode, ExpressionType op)
        {
            _builder.BeginExpression();
            BuildSql((dynamic)leftNode);
            ResolveOperation(op);
            BuildSql((dynamic)rightNode);
            _builder.EndExpression();
        }

        void ResolveNullValue(MemberNode memberNode, ExpressionType op)
        {
            switch (op)
            {
                case ExpressionType.Equal:
                    _builder.AddCondition(true, memberNode.TableName, memberNode.FieldName);
                    break;
                case ExpressionType.NotEqual:
                    _builder.AddCondition(false, memberNode.TableName, memberNode.FieldName);
                    break;
            }
        }

        void ResolveSingleOperation(ExpressionType op)
        {
            switch (op)
            {
                case ExpressionType.Not:
                    _builder.Not();
                    break;
            }
        }

        void ResolveOperation(ExpressionType op)
        {
            switch (op)
            {
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                    _builder.And();
                    break;
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    _builder.Or();
                    break;
                default:
                    throw new ArgumentException(string.Format("Unrecognized binary expression operation '{0}'", op.ToString()));
            }
        }
    }
}
