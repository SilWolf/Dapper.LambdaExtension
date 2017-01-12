using Dapper.Contrib.Extensions;
using Dapper.LambdaExtension.LambdaSqlBuilder.Attributes;

namespace testdemo.Entities
{
    [LamTable("tbn1")]
    public class Tbn
    {
        [LamKey]
        public int id { get; set; }

        public string name { get; set; }

        public string valpsin { get; set; }
    }
}
