using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AZ.Dapper.LambdaExtension;
using ConsoleApp1;
 
 
using Dapper;
using Dapper.Contrib.Extensions;
using testdemo.Entities;

namespace testdemo
{
    public class testmysqllogic:MysqlBase
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
