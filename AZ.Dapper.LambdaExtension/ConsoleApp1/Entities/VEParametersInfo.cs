using System;
using System.Collections.Generic;
using System.Text;
using AIRBox.Data.Entity;
using Dapper.LambdaExtension.LambdaSqlBuilder.Attributes;

namespace AIRBox.Data.VirtualEntity
{
    [ZPTable("r_matrix_parameter")]
    public class VEParametersInfo : RelationMatrixParameter
    {
        [ZPColumn("parameter_count")]
        public int ParameterCount { get; set; }

        [ZPColumn("max_frequency")]
        public int MaxFrequency { get; set; }
    }
}
