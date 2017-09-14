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
        public void ResolveUpdate<T>(T entity)
        {
            string tableName = GetTableName<T>();
            ResolveParameter<T>(tableName, entity);
        }

        public void ResolveUpdate(Type type, object obj)
        {
            string tableName = GetTableName(type);
            ResolveParameter<object>(tableName, obj);
        }

        public void ResolveInsert<T>(bool key, T entity)
        {
            string tableName = GetTableName<T>();
            ResolveParameter<T>(key, tableName, entity);
        }

        public void ResolveInsert(bool key, Type type, object obj)
        {
            string tableName = GetTableName(type);
            ResolveParameter<object>(key, tableName, obj);
        }

        public void ResolveInsert(bool key, SqlTableDefine tableDefine, List<SqlColumnDefine> columnDefines)
        {
            string tableName = GetTableName(tableDefine);
            ResolveParameter(key,tableName, columnDefines);
        }

        private void ResolveParameter<T>(string tableName, T entity)
        {
            var ps = GetPropertyInfos<T>(entity);
            foreach (PropertyInfo item in ps)
            {
                object obj = item.GetValue(entity, null);

                var propname = item.Name;

                var fieldAlias = propname;

                //resolve custome column name
                var colAttr = item.GetCustomAttribute<ZPColumnAttribute>();
                var colIgnore = item.GetCustomAttribute<ZPIgnoreAttribute>();
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

                _builder.AddSection(tableName, item.Name,fieldAlias, _operationDictionary[ExpressionType.Equal], obj);
            }
        }

        private void ResolveParameter<T>(bool key, string tableName, T entity)
        {
            _builder.UpdateInsertKey(key);

            var ps = GetPropertyInfos<T>(entity);
            foreach (PropertyInfo item in ps)
            {
                object obj = item.GetValue(entity, null);

                var propname = item.Name;

                var fieldAlias = propname;

                //resolve custome column name
                var colAttr = item.GetCustomAttribute<ZPColumnAttribute>();
                var colIgnore = item.GetCustomAttribute<ZPIgnoreAttribute>();
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

                _builder.AddSection(tableName, propname,fieldAlias, obj);
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
                

                _builder.AddSection(tableName, propname, fieldAlias,null);
            }
        }

        private IEnumerable<PropertyInfo> GetPropertyInfos<T>(T entity)
        {
            Type type = typeof(T);// entity.GetType();
            var ps = type.GetProperties().Where(m =>
            {
                var obj = m.GetCustomAttributes(typeof(ZPKeyAttribute), false).FirstOrDefault();
                if (obj != null)
                {
                    ZPKeyAttribute key = obj as ZPKeyAttribute;
                    return !key.Increment;
                }
                return true;
            });

            return ps;
        }
    }
}
