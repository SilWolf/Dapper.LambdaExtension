using Dapper.LambdaExtension.LambdaSqlBuilder.Adapter;

namespace Dapper.LambdaExtension.Bulk
{
    public abstract class SqlBulkBase:ISqlBulk
    {
        
        public SqlAdapterType GetAdapterType()
        {
            return SqlAdapterType.Default;
        }

        public void Register()
        {
            AdapterFactory.
        }
    }
}
