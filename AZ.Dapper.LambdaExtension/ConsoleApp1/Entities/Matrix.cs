using System;
using Dapper.LambdaExtension.LambdaSqlBuilder.Attributes;
using OfficeOpenXml.Announce;

namespace AIRBox.Data.Entity
{
    /// <summary>
    /// 译码参数矩阵
    /// </summary>
    /// <code>
	/// CREATE TABLE o_matrix
	/// (
	/// 	id VARCHAR(51) NOT NULL,
	/// 	name VARCHAR(255) NOT NULL,
	/// 	code VARCHAR NOT NULL,
	/// 	version INTEGER,
	/// 	word_length INTEGER NOT NULL,
	/// 	sync_word_1 INTEGER NOT NULL,
	/// 	sync_word_2 INTEGER,
	/// 	sync_word_3 INTEGER,
	/// 	sync_word_4 INTEGER,
	/// 	frame_counter_start_with INTEGER,
	/// 	activated BIT,
	/// 	deleted BIT,
	/// 	original_data TEXT,
	/// 	description VARCHAR(4000),
	/// 	created_time TIMESTAMP NOT NULL,
	/// 	CONSTRAINT o_matrix_pkey PRIMARY KEY (id, name)
	/// );
    /// </code>
    /// <remarks>
    /// Matrix具有版本属性，即一个Matrix可同时存在不同版本。
    /// 一个Matrix的不同版本ID不同，但具有相同的Code。
    /// Aircraft与Matrix进行关联时，需关联Matrix的Code，即每次都应该取最新版本的译码库进行译码；
    /// 译码时，译码行为与结果（包括工程值）需与Matrix的Id属性关联，即本次译码行为使用特定版本的译码库。
    /// </remarks>
    [Serializable]
    [ZPTable("o_matrix")]
    public class Matrix:EntityBase
    {
        /// <summary>
        /// 译码矩阵的易读名字
        /// 来自于机型的说明文件（DATA FRAME INTERFACE CONTROL AND REQUIREMENTS DOCUMENT,pdf)
        /// </summary>
        [EPColumnName("Name",1)]
        [ZPColumn("name")]
        public string Name { get; set; }
        /// <summary>
        /// 译码库唯一标识
        /// 不同版本的同一个译码库，其NoneVersionId相同。
        /// </summary>
        [EPColumnName("Code", 2)]
        [ZPColumn("code")]
        public string Code { get; set; }
        ///// <summary>
        ///// 版本 - 整形数字
        ///// </summary>
        //[EPColumnName("Version", 3)]
        //[Alias("version")]
        //public int Version { get; set; }
        /// <summary>
        /// 字长 - 标记每个子帧内有多少个字，取值范围为128,256,512,1024
        /// </summary>
        [EPColumnName("Word Length", 3)]
        [ZPColumn("word_length")]
        public int WordLength { get; set; }
        /// <summary>
        /// 第一同步字
        /// </summary>
        [EPColumnName("Sync Word #1", 5)]
        [ZPColumn("sync_word_1")]
        public int SyncWord1 { get; set; }
        /// <summary>
        /// 第二同步字
        /// </summary>
        [EPColumnName("Sync Word #2", 6)]
        [ZPColumn("sync_word_2")]
        public int SyncWord2 { get; set; }
        /// <summary>
        /// 第三同步字
        /// </summary>
        [EPColumnName("Sync Word #3", 7)]
        [ZPColumn("sync_word_3")]
        public int SyncWord3 { get; set; }
        /// <summary>
        /// 第四同步字
        /// </summary>
        [EPColumnName("Sync Word #4", 8)]
        [ZPColumn("sync_word_4")]
        public int SyncWord4 { get; set; }
        /// <summary>
        /// 帧计数器起始值,为0,则计数器范围为0~4095,为1,则计数器范围为1-4096
        /// </summary>
        [EPColumnName("Frame Counter Start With", 9)]
        [ZPColumn("frame_counter_start_with")]
        public int FrameCounterStartWith { get; set; }
        /// <summary>
        /// 是否正在创建
        /// </summary>
        [EPColumnName("Activated", 10)]
        [ZPColumn("is_activated")]
        public bool Activated { get; set; }
        ///// <summary>
        ///// 是否已删除
        ///// </summary>
        //[EPColumnName("Deleted", 11)]
        //[ZPColumn("deleted")]
        //public bool Deleted { get; set; }
        [EPColumnName("Original Data", 14)]
        [ZPColumn("original_data", FieldLength = "4000")]
        public string OriginalData { get; set; }
        [EPColumnName("Description", 15)]
        [ZPColumn("description", FieldLength = "4000")]
        public string Description { get; set; }

        public Matrix()
        {
        }

        public Matrix(string id):base(id)
        {

        }
    }
}
