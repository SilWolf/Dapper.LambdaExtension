using System;
using testdemo.TestLogic;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
           
            var npglogic = new PgTestLogic();

            //var pglist = npglogic.FindPage(2, 1);

            //var pglist2 = npglogic.FindPageAction(2, 1);
            //  var inlist = npglogic.FindAction();

            npglogic.CreateTable();

            npglogic.CreateSuperTable();



            npglogic.InsertIfNot(36, "aa");


            var pg = npglogic.FindPageAction(10, 2);

            Console.WriteLine($"total:{pg.Count},pageSize:{pg.PageSize},currentPage:{pg.PageNumber}");

            foreach (var p in pg.Results)
            {
                Console.WriteLine(p.Id.ToString() + "__|__" + p.Name);
            }

            //var t2ret = npglogic.TestSuperClass();

            //Console.WriteLine("name1 | name2");

            //foreach (var t2 in t2ret)
            //{
            //    Console.WriteLine(t2.Name+" | "+t2.TestName2);
            //}

            //npglogic.FindAction2();

            Console.ReadLine();
        }
    }
}