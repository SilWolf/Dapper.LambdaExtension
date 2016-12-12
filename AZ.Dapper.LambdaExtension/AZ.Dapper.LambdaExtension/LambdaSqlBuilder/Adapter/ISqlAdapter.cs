using System.Collections.Generic;
using AZ.Dapper.LambdaExtension.Entity;
using AZ.Dapper.LambdaExtension.LambdaSqlBuilder.Entity;

namespace AZ.Dapper.LambdaExtension.Adapter
{
 
    public interface ISqlAdapter
    {
        //string LeftToken { get; }
        //string RightToken { get; }
        //string ParamPrefix { get; }
        bool SupportParameter { get; }

        string Query(SqlEntity entity);
        string QueryPage(SqlEntity entity);
        string Insert(bool key, SqlEntity entity);
        string Update(SqlEntity entity);
        string Delete(SqlEntity entity);

        string Table(string tableName);
        string Field(string filedName);
        string Field(string tableName, string fieldName);
        string Parameter(string parameterId);

        string LikeStagement();

        string LikeChars();
        string CreateTable(SqlTableDefine tableDefine, List<SqlColumnDefine> columnDefines);
    }
}
