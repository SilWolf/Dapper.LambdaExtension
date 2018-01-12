using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Dapper.LambdaExtension.LambdaSqlBuilder;
using Dapper.LambdaExtension.LambdaSqlBuilder.Adapter;
using Dapper.LambdaExtension.LambdaSqlBuilder.Attributes;

namespace Dapper.LambdaExtension.Extentions
{
    public abstract  class TableBaseById<T,TDbFactory,TKey>:TableBase<T,TDbFactory> where T:class where TDbFactory : IDbFactory,new ()
    {

        public TKey Id { get; set; }

        public DateTime CreatedTime { get; set; }

 
    }

    public abstract class TableBaseByAutoId<T, TDbFactory> : TableBaseById<T, TDbFactory,int> where T : class where TDbFactory : IDbFactory, new()
    {

        [DBKey(true)]
        public new int Id { get; set; }
 
    }
}
