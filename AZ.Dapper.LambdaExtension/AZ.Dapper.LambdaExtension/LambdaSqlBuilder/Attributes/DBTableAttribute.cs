using System;

namespace Dapper.LambdaExtension.LambdaSqlBuilder.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DBTableAttribute : Attribute
    {


        public DBTableAttribute(string name,string schema=null)
        {
            this.Name = name;
            this.Schema = schema;
        }

        public string Name { get; set; }

        public string Schema { get; set; }
    }
}
