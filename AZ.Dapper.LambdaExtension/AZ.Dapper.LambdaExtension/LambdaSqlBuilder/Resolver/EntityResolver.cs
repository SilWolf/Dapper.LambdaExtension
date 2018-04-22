﻿using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Dapper.LambdaExtension.Helpers;
using Dapper.LambdaExtension.LambdaSqlBuilder.Attributes;
using Dapper.LambdaExtension.LambdaSqlBuilder.Entity;

namespace Dapper.LambdaExtension.LambdaSqlBuilder.Resolver
{
    partial class LambdaResolver
    {
        public void ResolveUpdate<T>()
        {
            string tableName = GetTableName<T>();
            ResolveParameter<T>(tableName);
        }

        public void ResolveUpdate<T>(Expression<Func<T, object>> expression, object value)
        {

            ResolveParameter<T>(expression, value);
        }

        public void ResolveUpdate(Type type, object obj)
        {
            string tableName = GetTableName(type);
            ResolveParameter<object>(tableName, obj);
        }

        public void ResolveInsert<T>(bool key)
        {
            string tableName = GetTableName<T>();
            ResolveParameter<T>(key, tableName);
        }

        public void ResolveInsert(bool key, Type type, object obj)
        {
            string tableName = GetTableName(type);
            ResolveParameter<object>(key, tableName);
        }

        public void ResolveInsert(bool key, SqlTableDefine tableDefine, List<SqlColumnDefine> columnDefines)
        {
            string tableName = GetTableName(tableDefine);
            ResolveParameter(key, tableName, columnDefines);
        }

        public void ResolveInsertValues<T>(bool key)
        {
            string tableName = GetTableName<T>();
            ResolveParameter<T>(key, tableName);
        }

        public void ResolveInsertValues(bool key, Type type, object obj)
        {
            string tableName = GetTableName(type);
            ResolveParameter<object>(key, tableName);
        }

        public void ResolveInsertValues(bool key, SqlTableDefine tableDefine, List<SqlColumnDefine> columnDefines)
        {
            string tableName = GetTableName(tableDefine);
            ResolveParameter(key, tableName, columnDefines);
        }


        private void ResolveParameter<T>(string tableName)
        {
            var ps = GetPropertyInfos<T>();
            foreach (PropertyInfo item in ps)
            {
                //object obj = item.GetValue(entity, null);

                var propname = item.Name;

                var fieldAlias = propname;

                //resolve custome column name
                var colAttr = item.GetCustomAttribute<DBColumnAttribute>();
                var colIgnore = item.GetCustomAttribute<DBIgnoreAttribute>();
                if (colIgnore != null)
                {
                    continue;
                }
                if (colAttr != null)
                {
                    if (!string.IsNullOrEmpty(colAttr.Name))
                    {
                        fieldAlias = colAttr.Name;
                    }
                }

                _builder.AddSection(tableName, item.Name, fieldAlias, _operationDictionary[ExpressionType.Equal], null);
            }
        }


        private void ResolveParameter<T>(Expression<Func<T, object>> expression, object value)
        {
 
            string tableName = GetTableName<T>();

            var columnName = GetColumnName(expression);

            //var memberExpression = GetMemberExpression(expression.Body);

            if (!EnvHelper.IsNetFX)
            {
 
                _builder.AddSection(tableName, columnName, columnName, _operationDictionary[ExpressionType.Equal], value);

            }
            else
            {
                _builder.AddSection(tableName, columnName, columnName, _operationDictionary[ExpressionType.Equal], value);
            }

        }

        private void ResolveParameter<T>(string tableName, object objs)
        {
            var ps = GetPropertyInfos<T>();
            foreach (PropertyInfo item in ps)
            {
                object obj = item.GetValue(objs, null);

                var propname = item.Name;

                var fieldAlias = propname;

                //resolve custome column name
                var colAttr = item.GetCustomAttribute<DBColumnAttribute>();
                var colIgnore = item.GetCustomAttribute<DBIgnoreAttribute>();
                if (colIgnore != null)
                {
                    continue;
                }
                if (colAttr != null)
                {
                    if (!string.IsNullOrEmpty(colAttr.Name))
                    {
                        fieldAlias = colAttr.Name;
                    }
                }

                _builder.AddSection(tableName, item.Name, fieldAlias, _operationDictionary[ExpressionType.Equal], obj);
            }
        }

        private void ResolveParameter<T>(bool key, string tableName)
        {
            _builder.UpdateInsertKey(key);

            var ps = GetPropertyInfos<T>();
            foreach (PropertyInfo item in ps)
            {
                //z object obj = item.GetValue(entity, null);

                var propname = item.Name;

                var fieldAlias = propname;

                //resolve custome column name
                var colAttr = item.GetCustomAttribute<DBColumnAttribute>();
                var colIgnore = item.GetCustomAttribute<DBIgnoreAttribute>();
                if (colIgnore != null)
                {
                    continue;
                }
                if (colAttr != null)
                {
                    if (!string.IsNullOrEmpty(colAttr.Name))
                    {
                        fieldAlias = colAttr.Name;
                    }
                }

                _builder.AddSection(tableName, propname, fieldAlias, null);
            }
        }

      

        private void ResolveParameter(bool key, string tableName, List<SqlColumnDefine> columnsDefines)
        {
            _builder.UpdateInsertKey(key);


            foreach (var item in columnsDefines)
            {


                var propname = item.Name;

                var fieldAlias = propname;

                //resolve custome column name
                //var colAttr = item.
                var colIgnore = item.IgnoreAttribute;
                if (colIgnore != null)
                {
                    continue;
                }


                if (!string.IsNullOrEmpty(item.AliasName))
                {
                    fieldAlias = item.AliasName;
                }


                _builder.AddSection(tableName, propname, fieldAlias, null);
            }
        }

        //private IEnumerable<PropertyInfo> GetPropertyInfos<T>(T entity)
        //{
        //    Type type = typeof(T);// entity.GetType();
        //    var ps = type.GetProperties().Where(m =>
        //    {
        //        var obj = m.GetCustomAttributes(typeof(DBKeyAttribute), false).FirstOrDefault();
        //        if (obj != null)
        //        {
        //            DBKeyAttribute key = obj as DBKeyAttribute;
        //            return !key.Increment;
        //        }
        //        return true;
        //    });

        //    return ps;
        //}

        internal IEnumerable<PropertyInfo> GetPropertyInfos<T>()
        {
            Type type = typeof(T);// entity.GetType();
            var ps = type.GetProperties().Where(m =>
            {
                var obj = m.GetCustomAttributes(typeof(DBKeyAttribute), false).FirstOrDefault();
                if (obj != null)
                {
                    DBKeyAttribute key = obj as DBKeyAttribute;
                    return !key.Increment;
                }
                return true;
            });

            return ps;
        }
    }
}
