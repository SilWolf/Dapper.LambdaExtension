using System;
using System.Collections.Generic;
using System.Reflection;
using Dapper.LambdaExtension.LambdaSqlBuilder.Attributes;
using Dapper.LambdaExtension.LambdaSqlBuilder.Entity;

namespace Dapper.LambdaExtension.Helpers
{
    public static class EntityHelper
    {
        public static Tuple<SqlTableDefine, List<SqlColumnDefine>> GetEntityDefine<T>()
        {
            var type = typeof(T);

            return GetEntityDefine(type);
        }

        private static Tuple<SqlTableDefine, List<SqlColumnDefine>> GetEntityDefine(Type type)
        {
            //处理表定义
            var name = type.Name;
#if NETCOREAPP1_0
            var tableAttr = type.GetTypeInfo().GetCustomAttribute<DBTableAttribute>();
#else
            var tableAttr = type.GetCustomAttribute<DBTableAttribute>();
#endif 
            var sqlTableDef = new SqlTableDefine(tableAttr, name);

            //处理列定义
            var colDeflist = new List<SqlColumnDefine>();

            var columns = type.GetProperties();

            foreach (var cp in columns)
            {
                var ignore = cp.GetCustomAttribute<DBIgnoreAttribute>();

                if (ignore == null)
                {
                    var keyAttr = cp.GetCustomAttribute<DBKeyAttribute>();
                    var columnAttr = cp.GetCustomAttribute<DBColumnAttribute>();
                    var dataTypeAttr = cp.GetCustomAttribute<DBCustomeDataTypeAttribute>();

                    var cname = cp.Name;

                    var alias = cname;
                    if (columnAttr != null)
                    {
                        alias = columnAttr.Name;
                    }

                    // edit by cheery 2017-2-21
                    var nullable = true;
                    // 如果是Key 不允许空
                    if (keyAttr != null)
                    {
                        nullable = false;
                    }
                    // 如果字段定义上有是否允许空标记 则依赖该标记
                    else if (columnAttr?.Nullable != null)
                    {
                        nullable = columnAttr.Nullable.Value;
                    }
                    // 否则 根据类型判断
                    else
                    {
                        nullable = cp.PropertyType.IsNullableType();
                    }

                    //var nullable = keyAttr == null && (columnAttr?.Nullable ?? cp.PropertyType.IsNullableType());

                    var cd = new SqlColumnDefine(cname, alias, null, cp.PropertyType, nullable, columnAttr, keyAttr, dataTypeAttr);

                    colDeflist.Add(cd);
                }
            }

            return new Tuple<SqlTableDefine, List<SqlColumnDefine>>(sqlTableDef, colDeflist);
        }


        //public static Tuple<SqlTableDefine, List<SqlColumnDefine>> GetEntityDefine<T>(this T t) where T : class
        //{
        //    return GetEntityDefine<T>();
        //}

        public static Tuple<SqlTableDefine, List<SqlColumnDefine>> GetEntityDefines(this Type t)
        {
            return GetEntityDefine(t);
        }

        public static bool IsNullableType(this Type theType)
        {
            // edit by cheery 2017-2-21
            // 如果是引用类型，默认允许空
#if NETCOREAPP1_0
            if (!theType.GetTypeInfo().IsValueType)
#else

            if (!theType.IsValueType)
#endif
            {
                return true;
            }

            var isgenericType = false;
#if NETCOREAPP1_0
            isgenericType = theType.GetTypeInfo().IsGenericType;
#else

            isgenericType = theType.IsGenericType;
#endif
            return (isgenericType && theType.GetGenericTypeDefinition() == typeof(Nullable<>));
        }
    }
}
