using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Dapper.LambdaExtension.LambdaSqlBuilder.Attributes;
 
using System.Data;

namespace AIRBox.Data.Entity
{
    [Serializable]
    public class EntityBase
    {
 
        [DBColumn("id", dbType:DbType.AnsiStringFixedLength,fieldLength:"51")]
        [DBKey]
        public string Id { get; set; }
        
 
        [DBColumn("is_deleted")]
        public bool IsDeleted { get; set; }
 
        [DBColumn("created_time")]
        public DateTime CreatedTime { get; set; }

 
        [DBColumn("last_updated_time")]
        public DateTime LastUpdatedTime { get; set; }

        [DBIgnore]
        public CustomProperties CustomProperties { get; set; } = new CustomProperties();

        public EntityBase()
        {
            Id = Helper.CreateId();
            CreatedTime = DateTime.UtcNow;
            IsDeleted = false;
            LastUpdatedTime = DateTime.UtcNow;
        }

        public EntityBase(string id)
        {
            if (id.Length != 51)
            {
                id = Helper.CreateId();
            }
            Id = id;
            CreatedTime = DateTime.UtcNow;
            IsDeleted = false;
            LastUpdatedTime = DateTime.UtcNow;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    [Serializable]
    public class CustomProperties : Dictionary<string, dynamic>
    {
        public CustomProperties() { }

        public CustomProperties(IDictionary<string, dynamic> dict ):base(dict) { }

        /// <summary>
        /// binary序列化反序列化时,必须有此构造方法才能正常反序列化
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public CustomProperties(SerializationInfo info,StreamingContext context) : base(info, context) { }

        public void AddOrUpdate(string key, dynamic obj)
        {
            if (ContainsKey(key))
            {
                this[key] = obj;
            }
            else
            {
                this.Add(key,obj);
            }
        }

        public dynamic Get(string key)
        {
            return ContainsKey(key) ? this[key] : null;
        }

        public CustomProperties DeepClone()
        {
            var newObj=new CustomProperties(this);
            foreach (var item in from item in this let vtype = item.Value.GetType() where !vtype.IsValueType select item)
            {
                newObj[item.Key] = Helper.DeepCopy<dynamic>(item.Value);
            }
            return newObj;
        }
    }
}
