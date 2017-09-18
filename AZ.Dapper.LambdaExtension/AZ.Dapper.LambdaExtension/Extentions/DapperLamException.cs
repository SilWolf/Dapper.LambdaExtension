using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper
{
    public class DapperLamException:Exception
    {
        public DapperLamException(string message,Exception innerException,string errorSqlString):base(message,innerException)
        {
            SqlString = errorSqlString;
       
        }
        public string SqlString { get; set; }

        public IDictionary<string,object> Parameters { get; set; }

        public override string StackTrace {
            get { return InnerException?.StackTrace; }
        }
    }
}
