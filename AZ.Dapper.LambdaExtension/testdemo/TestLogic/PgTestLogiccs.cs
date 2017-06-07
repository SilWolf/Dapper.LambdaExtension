using System.Collections.Generic;
using System.Linq;
using Dapper.LambdaExtension.Extentions;
using testdemo.Entities;

namespace testdemo.TestLogic
{
    public class PgTestLogic:NpgsqlBase
    {

        public List<Test2> FindAction()
        {
            using (var db = GetConnection())
            {
                return db.Query<Test2>(sql =>
                {
                    sql.WhereIsIn(p => p.Id, new List<object>() {1,4});
                }).ToList();
            }
        }

        public PagedResult<Test2> FindPage(int pageSize, int pageNumber)
        {
            using (var db = GetConnection())
            {
                return db.PagedQuery<Test2>(pageSize, pageNumber, p => p.Id >= 1);
            }
        }


        public PagedResult<Test2> FindPageAction(int pageSize, int pageNumber)
        {
            using (var db = GetConnection())
            {
                return db.PagedQuery<Test2>(pageSize, pageNumber, sql =>
                {
                    sql.Where(p => p.Id >= 1);
                } );
            }
        }

        public int Inset(Test2 item)
        {
            using (var db = GetConnection())
            {
                return db.Insert(item);
            }

        }


        public void InsertIfNot(int count,string prefix)
        {
            using (var db = GetConnection())
            {
                var ecount = db.Query<Test2>(null).ToList().Count;

                if (count > ecount)
                {
                    var waitInsertcount = count - ecount;

                    for (var i = 0; i < waitInsertcount; i++)
                    {
                        var item=new Test2()
                        {
                            Name = prefix + i.ToString()
                        };

                        db.Insert(item);
                    }
                }

            }
        }



        public void CreateTable()
        {
            using (var db = GetConnection())
            {
                db.CreateTable<Test2>();
            }
        }
    }
}
