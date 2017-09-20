using System;
using System.Threading;
using Dapper.LambdaExtension.LambdaSqlBuilder.Attributes;
 

namespace AIRBox.Data.Entity
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    [DBTable("v_matrix")]
    public class VMatrix:Matrix
    {
        /// <summary>
        /// 
        /// </summary>
 
        [DBColumn("parameter_count")]
        public int ParameterCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
 
        [DBColumn("max_frequency")]
        public int MaxFrequency { get; set; }

 
        [DBColumn("relation_aircraft_count")]
        public int? RelationAircraftCount { get; set; }

 
        [DBColumn("decode_times")]
        public int? DecodeTimes { get; set; }
 
        [DBColumn("last_usage_date")]
        public DateTime? LastUsageDate { get; set; }
        
 
        [DBIgnore]
        public string SyncWords => $"{SyncWord1}[{SyncWord1:X}] - {SyncWord2}[{SyncWord2:X}] - {SyncWord3}[{SyncWord3:X}] - {SyncWord4}[{SyncWord4:X}]";

        public VMatrix()
        {
        }

        public VMatrix(string id):base(id)
        {

        }

        public override bool Equals(object obj)
        {
            return obj is VMatrix vm && GetHashCode() == vm.GetHashCode();
        }

        public override int GetHashCode()
        {
            var hashString = Id;
            if (string.IsNullOrEmpty(hashString))
            {
                hashString = Name + WordLength + Code + ParameterCount;
            }
            return hashString.GetHashCode();
        }


    }
}
