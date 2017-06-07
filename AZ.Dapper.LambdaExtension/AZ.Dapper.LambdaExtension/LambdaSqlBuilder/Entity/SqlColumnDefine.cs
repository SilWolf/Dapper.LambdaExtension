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

        public DBColumnAttribute ColumnAttribute { get; set; }

        public DBKeyAttribute KeyAttribute { get; set; }

        [Obsolete]
        public DBCustomeDataTypeAttribute DataTypeAttribute { get; set; }


        public DBIgnoreAttribute IgnoreAttribute { get; set; }

        public SqlColumnDefine(string name, string aliasName, object value,Type valueType, bool nullAble,DBColumnAttribute columnAttr, DBKeyAttribute keyAttr, DBCustomeDataTypeAttribute customeDataTypeAttr, DBIgnoreAttribute ignoreAttr=null)
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

        public SqlColumnDefine(DBColumnAttribute columnAttribute, DBKeyAttribute keyAttribute = null)
        {
            Name = columnAttribute.Name;
            AliasName = columnAttribute.Name;
            ColumnAttribute = columnAttribute;
            KeyAttribute = keyAttribute;
        }
    }
}
