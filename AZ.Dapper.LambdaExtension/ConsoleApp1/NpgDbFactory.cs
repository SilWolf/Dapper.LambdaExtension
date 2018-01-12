using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using Dapper.LambdaExtension.Extentions;
using Npgsql;

namespace ConsoleApp1
{
    public class NpgDbFactory :DbFactoryBase<NpgsqlConnection>
    { 
        

        public NpgDbFactory()
        {
            
            ConnectionString = "server=192.168.10.25;User Id=postgres;password=ZeroPlus2013;database=testdb;Encoding=UTF-8;";
        }

        
    }
}
