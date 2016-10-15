using System;

namespace AZ.Dapper.LambdaExtension.Attributes
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
