using System;
using AZ.Dapper.LambdaExtension.Entity;

namespace AZ.Dapper.LambdaExtension.Adapter
{
    [Serializable]
    class SqlAnyWhereAdapter : AdapterBase
    {
        public SqlAnyWhereAdapter()
            : base(SqlConst.LeftTokens[2], SqlConst.RightTokens[2], SqlConst.ParamPrefixs[0])
        {

        }

        public override string QueryPage(SqlEntity entity)
        {
            int pageSize = entity.PageSize;
            int limit = pageSize * (entity.PageNumber - 1);
           // select top 30 start at (N - 1) * 30 + 1 * from customer
            return string.Format("SELECT TOP {4} start at {5} {0} FROM {1} {2} {3} ", entity.Selection, entity.TableName, entity.Conditions, entity.OrderBy, pageSize,limit+1 );
        }

        
    }
}
