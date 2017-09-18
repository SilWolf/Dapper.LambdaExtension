using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
 


namespace ConsoleApp1
{
    public class MsSqlBase
    {
        //protected string Connstr = "data source=192.168.10.24;Database=eftestdb;Password=ZeroPlus2013;User ID=sa;";
        protected string Connstr = "data source=192.168.10.24;Database=airbox2;Password=ZeroPlus2013;User ID=sa;";

        public MsSqlBase()
        {


            // base(connstr);
        }

        public MsSqlBase(string connstr)
        {

            Connstr = connstr;
        }

        public IDbConnection GetConnection()
        {
            var conn = new SqlConnection(Connstr);
            conn.Open();
            return conn;
        }


        public IDbConnection GetConnection(string strConn)
        {
            var conn = new SqlConnection(strConn);
            conn.Open();
            return conn;
        }


        public Tuple<bool, string> TestConn(string connstring)
        {
            bool isopen = false;
            string msg = string.Empty;

            try
            {
                var conn = GetConnection(connstring);
                if (conn.State == ConnectionState.Open)
                {
                    isopen = true;
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }


            return new Tuple<bool, string>(isopen, msg);
        }
        public Tuple<bool, string> TestConn()
        {
            bool isopen = false;
            string msg = string.Empty;

            try
            {
                var conn = GetConnection(Connstr);
                if (conn.State == ConnectionState.Open)
                {
                    isopen = true;
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }


            return new Tuple<bool, string>(isopen, msg);
        }
    }
}
