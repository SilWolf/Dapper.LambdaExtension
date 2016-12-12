using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 

namespace testdemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Test2();

          

            Console.ReadLine();
        }


        static void Test2()
        {
            int? aa;

            aa = 1;

            var t1 = aa.GetType();
        }

        static void Test1()
        {
            //var logic=new testmysqllogic();

            ////var allList = logic.Find();

            //var pagelist = logic.FindPage(2, 1);

            var npglogic = new PgTestLogic();

            //var pglist = npglogic.FindPage(2, 1);

            //var pglist2 = npglogic.FindPageAction(2, 1);
            var inlist = npglogic.FindAction();
        }
    }
}
