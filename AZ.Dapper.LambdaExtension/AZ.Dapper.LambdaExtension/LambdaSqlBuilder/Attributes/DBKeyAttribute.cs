using System;

namespace Dapper.LambdaExtension.LambdaSqlBuilder.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DBKeyAttribute : Attribute
    {
        public DBKeyAttribute(bool increment)
        {
            this.Increment = increment;
        }

        public DBKeyAttribute()
            : this(false)
        {

        }
        /// <summary>
        /// 是否是自增
        /// </summary>
        public bool Increment { get; set; }
    }
}
