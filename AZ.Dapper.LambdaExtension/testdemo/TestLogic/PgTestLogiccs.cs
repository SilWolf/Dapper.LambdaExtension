using System.Collections.Generic;
using System.Linq;
using Dapper;
using Dapper.Contrib.Extensions;
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
                return DapperLambdaExt.Insert(db, item);
            }

        }


        public void InsertIfNot(int count,string prefix)
        {
            using (var db = GetConnection())
            {
                var ecount = DapperLambdaExt.Query<Test2>(db, null).ToList().Count;

                if (count > ecount)
                {
                    var waitInsertcount = count - ecount;

                    for (var i = 0; i < waitInsertcount; i++)
                    {
                        var item=new Test2()
                        {
                            Name = prefix + i.ToString()
                        };

                        DapperLambdaExt.Insert(db, item);
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

        //public void testNObuffer()
        //{
        //    using (var db = GetConnection())
        //    {
        //        //扩展方法,为了不缓存要执行的SQL语句,比如大量的拼接插入values类语句,如果要缓存的话,是会造成内存一直增长的问题
        //        db.ExecuteNoCache("", flag: CommandFlags.NoCache);
        //    }
        //}
    }
}
