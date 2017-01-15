using System;

namespace Dapper.LambdaExtension.LambdaSqlBuilder.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DBCustomeDataTypeAttribute : Attribute
    {
        public string DataType { get; set; }

        /// <summary>
        /// define custome filed type for column.
        /// </summary>
        /// <param name="dataType">like : varchar(50)</param>
        public DBCustomeDataTypeAttribute(string dataType)
        {
            DataType = dataType;
        }
    }
}
