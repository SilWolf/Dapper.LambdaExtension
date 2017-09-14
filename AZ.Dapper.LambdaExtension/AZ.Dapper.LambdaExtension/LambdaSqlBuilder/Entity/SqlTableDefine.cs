using System;
using Dapper.LambdaExtension.LambdaSqlBuilder.Attributes;

namespace Dapper.LambdaExtension.LambdaSqlBuilder.Entity
{
    
    public class SqlTableDefine
    {
        public ZPTableAttribute TableAttribute { get; set; }

        public string Name { get; set; }
 

        public SqlTableDefine(ZPTableAttribute tableAttr,string name )
        {
            TableAttribute = tableAttr;
            if (tableAttr != null)
            {
                Name = tableAttr.Name;
                if (string.IsNullOrEmpty(tableAttr.Name))
                {
                    Name = name;
                }
            }
            else
            {
                Name = name;
            }
        }

        public SqlTableDefine(ZPTableAttribute tableAttr)
        {
            TableAttribute = tableAttr;
        }
    }
}
