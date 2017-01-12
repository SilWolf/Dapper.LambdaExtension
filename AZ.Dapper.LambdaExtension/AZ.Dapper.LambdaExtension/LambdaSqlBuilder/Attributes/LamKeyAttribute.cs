using System;

namespace Dapper.LambdaExtension.LambdaSqlBuilder.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class LamKeyAttribute : Attribute
    {
        public LamKeyAttribute(bool increment)
        {
            this.Increment = increment;
        }

        public LamKeyAttribute()
            : this(false)
        {

        }
        /// <summary>
        /// 是否是自增
        /// </summary>
        public bool Increment { get; set; }
    }
}
