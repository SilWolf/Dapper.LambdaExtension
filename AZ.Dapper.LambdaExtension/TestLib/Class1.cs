using System;
using System.Collections.Generic;
using System.Data;
using Dapper;
using Dapper.LambdaExtension.Extentions;
using Npgsql;

namespace TestLib
{
    public class Class1
    {
        public void TestLib()
        {
            IDbConnection db=new NpgsqlConnection();


            object t = 3;

            var list= db.Query<TestEnt>(sql =>
            {
                sql.WhereIsIn(p => p.Id, new List<int>());
            });

        }
    }


    class TestEnt
    {
        public int Id { get; set; }
    }
}
