using System;
using System.Collections.Generic;
using System.Text;
using AIRBox.Data.Entity;
using Dapper.LambdaExtension.LambdaSqlBuilder.Attributes;

namespace AIRBox.Data.VirtualEntity
{
    [DBTable("o_parameter_child")]
    public class VEParameterChild:ParameterChild
    {
        [DBColumn("parameter_id")]
        public string ParameterId { get; set; }

        [DBColumn("f_frequency")]
        public int Frequency { get; set; }
    }
}
