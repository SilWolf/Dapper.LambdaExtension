using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AZ.Dapper.LambdaExtension.Attributes;
 

namespace AZ.Dapper.LambdaExtension.LambdaSqlBuilder.Entity
{
    [Serializable]
    public class SqlColumnDefine
    {
        public object Value { get; set; }

        public Type ValueType { get; set; }

        public string AliasName { get; set; }

        public string Name { get; set; }

        public bool NullAble { get; set; }

        public LamColumnAttribute ColumnAttribute { get; set; }

        public LamKeyAttribute KeyAttribute { get; set; }

        public LamCustomeDataTypeAttribute DataTypeAttribute { get; set; }


        public LamIgnoreAttribute IgnoreAttribute { get; set; }

        public SqlColumnDefine(string name, string aliasName, object value,Type valueType, bool nullAble,LamColumnAttribute columnAttr, LamKeyAttribute keyAttr, LamCustomeDataTypeAttribute customeDataTypeAttr, LamIgnoreAttribute ignoreAttr=null)
        {
            Name = name;
            AliasName = aliasName;
            Value = value;
            ColumnAttribute = columnAttr;
            KeyAttribute = keyAttr;
            DataTypeAttribute = customeDataTypeAttr;
            IgnoreAttribute = ignoreAttr;
            ValueType = valueType;
            NullAble = nullAble;
        }
    }
}
