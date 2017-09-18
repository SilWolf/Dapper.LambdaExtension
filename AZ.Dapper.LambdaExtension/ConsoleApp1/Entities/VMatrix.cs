using System;
using System.Threading;
using Dapper.LambdaExtension.LambdaSqlBuilder.Attributes;
using OfficeOpenXml.Announce;

namespace AIRBox.Data.Entity
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    [ZPTable("v_matrix")]
    public class VMatrix:Matrix
    {
        /// <summary>
        /// 
        /// </summary>
        [EPColumnName("Parameter Count", 12)]
        [ZPColumn("parameter_count")]
        public int ParameterCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [EPColumnName("Max Frequency", 13)]
        [ZPColumn("max_frequency")]
        public int MaxFrequency { get; set; }

        [EPColumnName("Relation Aircraft Count", 14)]
        [ZPColumn("relation_aircraft_count")]
        public int? RelationAircraftCount { get; set; }

        [EPColumnName("Decode Times", 15)]
        [ZPColumn("decode_times")]
        public int? DecodeTimes { get; set; }
        [EPColumnName("Last Usage Date", 16)]
        [ZPColumn("last_usage_date")]
        public DateTime? LastUsageDate { get; set; }
        
        [EPColumnName("Sync Words",4)]
        [ZPIgnore]
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
