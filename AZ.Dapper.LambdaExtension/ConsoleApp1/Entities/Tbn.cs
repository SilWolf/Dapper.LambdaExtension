using Dapper.Contrib.Extensions;
using System;
using Dapper.LambdaExtension.LambdaSqlBuilder.Attributes;

namespace testdemo.Entities
{
    [ZPTable("tbn1")]
    public class Tbn
    {
        [ZPKey]
        public int id { get; set; }

        public string name { get; set; }

        public string valpsin { get; set; }
    }
}
