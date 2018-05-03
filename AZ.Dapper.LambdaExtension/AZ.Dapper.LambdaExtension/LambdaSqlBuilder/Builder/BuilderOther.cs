﻿using System;
using System.ComponentModel;
using Dapper.LambdaExtension.Helpers;
using Dapper.LambdaExtension.LambdaSqlBuilder.Resolver.ExpressionTree;

namespace Dapper.LambdaExtension.LambdaSqlBuilder.Builder
{
    partial class Builder
    {
        internal string JoinSubAliasTableName;

        public void Join(string originalTableName, string joinTableName, string leftField, string rightField,JoinType joinType)
        {
            var joinTypeStr = GetJoinType(joinType);

            var joinString = string.Format("{3} {0} ON {1} = {2}",
                                           _adapter.Table(joinTableName,""),
                                           _adapter.Field(originalTableName, leftField),
                                           _adapter.Field(joinTableName, rightField),joinTypeStr);
            _tableNames.Add(joinTableName);
            _joinExpressions.Add(joinString);
            _splitColumns.Add(rightField);
        }

        public void JoinSub(SqlExpBase sqlExp,string originalTableName, string joinTableName, string leftField, string rightField, JoinType joinType)
        {
           
            var joinTypeStr = GetJoinType(joinType);

            //var aliasTname = $"join_" + DateTime.Now.Ticks;
            var aliasTname = $"j_" + EnvHelper.GetRandomString(6, true, true, false);// $"join_" + DateTime.Now.Ticks;

            sqlExp.JoinSubAliasTableName = aliasTname;
    
            JoinSubAliasTableName = aliasTname;
            var subQueryStr = sqlExp.SqlString;

            var joinString = string.Format("{3} ({0}) {4} ON {1} = {4}.{2}",
                subQueryStr,
                _adapter.Field(originalTableName, leftField),
                 _adapter.Field(rightField), joinTypeStr,aliasTname);
            _tableNames.Add(joinTableName);
            _joinExpressions.Add(joinString);
            _splitColumns.Add(rightField);
        }

        private static string GetJoinType(JoinType joinType)
        {
            string joinTypeStr;
            switch (joinType)
            {
                case JoinType.InnerJoin:
                    joinTypeStr = "INNER JOIN";
                    break;
                case JoinType.LeftJoin:
                    joinTypeStr = "LEFT JOIN";
                    break;
                case JoinType.RightJoin:
                    joinTypeStr = "RIGHT JOIN";
                    break;
                case JoinType.LeftOuterJoin:
                    joinTypeStr = "LEFT OUTER JOIN";
                    break;
                case JoinType.RightOuterJoin:
                    joinTypeStr = "RIGHT OUTER JOIN";
                    break;
                default:
                    joinTypeStr = "JOIN";
                    break;
            }
            return joinTypeStr;
        }

        public void QuerySub(SqlExpBase sqlExp)
        {
             
            //var aliasTname = $"query_" + DateTime.Now.Ticks;
            JoinSubAliasTableName = sqlExp.JoinSubAliasTableName;
            //sqlExp.JoinSubAliasTableName = aliasTname;

            var subQueryStr = sqlExp.SqlString;

            var subTableString = string.Format("({0}) AS {1}", subQueryStr,Adapter.Table(sqlExp.JoinSubAliasTableName,string.Empty));
           
            _tableNames.Clear();
            _tableNames.Add(subTableString);
       
        }



        public void OrderBy(string tableName, string fieldName, bool desc = false)
        {
            var order = $"{Adapter.Table(tableName,string.Empty)}.{_adapter.Field(fieldName)}";
            if (desc) order += " DESC";

            _sortList.Add(order);
        }

        public void Select(string tableName)
        {
            var selectionString = string.Format("{0}.*", _adapter.Table(tableName,_schema));
            _selectionList.Add(selectionString);
        }

        public void Select(string tableName, string fieldName)
        {
            _selectionList.Add(_adapter.Field(tableName, fieldName));
        }

        public void Select(string tableName, string fieldName,string aliasName)
        {
            _selectionList.Add(_adapter.Field(tableName, fieldName)+" AS "+_adapter.Field(aliasName));
        }

        public void Select(string tableName, string fieldName, SelectFunction selectFunction, string aliasName)
        {
            string name = string.IsNullOrEmpty(aliasName) ? fieldName : aliasName;
            name = _adapter.Field(name);

            var fname = fieldName;
            if (fieldName != "*")
            {
                fname = _adapter.Field(tableName, fieldName);
            }
            var selectionString = string.Format("{0}({1}) AS {2}", selectFunction.ToString(), fname, name);
            _selectionList.Add(selectionString);
        }

        //public void SelectSubSql(string tableName, string fieldName, SelectFunction selectFunction, string aliasName)
        //{
        //    string name = string.IsNullOrEmpty(aliasName) ? fieldName : aliasName;
        //    name = _adapter.Field(name);

        //    var fname = fieldName;
        //    if (fieldName != "*")
        //    {
        //        fname = _adapter.Field(tableName, fieldName);
        //    }
        //    var selectionString = string.Format("{0}({1}) AS {2}", selectFunction.ToString(), fname, name);
        //    _selectionList.Add(selectionString);
        //}

        public void GroupBy(string tableName, string fieldName)
        {
            _groupingList.Add(_adapter.Field(tableName, fieldName));
        }
    }

  
}
