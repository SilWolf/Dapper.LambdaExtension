using System;
using Dapper.LambdaExtension.LambdaSqlBuilder.Attributes;
using OfficeOpenXml.Announce;
using System.Data;

namespace AIRBox.Data.Entity
{
    /// <summary>
    /// 参数定义
    /// </summary>
    /// <code>
    /// </code>
    [Serializable]
    [ZPTable("o_parameter_child")]
    public class ParameterChild:EntityBase
    {
        /// <summary>
        /// 主参数ID
        /// </summary>
        [EPColumnName("Parameter Id",1)]
        [ZPColumn("parameter_id", dbType: DbType.AnsiStringFixedLength, fieldLength: "51")]
        public string ParameterId { get; set; }
        /// <summary>
        /// 参数的频率索引 
        /// </summary>
        [EPColumnName("Occurred Index", 2)]
        [ZPColumn("occurred_index")]
        public int OccurredIndex { get; set; }
        /// <summary>
        /// 参数部分的顺序索引
        /// </summary>
        [EPColumnName("Part Index", 3)]
        [ZPColumn("part_index")]
        public int PartIndex { get; set; }
        /// <summary>
        /// 频率所在的超级帧索引 ,存储的是绝对值,即 0或 1-16. //0代表每个超级帧循环上都有值
        /// </summary>
        [EPColumnName("Super Frame Index", 4)]
        [ZPColumn("super_frame")]
        public int SuperFrameIndex { get; set; }
        /// <summary>
        /// 子帧索引,存储绝对值, 1-4,代表此部分在该子帧上
        /// </summary>
        [EPColumnName("Sub Frame Index", 5)]
        [ZPColumn("sub_frame")]
        public int SubFrameIndex { get; set; }
        /// <summary>
        /// 参数部分所在字
        /// </summary>
        [EPColumnName("Word Index", 6)]
        [ZPColumn("word_index")]
        public int WordIndex { get; set; }
        /// <summary>
        /// 参数在字中的定位值
        /// </summary>
        [EPColumnName("Position", 7)]
        [ZPColumn("p_position")]
        public int Position { get; set; }
        /// <summary>
        /// 参数部分是否有符号
        /// </summary>
        [EPColumnName("Sign", 8)]
        [ZPColumn("sign")]
        public bool Sign { get; set; }
        /// <summary>
        /// 参数部分缩放值
        /// </summary>
        [EPColumnName("Resolution", 9)]
        [ZPColumn("resolution")]
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
