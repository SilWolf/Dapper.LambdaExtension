using System;
using System.Collections.Generic;
using System.Text;
using Dapper.LambdaExtension.Extentions;
using Dapper.LambdaExtension.LambdaSqlBuilder.Attributes;

namespace ConsoleApp1.Entities
{
    public class Test3:TableBase<Test3,NpgDbFactory>
    {
        [DBKey(true)]
        public int Id { get; set; }

        public string MyName { get; set; }

        public DateTime CreateDate { get; set; }
    }
}
