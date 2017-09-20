using System;
using Dapper.LambdaExtension.LambdaSqlBuilder.Attributes;
 

namespace AIRBox.Data.Entity
{
     
    [Serializable]
    [DBTable("o_matrix")]
    public class Matrix:EntityBase
    {
 
        [DBColumn("name")]
        public string Name { get; set; }
 
        [DBColumn("code")]
        public string Code { get; set; }
 
        [DBColumn("word_length")]
        public int WordLength { get; set; }
 
        [DBColumn("sync_word_1")]
        public int SyncWord1 { get; set; }
 
        [DBColumn("sync_word_2")]
        public int SyncWord2 { get; set; }
 
        [DBColumn("sync_word_3")]
        public int SyncWord3 { get; set; }
 
        [DBColumn("sync_word_4")]
        public int SyncWord4 { get; set; }
 
        [DBColumn("frame_counter_start_with")]
        public int FrameCounterStartWith { get; set; }
 
        [DBColumn("is_activated")]
        public bool Activated { get; set; }
 
        [DBColumn("original_data", FieldLength = "4000")]
        public string OriginalData { get; set; }
 
        [DBColumn("description", FieldLength = "4000")]
        public string Description { get; set; }

        public Matrix()
        {
        }

        public Matrix(string id):base(id)
        {

        }
    }
}
