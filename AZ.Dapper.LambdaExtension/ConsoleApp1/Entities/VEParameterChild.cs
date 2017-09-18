using System;
using System.Collections.Generic;
using System.Text;
using AIRBox.Data.Entity;
using Dapper.LambdaExtension.LambdaSqlBuilder.Attributes;

namespace AIRBox.Data.VirtualEntity
{
    [ZPTable("o_parameter_child")]
    public class VEParameterChild:ParameterChild
    {
        [ZPColumn("parameter_id")]
        public string ParameterId { get; set; }

        [ZPColumn("f_frequency")]
        public int Frequency { get; set; }
    }
}
