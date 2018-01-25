using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apache.Ignite;
using Apache.Ignite.Core;
using Apache.Ignite.Core.Cache.Query;
using Apache.Ignite.Core.Impl;
using ConsoleApp1.Entities;
using testdemo.Entities;

namespace ConsoleApp1
{
    public class IgniteLogic
    {
        private IIgnite _instance;

        public IgniteLogic()
        {
            var cfg=new IgniteConfiguration()
            {

            };


            _instance = Ignition.TryGetIgnite() ?? Ignition.Start();

            var cache=_instance.CreateCache<int, Test3>("ab");

            var retcur=cache.Query(new SqlQuery(typeof(Test3), ""));

            var itemlist = retcur.GetAll().Select(p=>p.Value);


            
        }
    }
}
