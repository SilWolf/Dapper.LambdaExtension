using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
 
//using System.Web;
using Dapper.LambdaExtension.LambdaSqlBuilder.Attributes;
 
using Dapper.LambdaExtension.Helpers;
#if NETCOREAPP1_0 || NETSTANDARD1_6

using Microsoft.Extensions.DependencyModel;
#endif

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

            var mappedTypeList = new List<Type>();

            if (!EnvHelper.IsNetFX)
            {
#if NETCOREAPP1_0 || NETSTANDARD1_6
                try
                {
 
                    var runLiblist = DependencyContext.Default.RuntimeLibraries.ToList();


                    foreach (var rl in runLiblist)
                    {
                        var assemlist = rl.GetDefaultAssemblyNames(DependencyContext.Default).ToList();
                        foreach (var asm in assemlist)
                        {
                            var assem = Assembly.Load(asm);
                            var types = assem.GetExportedTypes().ToList();
                            var mappedtypes = types.Where(f =>
                                f.GetProperties().Any(
                                    p =>
                                        p.GetCustomAttributes(false).Any(
                                            a => a.GetType().Name == aliasType.Name)));

                            mappedTypeList.AddRange(mappedtypes);
                        }
                    }
 
 
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
#endif
            }
            else
            {

#if NETCOREAPP1_0 || NETSTANDARD1_6
#else
                var assemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();

                AppDomain.CurrentDomain.AssemblyLoad += (sender, args) =>
                {
                    var aliasType2 = typeof(DBColumnAttribute);
                    var mappedTypeList2 = new List<Type>();
                    var assembly = args.LoadedAssembly;

                    var mappedTypes = assembly.GetExportedTypes().Where(
                        f =>
                            f.GetProperties().Any(
                                p =>
                                    p.GetCustomAttributes(false).Any(
                                        a => a.GetType().Name == aliasType2.Name)));

                    mappedTypeList2.AddRange(mappedTypes);

                    foreach (var mappedType in mappedTypeList2)
                    {
                        SqlMapper.SetTypeMap(mappedType, new CustomPropertyTypeMap(mappedType, _fu));
                    }
                }; //CurrentDomain_AssemblyLoad};

                

                foreach (var assembly in assemblies)
                {
                    try
                    {
                        var mappedTypes = assembly.GetExportedTypes().Where(
                            f =>
                                f.GetProperties().Any(
                                    p =>
                                        p.GetCustomAttributes(false).Any(
                                            a => a.GetType().Name == aliasType.Name)));

                        mappedTypeList.AddRange(mappedTypes);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
#endif
            }
            foreach (var mappedType in mappedTypeList)
            {
                SqlMapper.SetTypeMap(mappedType, new CustomPropertyTypeMap(mappedType, _fu));
            }

        }
 

        static string GetColumnAttribute(MemberInfo member)
        {
            if (member == null) return null;
            if (!EnvHelper.IsNetFX)
            {

                var attrib = member.GetCustomAttribute<DBColumnAttribute>(false);
                //var attrib = (DBColumnAttribute)Attribute.GetCustomAttribute(member, typeof(DBColumnAttribute), false);
                return attrib == null ? member.Name : attrib.Name;//if not define zpcolumn attribute on an propertity/field then use it's own name.

            }
            else
            {
                var attrib = member.GetCustomAttribute<DBColumnAttribute>(false);// (DBColumnAttribute)Attribute.GetCustomAttribute(member, typeof(DBColumnAttribute), false);
                return attrib == null ? member.Name : attrib.Name;//if not define zpcolumn attribute on an propertity/field then use it's own name.

            }
        }

    }
}
