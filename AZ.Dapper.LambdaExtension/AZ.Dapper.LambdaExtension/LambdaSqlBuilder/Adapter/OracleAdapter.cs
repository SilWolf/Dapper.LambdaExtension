using System;
using AZ.Dapper.LambdaExtension.Entity;

namespace AZ.Dapper.LambdaExtension.Adapter
{
    [Serializable]
    class OracleAdapter : AdapterBase
    {
        public OracleAdapter()
            : base(SqlConst.LeftTokens[2], SqlConst.RightTokens[2], SqlConst.ParamPrefixs[1])
        {

        }

        public override string QueryPage(SqlEntity entity)
        {
            int pageSize = entity.PageSize;
            int begin = (entity.PageNumber - 1) * pageSize;
            int end = entity.PageNumber * pageSize;
            return string.Format(@"SELECT * FROM (
SELECT A.*, ROWNUM RN FROM (SELECT {0} FROM {1} {2} {3}) A WHERE ROWNUM <= {5})WHERE RN >{4}", entity.Selection, entity.TableName, entity.Conditions, entity.OrderBy, begin, end);
        }
    }
}
