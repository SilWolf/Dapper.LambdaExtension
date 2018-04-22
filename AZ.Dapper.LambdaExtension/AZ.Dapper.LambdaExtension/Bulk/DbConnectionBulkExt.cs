using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Threading;
using Dapper.LambdaExtension.Extentions;
using Dapper.LambdaExtension.LambdaSqlBuilder;

namespace Dapper.LambdaExtension.Bulk
{
    public static class DbConnectionBulkExt
    {

        public static void BulkInsertValues<T>(this IDbConnection conn, List<T> itemlist, int batchSize = 1000, IDbTransaction trans = null, int? commandTimeout = null)
        {
            var sqlexp = conn.GetSqlExp<T>();

            sqlexp.InsertValues(key: false);

            var sqlstr = sqlexp.SqlString; //like insert into tablename (column1,colmn2, ...) values ,need to add the (c1value,c2value,...),(),...

            var pi = sqlexp._resolver.GetPropertyInfos<T>();

            var lastCont = 0;

            var batchCount = 1;

            if (itemlist.Count > batchSize)
            {
                lastCont = itemlist.Count % batchSize;
                batchCount = (int)Math.Ceiling((double)itemlist.Count / (double)batchSize);


                for (var i = 0; i < batchCount; i++)
                {
                    var offset = i * batchSize;

                    IEnumerable<T> templist;

                    if ((offset + batchSize) > itemlist.Count)
                    {
                        templist = itemlist.Skip(offset).Take(lastCont);
                    }
                    else
                    {
                        templist = itemlist.Skip(offset).Take(batchSize);
                    }

                    if (templist.Any())
                    {
                        var parameterDict = sqlexp._builder.GetInsertValuesParameters(pi, templist.ToList());

                        var finalSql = sqlstr + parameterDict.Item1;
                        dynamic parameter = parameterDict.Item2 as ExpandoObject;

                        CommandDefinition command = new CommandDefinition(finalSql, parameter, trans, commandTimeout, null, CommandFlags.NoCache, new CancellationToken());

                        conn.Execute(command);
                    }
                }
            }
            else
            {
                if (itemlist.Any())
                {
                    var parameterDict = sqlexp._builder.GetInsertValuesParameters(pi, itemlist.ToList());

                    var finalSql = sqlstr + parameterDict.Item1;
                    dynamic parameter = parameterDict.Item2 as ExpandoObject;

                    CommandDefinition command = new CommandDefinition(finalSql, parameter, trans, commandTimeout, null, CommandFlags.NoCache, new CancellationToken());

                    conn.Execute(command);
                }
            }
        }

        //public static void BulkInsertValues(this IDbConnection conn, List<object> itemlist, int batchSize = 1000, IDbTransaction trans = null,int? commandTimeout=null)
        //{
        //    var db = trans == null ? conn : trans.Connection;
        //    var sqllam = new SqlExp<object>(tableDefine, columnDefines, db.GetAdapter());

        //    sqllam = sqllam.Insert(tableDefine, columnDefines);
        //    var sqlString = sqllam.SqlString;
        //    try
        //    {

        //        DebuggingSqlString(sqlString);
        //        return db.Execute(sqlString, entity, trans, commandTimeout, CommandType.Text);
        //    }
        //    catch (Exception ex)
        //    {
        //        DebuggingException(ex, sqlString);
        //        throw new DapperLamException(ex.Message, ex, sqlString) { Parameters = sqllam.Parameters };
        //    }



        //    sqlexp.InsertValues(key: false);

        //    var sqlstr = sqlexp.SqlString; //like insert into tablename (column1,colmn2, ...) values ,need to add the (c1value,c2value,...),(),...

        //    var pi=sqlexp._resolver.GetPropertyInfos<object>();

        //    var lastCont = 0;

        //    var batchCount = 1;

        //    if (itemlist.Count > batchSize)
        //    {
        //        lastCont = itemlist.Count % batchSize;
        //        batchCount = (int)Math.Ceiling((double)itemlist.Count / (double) batchSize);
        //    }

        //    for (var i = 0; i < batchCount; i++)
        //    {
        //        var offset = i * batchSize;
 
        //        IEnumerable<object> templist;

        //        if ((offset + batchSize) > itemlist.Count)
        //        {
        //            templist = itemlist.Skip(offset).Take(lastCont);
        //        }
        //        else
        //        {
        //            templist = itemlist.Skip(offset).Take(batchSize);
        //        }

        //        if (templist.Any())
        //        {
        //            var parameterDict=sqlexp._builder.GetInsertValuesParameters(pi, templist.ToList());

        //            var finalSql = sqlstr + parameterDict.Item1;
        //            dynamic parameter = parameterDict.Item2 as ExpandoObject;
 
        //            CommandDefinition command = new CommandDefinition(finalSql, parameter, trans, commandTimeout, null, CommandFlags.NoCache, new CancellationToken());

        //            conn.Execute(command);
        //        }
        //    }
        //}
    }
}
