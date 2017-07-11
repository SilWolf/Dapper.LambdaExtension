using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleApp1;
using Dapper.LambdaExtension.Extentions;
using testdemo.Entities;

namespace testdemo.TestLogic
{
    public class SqlCETestLogics:MsSqlCEBase
    {
        public List<Test1> GetAll()
        {
            using (var db = GetConnection())
            {
                return db.Query<Test1>(wherExpression: null).ToList();
            }
        }

        public void TestTest1()
        {
            var list = GetAll();

            foreach (var item in list)
            {
                Console.WriteLine(item.Id+"\t\t"+item.Name);
            }
        }
    }
}
