using System;
using System.Collections.Generic;
 
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AZ.Dapper.LambdaExtension.Attributes;

namespace testdemo.Entities
{
    public class Test2
    {
        [LamKey]
        public int Id { get; set; }

        public string Name { get; set; }
    }
}
