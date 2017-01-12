using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper.LambdaExtension.LambdaSqlBuilder.Attributes;

namespace testdemo.Entities
{
    [LamTable("o_test3")]
    public class Test3
    {
        [LamKey(true)]
        [LamColumn("id")]
        public int Id { get; set; }

        [LamColumn("v_name")]
        public string VName { get; set; }
    }
}
