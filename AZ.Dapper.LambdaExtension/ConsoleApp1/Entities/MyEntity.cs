using Dapper.LambdaExtension.LambdaSqlBuilder.Attributes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Dapper;
using Dapper.Contrib.Extensions;
using Dapper.LambdaExtension.Extentions;
using Npgsql;

namespace ConsoleApp1.Entities
{

    [DBTable("o_myentity")]
    public class MyEntity
    {
        [DBKey(true)] //the parameter increment define the field does auto increment 
        [DBColumn("id",dbType:DbType.Int32)]
        public int Id { get; set; }

        [DBColumn("myname",nullable:false,dbType:DbType.AnsiStringFixedLength,fieldLength:"64")]
        public string Name { get; set; }

        [DBColumn("created_date",DbType.DateTime2)]
        public DateTime CreateDate { get; set; }

        [DBColumn("is_deleted")]
        public bool Deleted { get; set; }
    }


    public class logictest
    {
        public void Test()
        {
            using (var connection = new NpgsqlConnection())
            {
                connection.CreateTable<MyEntity>();


                var entity=new MyEntity()
                {
                    Name = "myName"
                };

                connection.Insert(entity);

                connection.Update(entity);

                connection.Delete(entity);

                var results=connection.Query<MyEntity>(p => p.Name.Contains("name"));

                var resultlist = connection.Query<MyEntity>(sql =>
                {
                    sql.Where(p => p.Name.Contains("aa"));
                    sql.Or(p => p.Deleted == true);

                });
            }
        }
    }
}
