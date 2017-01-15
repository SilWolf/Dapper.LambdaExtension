using System;
using Dapper.LambdaExtension.LambdaSqlBuilder.Attributes;

namespace Dapper.LambdaExtension.LambdaSqlBuilder.Entity
{
    [Serializable]
    public class SqlTableDefine
    {
        public DBTableAttribute TableAttribute { get; set; }

        public string Name { get; set; }
 

        public SqlTableDefine(DBTableAttribute tableAttr,string name )
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
    }
}
