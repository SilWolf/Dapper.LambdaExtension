using Dapper.Contrib.Extensions;
using Dapper.LambdaExtension.LambdaSqlBuilder.Attributes;

namespace testdemo.Entities
{
    [DBTable("tbn1")]
    public class Tbn
    {
        [DBKey]
        public int id { get; set; }

        public string name { get; set; }

        public string valpsin { get; set; }
    }
}
