using System;
using System.Collections.Generic;
 
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper.LambdaExtension.LambdaSqlBuilder.Attributes;

namespace testdemo.Entities
{
     //[DBTable("test2")]
    public class Test2
    {
        [DBKey(true)]
        
        public int Id { get; set; }

        [DBColumn("c_name")]
        public string Name { get; set; }
    }
}
