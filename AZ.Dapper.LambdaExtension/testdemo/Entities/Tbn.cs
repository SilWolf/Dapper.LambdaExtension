using Dapper.Contrib.Extensions;

namespace testdemo.Entities
{
    [Table("tbn1")]
    public class Tbn
    {
        [Key]
        public int id { get; set; }

        public string name { get; set; }

        public string valpsin { get; set; }
    }
}
