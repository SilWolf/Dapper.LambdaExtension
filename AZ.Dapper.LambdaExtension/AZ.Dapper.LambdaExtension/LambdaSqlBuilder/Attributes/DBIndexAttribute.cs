using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.LambdaExtension.LambdaSqlBuilder.Attributes
{

    [AttributeUsage(AttributeTargets.Property)]
    public class DBIndexAttribute:Attribute
    {
        /// <summary>
        /// 索引名称
        /// </summary>
        public string IndexName { get; set; }

        /// <summary>
        /// 是否唯一索引
        /// </summary>
        public bool Unique { get; set; }

        /// <summary>
        /// 索引排序
        /// </summary>
        public bool Asc { get; set; }

        public DBIndexAttribute(string indexName="",bool asc=true,bool unique=false)
        {
            this.IndexName = indexName;
            this.Unique = unique;
            this.Asc = asc;
        }
    }
}
