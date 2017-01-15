using System.Collections.Generic;
using System.Linq;
using ConsoleApp1;
using Dapper.LambdaExtension.Extentions;
using testdemo.Entities;

namespace testdemo.TestLogic
{
    public class mysqltestlogic:MysqlBase
    {
 
        public List<Tbn> Find()
        {
            using (var db = GetConnection())
            {
                return db.Query<Tbn>(p => p.id >= 1).ToList();
            }
        }

        public PagedResult<Tbn> FindPage(int pageSize, int pageNumber)
        {
            using (var db = GetConnection())
            {
                return db.PagedQuery<Tbn>(pageSize,pageNumber,p => p.id >= 1);
            }
        }
    }
}
