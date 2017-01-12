using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Dapper.LambdaExtension.Extentions;
using testdemo.Entities;

namespace testdemo
{
    public class PgTestLogic:NpgsqlBase
    {


        public void CreateTest3()
        {
            using (var db = GetConnection())
            {
                try
                {
                    db.CreateTable<Test3>();

                    Console.WriteLine("table test3 created.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message+ex.StackTrace);
                }
            }
        }


        public void InsertTest3(Test3 item)
        {
            using (var db = GetConnection())
            {
                try
                {
                    db.Insert(item);

                    

                    Console.WriteLine("  test3 inserted.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message + ex.StackTrace);
                }
            }
        }

        public void UpdateTest3(Test3 item)
        {
            using (var db = GetConnection())
            {
                try
                {
                    db.Update(item);



                    Console.WriteLine("  test3 updated.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message + ex.StackTrace);
                }
            }
        }

        public void DeleteTest3(Test3 item)
        {
            using (var db = GetConnection())
            {
                try
                {
                    db.Delete(item);



                    Console.WriteLine("  test3 updated.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message + ex.StackTrace);
                }
            }
        }

        public Test3 GeTest3()
        {
            using (var db = GetConnection())
            {
                try
                {
                    var test3=db.QueryFirstOrDefault<Test3>();


                    Console.WriteLine("  test3 get.");
                    return test3;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message + ex.StackTrace);
                }
            }
            return null;
        }


        public List<Test2> FindAction()
        {
            using (var db = GetConnection())
            {
                return db.QueryWithAction<Test2>(sql =>
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
                return db.PagedQueryWithAction<Test2>(pageSize, pageNumber, sql =>
                {
                    sql.Where(p => p.Id >= 1);
                } );
            }
        }
    }
}
