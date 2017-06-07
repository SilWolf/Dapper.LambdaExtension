using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Dapper.LambdaExtension.Extentions;
using Dapper.LambdaExtension.Helpers;
using Dapper.LambdaExtension.LambdaSqlBuilder;
using Dapper.LambdaExtension.LambdaSqlBuilder.Attributes;
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
                    sql.WhereIsIn(p => p.Id, new List<object>() { 1, 4 });
                }).ToList();
            }
        }


        public List<Test2> FindAction2()
        {
            using (var db = GetConnection())
            {
                return db.Query<Test2>(sql =>
                {
                    //sql.WhereIsIn(p => p.Id, new List<object>() { 1, 4 });

                    sql.Where(p => p.Id >= 1 && p.Id <= 20);


                    // var exp=new SqlExp<Test2>();

                    //exp.WhereIsIn(p => p.Name, new List<object>() {"aa1", "aa3", "aa0"});

                    sql.WhereIsIn(p => p.Name, new List<object>() { "aa1", "aa3", "aa0" });



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
                return db.PagedQuery<Test2>(pageSize, pageNumber, Action() );
            }
        }

        private static Action<SqlExp<Test2>> Action()
        {
            return sql =>
            {
                sql.Where(p => p.Id >= 1);
                sql.Where(p => p.Id < 30);
            };
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
                var ecount = db.Query<Test2>().ToList().Count;

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


        public List<t2> TestSuperClass()
        {
            using (var db = GetConnection())
            {
               
                return db.Query<t2>().ToList();
            }
        }


        public void CreateSuperTable()
        {
            using (var db = GetConnection())
            {
                db.CreateTable<t2>();
            }
        }


        public void CreateTable()
        {
            using (var db = GetConnection())
            {
                db.CreateTable<Test2>();
            }
        }


        [DBTable("tt1",Schema = "test")]
        public class t1
        {
            [DBColumn("name1")]
            public string Name { get; set; }
        }

        [DBTable("v_tt2",Schema = "test")]
        public class t2:t1
        {
            [DBColumn("name2")]
            public string TestName2 { get; set; }
        }

        public void TestType()
        {
            var type = typeof(Test2);

            var entityDef = type.GetEntityDefines();

        }
    }
}
