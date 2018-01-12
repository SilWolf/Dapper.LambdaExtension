using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using AIRBox.Data.Entity;
using AIRBox.Data.VirtualEntity;
using ConsoleApp1;
using Dapper.LambdaExtension;
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
                    sql.WhereIsIn(p => p.Id, new List<object>() {1,4});
                }).ToList();
            }
        }

        public List<Test2> FindActionByTrans()
        {
            using (var db = GetConnection())
            {
                var trans = db.BeginTransaction();
                return db.Query<Test2>(sql =>
                {
                    sql.WhereIsIn(p => p.Id, new List<object>() { 1, 4 });
                },trans).ToList();
            }
        }


        public void TestTransInsert()
        {
            using (var db = GetConnection())
            {
                var trans = db.BeginTransaction();

                try
                {
                    db.Insert(new Test2() {Name = "TransTEst"},trans);
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw ex;
                }
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

        public void InsertMulitFor(List<Test2> itemList)
        {
            using (var db = GetConnection())
            {
                ////var ecount = db.Query<Test2>().ToList().Count;



                for (var i = 0; i < itemList.Count; i++)
                {
                    var item = new Test2()
                    {
                        Name = itemList[i] + i.ToString()
                    };

                    db.Insert(item);
                }


            }
        }
        public void InsertMulti(List<Test2> itemList)
        {


            using (var db = GetConnection())
            {
                db.InsertList(itemList);
            }
        }

        public    List<Test2> GetMultiList(int count, string prefix)
        {
            var multiList = new List<Test2>();

            for (var i = 1; i <= count; i++)
            {
                multiList.Add(new Test2()
                {
                    Name = prefix + i.ToString()
                });
            }
            return multiList;
        }
        //public dynamic TestJoinCount(Action<SqlExp<Test2>> sql)
        //{
        //    using (var db = GetConnection())
        //    {
        //        return db.Query<Test2,dynamic>(sql);
        //    }
        //}

        public dynamic TestJoinCount<TResult>(Action<SqlExp<Test2>> sql)
        {
            using (var db = GetConnection())
            {
                return db.Query<Test2, TResult>(sql);
            }
        }


        public dynamic TestV<TResult>(Action<SqlExp<Test2>> sql)
        {
            using (var db = GetConnection())
            {
                return db.Query<Test2, TResult>(sql);
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

        public void TestSubSubQuery()
        {

            using (var db = GetConnection())
            {
                var list = db.PagedQuery<Matrix, VMatrix>(10, 3, sql =>
                    {
                        sql.Select(v => v.DecodeTimes);
                        sql.Where(p => p.ParameterCount > 1);
                    }, exp => GetVMatrix(exp)
                );

                //db.Update<Matrix>(sql =>
                //{
                //    sql.Update(v => v.Code, "a").Update(p=>p.SyncWord1,"aa"); 
                //});


            }
        }
        private void GetVMatrix(SqlExp<Matrix> sqlMain)
        {


            //sqlMain =>
            //{
            //main select
            sqlMain.Select(p => p.Id);
            sqlMain.Select(p => p.Name);
            sqlMain.Select(p => p.Code);
            sqlMain.Select(p => p.WordLength);
            sqlMain.Select(p => p.SyncWord1);
            sqlMain.Select(p => p.SyncWord2);
            sqlMain.Select(p => p.SyncWord3);
            sqlMain.Select(p => p.SyncWord4);
            sqlMain.Select(p => p.FrameCounterStartWith);
            sqlMain.Select(p => p.Activated);
            sqlMain.Select(p => p.IsDeleted);
            sqlMain.Select(p => p.OriginalData);
            sqlMain.Select(p => p.Description);
            sqlMain.Select(p => p.CreatedTime);
            sqlMain.Select(p => p.LastUpdatedTime);


            //sub query parameter indo
            var sqlInfo =
                sqlMain.JoinSubQuery<VEParametersInfo, string>(sql =>
                {

                        //sub query parameter child
                        var sqlchild = sql.JoinSubQuery<VEParameterChild, string>(sql2 =>
                    {
                        sql2.Select(p => p.ParameterId);
                        sql2.Count<VEParameterChild>(p => p.Id, v => v.Frequency);
                        sql2.Where(v => v.PartIndex == 1);
                        sql2.GroupBy(p => p.ParameterId);

                    }, p => p.ParameterId,
                        v => v.ParameterId, JoinType.InnerJoin);
                        //selection and other

                        sql.Select(p => p.MatrixId);
                    sql.Count<VEParametersInfo>(p => p.Id, v => v.ParameterCount);
                    sql.MaxSubQuery<VEParameterChild>(sqlchild, p => p.Frequency, v => v.MaxFrequency);

                    sql.GroupBy(p => p.MatrixId);

                }, p => p.Id, v => v.MatrixId, JoinType.LeftOuterJoin);

            sqlMain.SelectSubQuery<VMatrix>(sqlInfo, p => p.ParameterCount, p => p.MaxFrequency);
        

            //sub query  r aircraft _matrix

            var sqlAir = sqlMain.JoinSubQuery<RelationAircraftMatrix, string>(sql =>
            {
                sql.Select(p => p.MatrixCode);
                sql.Count<VMatrix>(v => v.Id, p => p.RelationAircraftCount);
                sql.Where(p => p.IsDeleted == false);
                sql.GroupBy(p => p.MatrixCode);

            }, p => p.Code, v => v.MatrixCode, JoinType.LeftOuterJoin);

            sqlMain.SelectSubQuery<VMatrix>(sqlAir, v => v.RelationAircraftCount);

            //subquery decode process

            var sqlDecode = sqlMain.JoinSubQuery<DecodeProcess, string>(sql =>
            {
                sql.Select(p => p.MatrixCode);
                sql.Count<VMatrix>(p => p.Id, v => v.DecodeTimes);
                sql.Max<VMatrix>(p => p.CreatedTime, v => v.LastUsageDate);
                sql.GroupBy(p => p.MatrixCode);

            }, p => p.Code,
                v => v.MatrixCode, JoinType.LeftOuterJoin);

            sqlMain.SelectSubQuery<VMatrix>(sqlDecode, v => v.DecodeTimes, v => v.LastUsageDate);

            //};
        }
    }
}
