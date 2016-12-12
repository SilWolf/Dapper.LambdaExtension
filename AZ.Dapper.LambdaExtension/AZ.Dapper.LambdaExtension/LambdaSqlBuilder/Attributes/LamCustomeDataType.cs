using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZ.Dapper.LambdaExtension.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class LamCustomeDataTypeAttribute : Attribute
    {
        public string DataType { get; set; }

        /// <summary>
        /// define custome filed type for column.
        /// </summary>
        /// <param name="dataType">like : varchar(50)</param>
        public LamCustomeDataTypeAttribute(string dataType)
        {
            DataType = dataType;
        }
    }
}
