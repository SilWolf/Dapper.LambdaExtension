using System;
using System.Collections.Generic;
 
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper.LambdaExtension.LambdaSqlBuilder.Attributes;

namespace testdemo.Entities
{
     
    public class Test2
    {
        [LamKey(true)]
        
        public int Id { get; set; }

        [LamColumn("c_name")]
        public string Name { get; set; }
    }
}
