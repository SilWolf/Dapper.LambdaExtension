using System;

namespace AZ.Dapper.LambdaExtension.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class LamTableAttribute : Attribute
    {
        public LamTableAttribute(string name)
        {
            this.Name = name;
        }

        public string Name { get; set; }
    }
}
