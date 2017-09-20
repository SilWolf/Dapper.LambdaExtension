using System;
using Dapper.LambdaExtension.LambdaSqlBuilder.Attributes;
using System.Data;

namespace AIRBox.Data.Entity
{
    /// <summary>
    /// 
    /// </summary>
    /// <code>
	///	CREATE TABLE r_matrix_parameter
	///	(
	///		id VARCHAR(51) PRIMARY KEY NOT NULL,
	///		parameter_id VARCHAR(51) NOT NULL,
	///		matrix_id VARCHAR(51) NOT NULL,
	///		created_time TIMESTAMP NOT NULL,
	///		name VARCHAR(100)
	///	);
    /// </code>
    [Serializable]
    [DBTable("r_matrix_parameter")]
    public class RelationMatrixParameter:EntityBase
    {
        [DBColumn("matrix_id", dbType: DbType.AnsiStringFixedLength, fieldLength: "51")]
        public string MatrixId { get; set; }
        [DBColumn("parameter_id", dbType: DbType.AnsiStringFixedLength, fieldLength: "51")]
        public string ParameterId { get; set; }
        [DBColumn("r_name")]
        public string Name { get; set; }

  

        [DBColumn("r_index")]
        public int Index { get; set; }
        /// <summary>
        /// 该参数存储的Engine Table名称
        /// </summary>
        [DBColumn("engine_table_name")]
        public string EngineTableName { get; set; }

        public RelationMatrixParameter()
        {
        }

        public RelationMatrixParameter(string id) : base(id)
        {
        }
    }
}
