using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AZ.Dapper.LambdaExtension.Attributes;

namespace AZ.Dapper.LambdaExtension.LambdaSqlBuilder.Entity
{
    [Serializable]
    public class SqlTableDefine
    {
        public LamTableAttribute TableAttribute { get; set; }

        public string Name { get; set; }
 

        public SqlTableDefine(LamTableAttribute tableAttr,string name )
        {
            TableAttribute = tableAttr;
            Name = name;
            
        }
    }
}
