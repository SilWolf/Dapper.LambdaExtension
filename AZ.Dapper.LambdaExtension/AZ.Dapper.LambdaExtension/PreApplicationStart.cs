using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
//using System.Web;
using Dapper.LambdaExtension.LambdaSqlBuilder.Attributes;
 

//[assembly: PreApplicationStartMethod(typeof(Dapper.LambdaExtension.PreApplicationStart), "RegisterTypeMaps")]

namespace Dapper.LambdaExtension
{
   
    public static class PreApplicationStart
    {

        static Func<Type, string, PropertyInfo> _fu = (type, columnName) => type.GetProperties().FirstOrDefault(prop => GetColumnAttribute(prop) == columnName);
        private static bool _initialized = false;
 
        public static void RegisterTypeMaps()
        {
            if (_initialized)
            {
                return;
            }

            _initialized = true;

            var aliasType = typeof(DBColumnAttribute);

            var mappedTypeList=new List<Type>();

            var assemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
            foreach (var assembly in assemblies)
            {
                var mappedTypes = assembly.GetTypes().Where(
                 f =>
                 f.GetProperties().Any(
                     p =>
                     p.GetCustomAttributes(false).Any(
                         a => a.GetType().Name == aliasType.Name)));

                mappedTypeList.AddRange(mappedTypes);
            }
            
            foreach (var mappedType in mappedTypeList)
            {
                SqlMapper.SetTypeMap(mappedType,new CustomPropertyTypeMap(mappedType,_fu));
            }
        }

        static string GetColumnAttribute(MemberInfo member)
        {
            if (member == null) return null;

            var attrib = (DBColumnAttribute)Attribute.GetCustomAttribute(member, typeof(DBColumnAttribute), false);
            return attrib == null ? member.Name : attrib.Name;//if not define zpcolumn attribute on an propertity/field then use it's own name.
        }

    }
}
