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

            ZPTableAttribute tableAttr=new ZPTableAttribute("");
 

            if (EnvHelper.IsNetFX)
            {
#if NETCOREAPP1_0 ||NETSTANDARD1_6
#else
                tableAttr = type.GetCustomAttribute<ZPTableAttribute>();
#endif
            }
            else
            {
                tableAttr = type.GetTypeInfo().GetCustomAttribute<ZPTableAttribute>();
            }

            var sqlTableDef = new SqlTableDefine(tableAttr, name);

            //处理列定义
            var colDeflist = new List<SqlColumnDefine>();

            var columns = type.GetProperties();

            foreach (var cp in columns)
            {
                var ignore = cp.GetCustomAttribute<ZPIgnoreAttribute>();

                if (ignore == null)
                {
                    var keyAttr = cp.GetCustomAttribute<ZPKeyAttribute>();
                    var columnAttr = cp.GetCustomAttribute<ZPColumnAttribute>();
                    var dataTypeAttr = cp.GetCustomAttribute<ZPCustomeDataTypeAttribute>();

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

            bool isValueType = false;
            if (!EnvHelper.IsNetFX)
            {
                isValueType = theType.GetTypeInfo().IsValueType;
            }
            else
            {
                isValueType = theType.GetTypeInfo().IsValueType;
            }
            if (!isValueType)
            {
                return true;
            }
 

            var isgenericType = false;
            if (!EnvHelper.IsNetFX)
            {
                isgenericType = theType.GetTypeInfo().IsGenericType;
            }
            else
            {
                isgenericType = theType.GetTypeInfo().IsGenericType;
            }
 
            return (isgenericType && theType.GetGenericTypeDefinition() == typeof(Nullable<>));
        }
    }
}
