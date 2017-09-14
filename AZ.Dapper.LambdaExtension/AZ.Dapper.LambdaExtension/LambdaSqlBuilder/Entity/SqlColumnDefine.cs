using System;
using Dapper.LambdaExtension.LambdaSqlBuilder.Attributes;

namespace Dapper.LambdaExtension.LambdaSqlBuilder.Entity
{
    
    public class SqlColumnDefine
    {
        [Obsolete]
        public object Value { get; set; }

        public Type ValueType { get; set; }

        public string AliasName { get; set; }

        public string Name { get; set; }

        public bool NullAble { get; set; }

        public ZPColumnAttribute ColumnAttribute { get; set; }

        public ZPKeyAttribute KeyAttribute { get; set; }
        [Obsolete]
        public ZPCustomeDataTypeAttribute DataTypeAttribute { get; set; }


        public ZPIgnoreAttribute IgnoreAttribute { get; set; }

        public SqlColumnDefine(string name, string aliasName, object value,Type valueType, bool nullAble, ZPColumnAttribute columnAttr, ZPKeyAttribute keyAttr, ZPCustomeDataTypeAttribute customeDataTypeAttr, ZPIgnoreAttribute ignoreAttr=null)
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

        public SqlColumnDefine( ZPColumnAttribute columnAttribute, ZPKeyAttribute keyAttribute = null )
        {
            Name = columnAttribute.Name;
            AliasName = columnAttribute.Name;
            ColumnAttribute = columnAttribute;
            KeyAttribute = keyAttribute;
        }
    }
}
