using Dapper.LambdaExtension.LambdaSqlBuilder.Attributes;
using System.Data;

namespace AIRBox.Data.Entity
{
    /// <summary>
    /// 
    /// </summary>
    /// <code>
	///	CREATE TABLE r_aircraft_matrix
	///	(
	///		id VARCHAR(51) PRIMARY KEY NOT NULL,
	///		aircraft_id VARCHAR(51) NOT NULL,
	///		matrix_code VARCHAR NOT NULL,
	///		created_time TIMESTAMP NOT NULL
	///	);
    /// </code>
    [DBTable("r_aircraft_matrix")]
    public class RelationAircraftMatrix:EntityBase
    {
        [DBColumn("aircraft_id", dbType: DbType.AnsiStringFixedLength, fieldLength: "51")]
        public string AircraftId { get; set; }


        [DBColumn("matrix_code", dbType: DbType.AnsiStringFixedLength, fieldLength: "51")]
        public string MatrixCode { get; set; }
    }
}
