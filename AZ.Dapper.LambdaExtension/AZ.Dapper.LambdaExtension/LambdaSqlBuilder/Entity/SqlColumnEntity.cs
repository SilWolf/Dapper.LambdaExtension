using System;

namespace Dapper.LambdaExtension.LambdaSqlBuilder.Entity
{
    [Serializable]
    public class SqlColumnEntity
    {
        public object Value { get; set; }

        public string AliasName { get; set; }

        public SqlColumnEntity(string name , object value)
        {
            this.AliasName = name;
            this.Value = value;
        }
    }
}
