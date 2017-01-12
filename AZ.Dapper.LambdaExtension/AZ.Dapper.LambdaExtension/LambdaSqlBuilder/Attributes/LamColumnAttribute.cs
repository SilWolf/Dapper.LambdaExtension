using System;

namespace Dapper.LambdaExtension.LambdaSqlBuilder.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class LamColumnAttribute : Attribute
    {
        public LamColumnAttribute(string name)
        {
            this.Name = name;
        }
        public string Name { get; set; }
    }
}
