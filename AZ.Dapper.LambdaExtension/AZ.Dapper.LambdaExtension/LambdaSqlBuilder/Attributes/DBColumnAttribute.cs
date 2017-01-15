using System;

namespace Dapper.LambdaExtension.LambdaSqlBuilder.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DBColumnAttribute : Attribute
    {
        public DBColumnAttribute(string name)
        {
            this.Name = name;
        }
        public string Name { get; set; }
    }
}
