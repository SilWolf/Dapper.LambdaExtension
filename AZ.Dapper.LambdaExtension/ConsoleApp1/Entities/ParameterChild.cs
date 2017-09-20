using System;
using Dapper.LambdaExtension.LambdaSqlBuilder.Attributes;
 
using System.Data;

namespace AIRBox.Data.Entity
{
    /// <summary>
    /// 参数定义
    /// </summary>
    /// <code>
    /// </code>
    [Serializable]
    [DBTable("o_parameter_child")]
    public class ParameterChild:EntityBase
    {
        /// <summary>
        /// 主参数ID
        /// </summary>
 
        [DBColumn("parameter_id", dbType: DbType.AnsiStringFixedLength, fieldLength: "51")]
        public string ParameterId { get; set; }
        /// <summary>
        /// 参数的频率索引 
        /// </summary>
 
        [DBColumn("occurred_index")]
        public int OccurredIndex { get; set; }
        /// <summary>
        /// 参数部分的顺序索引
        /// </summary>
 
        [DBColumn("part_index")]
        public int PartIndex { get; set; }
        /// <summary>
        /// 频率所在的超级帧索引 ,存储的是绝对值,即 0或 1-16. //0代表每个超级帧循环上都有值
        /// </summary>
 
        [DBColumn("super_frame")]
        public int SuperFrameIndex { get; set; }
        /// <summary>
        /// 子帧索引,存储绝对值, 1-4,代表此部分在该子帧上
        /// </summary>
 
        [DBColumn("sub_frame")]
        public int SubFrameIndex { get; set; }
        /// <summary>
        /// 参数部分所在字
        /// </summary>
 
        [DBColumn("word_index")]
        public int WordIndex { get; set; }
        /// <summary>
        /// 参数在字中的定位值
        /// </summary>
 
        [DBColumn("p_position")]
        public int Position { get; set; }
        /// <summary>
        /// 参数部分是否有符号
        /// </summary>
 
        [DBColumn("sign")]
        public bool Sign { get; set; }
        /// <summary>
        /// 参数部分缩放值
        /// </summary>
 
        [DBColumn("resolution")]
        public double Resolution { get; set; }

        public ParameterChild()
        {
        }

        public ParameterChild(string id) : base(id)
        {
        }

        public override bool Equals(object obj)
        {
            var pc = obj as ParameterChild;
            return Equals(pc);
        }

        public bool Equals(ParameterChild other)
        {
            if (other == null)
            {
                return false;
            }

            return string.Equals(Id, other.Id)
                   && string.Equals(ParameterId, other.ParameterId)
                   && OccurredIndex == other.OccurredIndex
                   && PartIndex == other.PartIndex
                   && SuperFrameIndex == other.SuperFrameIndex
                   && SubFrameIndex == other.SubFrameIndex
                   && WordIndex == other.WordIndex
                   && Position == other.Position
                   && Sign == other.Sign
                   && Resolution == other.Resolution;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
