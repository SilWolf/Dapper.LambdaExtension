using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZ.Dapper.LambdaExtension.LambdaSqlBuilder
{
    public class DbTypeMap
    {
       // public bool ShouldQuoteValue;
        public Dictionary<Type, string> ColumnTypeMap = new Dictionary<Type, string>();
        public Dictionary<Type, DbType> ColumnDbTypeMap = new Dictionary<Type, DbType>();

        public void Set<T>(DbType dbtype, string columnDefine)
        {
            //ShouldQuoteValue = fieldDefinition != "INTEGER"
            //  && fieldDefinition != "BIGINT"
            //  && fieldDefinition != "DOUBLE"
            //  && fieldDefinition != "DECIMAL"
            //  && fieldDefinition != "BOOL";

            ColumnTypeMap[typeof(T)] = columnDefine;
            ColumnDbTypeMap[typeof(T)] = dbtype;
        }
    }
}
