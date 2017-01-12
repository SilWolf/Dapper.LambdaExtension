using System.Collections.Generic;

namespace Dapper.LambdaExtension.Extentions
{
    public class PagedResult<T> where T : class
    {
        public PagedResult(IEnumerable<T> resultList, int resultCount,int pageSize,int pageNumber)
        {
            Results = resultList;
            Count = resultCount;
            PageSize = pageSize;
            PageNumber = pageNumber;
        }
        public IEnumerable<T> Results { get; set; }

        public int Count { get; set; }

        public int PageSize { get; set; }

        public int PageNumber { get; set; }
    }
}
