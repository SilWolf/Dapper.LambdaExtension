using System;
using Dapper.LambdaExtension.LambdaSqlBuilder.Attributes;
using OfficeOpenXml.Announce;
using System.Data;

namespace AIRBox.Data.Entity
{
    /// <summary>
    /// 译码进程
    /// </summary>
    /// <code>
	///	CREATE TABLE o_decode_process
	///	(
	///		id VARCHAR(51) PRIMARY KEY NOT NULL,
	///		aircraft_id VARCHAR(51) NOT NULL,
	///		matrix_id VARCHAR(51) NOT NULL,
	///		data_path VARCHAR(1000),
	///		data_md5 VARCHAR(32),
	///		engine_data_tables VARCHAR(1000),
	///		start_time TIMESTAMP,
	///		complete_time TIMESTAMP,
	///		status VARCHAR(255),
	///		created_time TIMESTAMP
	///	);
    /// </code>
    [Serializable]
    [ZPTable("o_decode_process")]
    public class DecodeProcess:EntityBase
    {
        /// <summary>
        /// 飞机ID
        /// </summary>
        [EPColumnName("Aircraft Id",1)]
        [ZPColumn("aircraft_id", dbType: DbType.AnsiStringFixedLength, fieldLength: "51")]
        public string AircraftId { get; set; }
        /// <summary>
        /// 数据文件地址
        /// </summary>
        [EPColumnName("Data Path", 5)]
        [ZPColumn("data_path")]
        public string DataPath { get; set; }
        /// <summary>
        /// 数据文件的MD5值
        /// </summary>
        [EPColumnName("Data MD5", 6)]
        [ZPColumn("data_md5")]
        public string DataMd5 { get; set; }
        /// <summary>
        /// 译码矩阵ID
        /// </summary>
        [EPColumnName("Matrix Id", 7)]
        [ZPColumn("matrix_id", dbType: DbType.AnsiStringFixedLength, fieldLength: "51")]
        public string MatrixId { get; set; }

        [EPColumnName("Matrix Code",7)]
        [ZPColumn("matrix_code",dbType:DbType.AnsiStringFixedLength,fieldLength:"51")]
        public string MatrixCode { get; set; }

        ///// <summary>
        ///// 工程值数据表
        ///// </summary>
        //[Obsolete]
        //[EPColumnName("Engine Tables Name",8)]
        //[Alias("engine_data_tables")]
        //public string EngineDataTables { get; set; }

        DateTime _startTime;
        /// <summary>
        /// 开始时间
        /// </summary>
        [EPColumnName("Start Time", 9)]
        [ZPColumn("start_time")]
        public DateTime StartTime
        {
            get => _startTime.GetDateTime();
            set => _startTime = value.GetDateTime();
        }
        DateTime _completeTime;
        /// <summary>
        /// 结束时间
        /// </summary>
        [EPColumnName("Completed Time", 10)]
        [ZPColumn("complete_time")]
        public DateTime CompleteTime
        {
            get => _completeTime.GetDateTime();
            set => _completeTime = value.GetDateTime();
        }
        /// <summary>
        /// 状态
        /// </summary>
        [EPColumnName("Status", 11)]
        [ZPColumn("status")]
        public string Status { get; set; }


        public DecodeProcess()
        {
        }

        public DecodeProcess(string id) : base(id)
        {
        }
    }
}
