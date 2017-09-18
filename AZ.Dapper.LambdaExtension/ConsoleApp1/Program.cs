using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Dapper.LambdaExtension;
using Dapper.LambdaExtension.LambdaSqlBuilder.Entity;
using testdemo.Entities;
using testdemo.TestLogic;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {

            PreApplicationStart.RegisterTypeMaps();

            //TestINsert();


            //var pg = npglogic.FindPageAction(10, 2);

            //Console.WriteLine($"total:{pg.Count},pageSize:{pg.PageSize},currentPage:{pg.PageNumber}");

            //foreach (var p in pg.Results)
            //{
            //    Console.WriteLine(p.Id.ToString() + "__|__" + p.Name);
            //}

            //var t2ret = npglogic.TestSuperClass();

            //Console.WriteLine("name1 | name2");

            //foreach (var t2 in t2ret)
            //{
            //    Console.WriteLine(t2.Name+" | "+t2.TestName2);
            //}

            //npglogic.FindAction2();



            //var result = npglogic.TestJoinCount<dynamic>(sql =>
            //{

            //    sql.Select(p => p.Id, p => p.Name);
            //    sql.Count(p => p.Id, "Count");
            //    sql.GroupBy(p => p.Id, p => p.Name);

            //    //sql.Join<PgTestLogic.t2>(((test2, t2) =>test2.Name==t2.Name)).Where(v=>v.TestName2.Contains("d"));

            //    //sql.Join<PgTestLogic.t2,int,dynamic>(sql => { },p=>p.Id,v=>v.Name,((test2, t2) => test2.Id, ))

            //    //sql.SelectEntity(p => new SqlColumnEntity("c_name",null), p => new SqlColumnEntity("Id",null));
            //    //sql.Join<PgTestLogic.t2, string>(exp =>
            //    //{
            //    //    exp.Where(p => p.TestName2.Contains("ccc"));
            //    //}, p => p.Name, p => p.Name,JoinType.LeftJoin, v => v.Name, v => v.TestName2);

            //    var sqlsub=sql.InnerJoinSubQuery<PgTestLogic.t2, string>(sql2 =>
            //    {
            //        sql2.Max(p => p.Name);
            //        sql2.GroupBy(p => p.TestName2);
            //        sql2.Count<Test2>(p => p.TestName2,v=>v.Name);
            //    }, p=>p.Name,v=>v.Name,m=>m.TestName2);

            //    sql.GroupBySubQuery<PgTestLogic.t2>(sqlsub.JoinSubAliasTableName, p => p.TestName2);

            //});




            // test for sub query from lambda.

            //var result = npglogic.TestV<VTest>(sql =>
            //{
            //    //sql.Select();

            //    sql.SelectAll();

            //    var sqlsub = sql.LeftJoinSubQuery<PgTestLogic.t2, string>(sql2 =>
            //    {
            //        sql2.Select(p => p.Name);
            //        sql2.Select<VTest>(t => t.TestName2, v => v.Name);
            //          sql2.Count<VTest>(p => p.TestName2, p => p.Count5);
            //        sql2.GroupBy(p => p.Name);

            //    }, v => v.Name, v => v.Name);

            //    sql.SelectSubQuery<VTest>(sqlsub, p => p.Count5);

            //});

            //the above code make below t-sql script:
            //SELECT

            //"Id",
            //"c_name",
            //join_636411782561333769."count_5"
            //FROM
            //    Test2
            //LEFT JOIN(
            //    SELECT
            //        COUNT ("test".v_tt2."name2") AS "count_5",
            //name1
            //    FROM

            //"test".v_tt2

            //group by name1
            //    ) join_636411782561333769 ON Test2."c_name" = join_636411782561333769."name1"




            //var first = result[0];

            //var firstId = first.Id;

            //foreach (var item in result)
            //{
            //    Console.WriteLine($"{item.Id}_{item.c_name}_{item.Count}");
            //}

            var msLogic=new MssqlTestLogic();

            msLogic.TestSubSubQuery();


            Console.ReadLine();
        }

        private static void TestINsert()
        {
            Console.WriteLine("===========for Npgsql==========");
            var npglogic = new PgTestLogic(); // new MssqlTestLogic();//TestLogic();

            //var pglist = npglogic.FindPage(2, 1);

            //var pglist2 = npglogic.FindPageAction(2, 1);
            //  var inlist = npglogic.FindAction();

            npglogic.CreateTable();

            //npglogic.CreateSuperTable();


            var list1 = npglogic.GetMultiList(2001, "aa" + DateTime.Now.ToString());

            var st = Stopwatch.StartNew();

            npglogic.InsertMulitFor(list1);

            st.Stop();


            Console.WriteLine("use foreach: " + st.ElapsedMilliseconds + " ms.");


            var list2 = npglogic.GetMultiList(2001, "bb" + DateTime.Now.ToString());
            var st2 = Stopwatch.StartNew();
            npglogic.InsertMulti(list2);

            st2.Stop();

            Console.WriteLine("use multi: " + st2.ElapsedMilliseconds + " ms.");


            Console.WriteLine("===========for MS Sql==========");

            var mslogic = new MssqlTestLogic(); // new MssqlTestLogic();//TestLogic();

            //var pglist = npglogic.FindPage(2, 1);

            //var pglist2 = npglogic.FindPageAction(2, 1);
            //  var inlist = npglogic.FindAction();

            //mslogic.CreateTable();

            //npglogic.CreateSuperTable();


            var list11 = mslogic.GetMultiList(2001, "aa" + DateTime.Now.ToString());

            var st12 = Stopwatch.StartNew();

            mslogic.InsertMulitFor(list11);

            st12.Stop();


            Console.WriteLine("use foreach: " + st12.ElapsedMilliseconds + " ms.");


            var list12 = mslogic.GetMultiList(2001, "bb" + DateTime.Now.ToString());
            var st22 = Stopwatch.StartNew();
            mslogic.InsertMulti(list12);

            st22.Stop();

            Console.WriteLine("use multi: " + st22.ElapsedMilliseconds + " ms.");
        }
    }
}