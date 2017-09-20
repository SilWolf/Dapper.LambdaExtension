using System;
using Dapper.LambdaExtension.LambdaSqlBuilder.Attributes;
 
using System.Data;

namespace AIRBox.Data.Entity
{
 
    [Serializable]
    [DBTable("o_decode_process")]
    public class DecodeProcess:EntityBase
    {
 
 
        [DBColumn("aircraft_id", dbType: DbType.AnsiStringFixedLength, fieldLength: "51")]
        public string AircraftId { get; set; }
 
        [DBColumn("data_path")]
        public string DataPath { get; set; }
 
        [DBColumn("data_md5")]
        public string DataMd5 { get; set; }
 
        [DBColumn("matrix_id", dbType: DbType.AnsiStringFixedLength, fieldLength: "51")]
        public string MatrixId { get; set; }

 
        [DBColumn("matrix_code",dbType:DbType.AnsiStringFixedLength,fieldLength:"51")]
        public string MatrixCode { get; set; }
 

        DateTime _startTime;
 
        [DBColumn("start_time")]
        public DateTime StartTime
        {
            get => _startTime.GetDateTime();
            set => _startTime = value.GetDateTime();
        }
        DateTime _completeTime;
 
        [DBColumn("complete_time")]
        public DateTime CompleteTime
        {
            get => _completeTime.GetDateTime();
            set => _completeTime = value.GetDateTime();
        }
 
        [DBColumn("status")]
        public string Status { get; set; }


        public DecodeProcess()
        {
        }

        public DecodeProcess(string id) : base(id)
        {
        }
    }
}
