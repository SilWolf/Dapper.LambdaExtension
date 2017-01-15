using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper.LambdaExtension.LambdaSqlBuilder.Attributes;

namespace testdemo.Entities
{
    [DBTable("o_test3")]
    public class Test3
    {
        [DBKey(true)]
        [DBColumn("id")]
        public int Id { get; set; }

        [DBColumn("v_name")]
        public string VName { get; set; }
    }
}
