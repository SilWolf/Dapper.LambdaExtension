using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using Dapper.LambdaExtension.Helpers;
using Dapper.LambdaExtension.LambdaSqlBuilder.Adapter;
using Dapper.LambdaExtension.LambdaSqlBuilder.Attributes;
using Dapper.LambdaExtension.LambdaSqlBuilder.Entity;

namespace Dapper.LambdaExtension.LambdaSqlBuilder.Builder
{
    
    public partial class Builder
    {
        internal Builder(SqlType type, Type entityType, ISqlAdapter adapter)
        {
            _paramIndex = 0;
           
            this._adapter = adapter;
            this._type = type;
            this._useField = true;
            _entityType = entityType;
            var tabDef = _entityType.GetEntityDefines();

            var tname = tabDef.Item1.TableAttribute?.Name;
            if (string.IsNullOrEmpty(tname))
            {
                tname = tabDef.Item1.Name;
            }

            _tableNames.Add(tname);
            _schema = tabDef.Item1.TableAttribute?.Schema;
            _tableDefine = tabDef.Item1;
            _columnDefines = tabDef.Item2;
            this._parameterDic = new Dictionary<string, object>(); //new ExpandoObject();
        }

        internal Builder(SqlType type, SqlTableDefine tableDefine ,List<SqlColumnDefine> columnDefines,ISqlAdapter adapter)
        {
            _paramIndex = 0;

            this._adapter = adapter;
            this._type = type;
            this._useField = true;
             
            var tabDef = tableDefine;

            var tname = tabDef.TableAttribute?.Name;
            if (string.IsNullOrEmpty(tname))
            {
                tname = tabDef.Name;
            }

            _tableNames.Add(tname);
            _schema = tabDef.TableAttribute?.Schema;
            _tableDefine = tableDefine;
            _columnDefines = columnDefines;
            this._parameterDic = new Dictionary<string, object>(); //new ExpandoObject();
        }

        private ISqlAdapter _adapter;
        private SqlType _type;
        private bool _useField;
        private bool _userKey;

        internal bool _isSubQuery;

        internal Type _entityType;

        internal SqlTableDefine _tableDefine;
        internal List<SqlColumnDefine> _columnDefines;

        internal ISqlAdapter Adapter { get { return _adapter; } set { _adapter = value; } }

        //private const string PARAMETER_PREFIX = "Param";
        private readonly string PARAMETER_PREFIX = "P_" + DateTime.Now.Ticks.ToString();

        private readonly List<string> _tableNames = new List<string>();
        private readonly string _schema = string.Empty;
        private readonly List<string> _joinExpressions = new List<string>();
        private readonly List<string> _selectionList = new List<string>();
        private readonly List<string> _conditions = new List<string>();
        private readonly List<string> _sortList = new List<string>();
        private readonly List<string> _groupingList = new List<string>();
        private readonly List<string> _havingConditions = new List<string>();
        private readonly List<string> _splitColumns = new List<string>();
        private readonly List<string> _parameters = new List<string>();
        private int _paramIndex;
        private IDictionary<string, object> _parameterDic;

        public List<string> SplitColumns { get { return _splitColumns; } }
        public IDictionary<string, object> Parameters { get { return _parameterDic; } }

        public string SqlString()
        {
            string sql = string.Empty;
            SqlEntity entity = GetSqlEntity();
            switch (this._type)
            {
                case SqlType.Query:
                    sql = _adapter.Query(entity);
                    break;
                case SqlType.Insert:
                    sql = _adapter.Insert(_userKey, entity);
                    break;
                case SqlType.Update:
                    sql = _adapter.Update(entity);
                    break;
                case SqlType.Delete:
                    sql = _adapter.Delete(entity);
                    break;
                case SqlType.InsertValues :
                    sql = _adapter.InsertValues(entity);
                    break;
                default:
                    break;
            }
            return sql;
        }

        //public bool NeedOrderBy()
        //{
        //    if (_sortList.Count == 0 && _adapter is SqlserverAdapter)
        //    {
        //        return true;
        //    }
        //    return false;
        //}

        public string QueryPage(int pageSize, int? pageNumber = null)
        {
            SqlEntity entity = GetSqlEntity();
            entity.PageSize = pageSize;
            if (pageNumber.HasValue)
            {
                if (_sortList.Count == 0 && _adapter is SqlserverAdapter)
                {
                    var key=_columnDefines.FirstOrDefault(p => p.KeyAttribute != null);
                    if (key == null)
                    {
                        key = _columnDefines.FirstOrDefault(p => p.Name.ToUpper() == "ID" || p.AliasName == "ID");
                    }
                    if (key != null)
                    {
                        var orderbyname = string.IsNullOrEmpty(key.AliasName) ? key.Name : key.AliasName;

                        OrderBy(entity.TableName, orderbyname);

                        entity.OrderBy= GetForamtList(", ", "ORDER BY ", _sortList);
                    }
                    else
                    {
                        throw new Exception("Pagination requires the ORDER BY statement to be specified");
                    }
                }
                 
                entity.PageNumber = pageNumber.Value;
            }
            return _adapter.QueryPage(entity);
        }

        public string QuerySubPage(int pageSize,string aliasTable, int? pageNumber = null)
        {
            SqlEntity entity = GetSqlEntity();
            entity.PageSize = pageSize;
            if (pageNumber.HasValue)
            {
                if (_sortList.Count == 0 && _adapter is SqlserverAdapter)
                {
                    var key = _columnDefines.FirstOrDefault(p => p.KeyAttribute != null);
                    if (key == null)
                    {
                        key = _columnDefines.FirstOrDefault(p => p.Name.ToUpper() == "ID" || p.AliasName == "ID");
                    }
                    if (key != null)
                    {
                        var orderbyname = string.IsNullOrEmpty(key.AliasName) ? key.Name : key.AliasName;

                        OrderBy(aliasTable, orderbyname);

                        entity.OrderBy = GetForamtList(", ", "ORDER BY ", _sortList);
                    }
                    else
                    {
                        throw new Exception("Pagination requires the ORDER BY statement to be specified");
                    }
                }

                entity.PageNumber = pageNumber.Value;
            }
            return _adapter.QueryPage(entity);
        }

        public void Clear()
        {
            if (_joinExpressions.Count > 0)
            {
                string tableName = _tableNames[0];
                _tableNames.Clear();
                _joinExpressions.Clear();
                _tableNames.Add(tableName);
            }
            _selectionList.Clear();
            _conditions.Clear();
            _sortList.Clear();
            _groupingList.Clear();
            _havingConditions.Clear();
            _splitColumns.Clear();
            _parameters.Clear();
            _paramIndex = 0;
            this._parameterDic = new ExpandoObject();
            this._type = SqlType.Query;
        }

        public void UpdateSqlType(SqlType type)
        {
            this._type = type;
        }

        public void UpdateUseEntityProperty(bool use)
        {
            this._useField = use;
        }

        public void UpdateInsertKey(bool key)
        {
            _userKey = key;
        }

        #region Private

        private string GetTableName()
        {
            if (_isSubQuery)
            {
                return _tableNames.First();
            }
            var joinExpression = string.Join(" ", _joinExpressions);
            return string.Format("{0} {1}", _adapter.Table(_tableNames.First(),_schema), joinExpression);
        }
        private string GetSelection()
        {
            if (_selectionList.Count == 0)
            {
                //var columnList = new List<string>();

                SelectAll();

                return string.Join(",", _selectionList);
            }
            //return string.Format("{0}.*", _adapter.Table(_tableNames.First()));
            else
                return string.Join(", ", _selectionList);
        }

        public void SelectAll()
        {
            var entityDef = _entityType.GetEntityDefines();
            var tablename = entityDef.Item1.Name;
            if (string.IsNullOrEmpty(tablename))
            {
                tablename = entityDef.Item1.TableAttribute.Name;
            }

            var schema = entityDef.Item1.TableAttribute?.Schema;

            var tbname = _adapter.Table(tablename, schema);

            if (_isSubQuery && !string.IsNullOrEmpty(JoinSubAliasTableName))
            {
                tbname = JoinSubAliasTableName;
            }

            foreach (var cdef in entityDef.Item2)
            {
                var s = _adapter.Field(tbname, cdef.AliasName);

                if (!string.IsNullOrEmpty(cdef.AliasName))
                {
                    //s += " as " + _adapter.Field(cdef.Name);
                }
                else
                {
                    s = _adapter.Field(tbname, cdef.Name);
                }
                _selectionList.Add(s);
            }
        }

        public void SelectSub()
        {
            
        }

        private string GetForamtList(string join, string head, List<string> list)
        {
            if (list.Count == 0) return "";
            return head + string.Join(join, list);
        }

        private SqlEntity GetSqlEntity()
        {
            SqlEntity entity = new SqlEntity();
            entity.Having = GetForamtList(" ", "HAVING ", _havingConditions);
            entity.Grouping = GetForamtList(", ", "GROUP BY ", _groupingList);
            entity.OrderBy = GetForamtList(", ", "ORDER BY ", _sortList);
            entity.Conditions = GetForamtList("", "WHERE ", GetConditions());
            entity.Parameter = GetForamtList(", ", "", _parameters);
            entity.TableName = GetTableName();
            entity.Selection = GetSelection();
            return entity;
        }

        //private SqlEntity GetSqlEntity()
        //{
        //    SqlEntity entity = new SqlEntity();
        //    entity.Having = GetForamtList(" ", "HAVING ", _havingConditions);
        //    entity.Grouping = GetForamtList(", ", "GROUP BY ", _groupingList);
        //    entity.OrderBy = GetForamtList(", ", "ORDER BY ", _sortList);
        //    entity.Conditions = GetForamtList("", "WHERE ", _conditions);
        //    entity.Parameter = GetForamtList(", ", "", _parameters);
        //    entity.TableName = GetTableName();
        //    entity.Selection = GetSelection();
        //    return entity;
        //}

        private string GetParamId()
        {
            _paramIndex++;
            return PARAMETER_PREFIX + _paramIndex.ToString(CultureInfo.InvariantCulture);
        }

        private string GetParamId(string fieldName)
        {
            if (_useField)
            {
                if (_type == SqlType.Query)
                {
                    _paramIndex++;
                    return PARAMETER_PREFIX + "_" + _paramIndex.ToString(CultureInfo.InvariantCulture) + "_" + fieldName;
                }

                return fieldName;
            }
            return this.GetParamId();
        }

        private string GetCondition(string tableName, string fieldName, string op, object fieldValue)
        {
            string paramId = this.GetParamId(fieldName);
            string key = _adapter.Field(tableName, fieldName);
 
            string value = _adapter.Parameter(paramId);

            if (_parameterDic.ContainsKey(value))
            {
                value = value + Guid.NewGuid().ToString("n");
            }

            if (_isSubQuery)
            {
                key = _adapter.Field(JoinSubAliasTableName, fieldName);
                //value = JoinSubAliasTableName + value;
            }

            this.AddParameter(value, fieldValue);

            return string.Format("{0} {1} {2}", key, op, value);
        }

        private string GetCondition(string tableName, string fieldName, string aliasName, string op, object fieldValue)
        {
            string paramId = this.GetParamId(fieldName);
            string key = _adapter.Field(aliasName);
             
            string value = _adapter.Parameter(paramId);
            if (_parameterDic.ContainsKey(value))
            {
                value = value + Guid.NewGuid().ToString("n");
            }

            if (_isSubQuery)
            {
                key = _adapter.Field(JoinSubAliasTableName, fieldName);
                value = JoinSubAliasTableName + value;
            }

            this.AddParameter(value, fieldValue);
            return string.Format("{0} {1} {2}", key, op, value);
        }

        public List<string> GetConditions()
        {
            if (_type == SqlType.Update||_type==SqlType.Delete)
            {
                if (_conditions.Count == 0)
                {
                    var tabDef = _entityType.GetEntityDefines();

                    var keyField = tabDef.Item2.FirstOrDefault(p => p.KeyAttribute != null);

                    if (keyField == null)
                    {
                        throw new Exception("Must to define LamKey attribute to entity");
                    }

                    string paramId = keyField.AliasName ?? keyField.Name;
                    string key = _adapter.Field(paramId);
                    string value = _adapter.Parameter(paramId);

                    var condition = string.Format("{0} {1} {2}", key, "=", value);


                    return new List<string>() { condition };

                }
            }

            return _conditions;
        }

        private void AddParameter(string key, object value)
        {
            if (!_parameterDic.ContainsKey(key))
                _parameterDic.Add(key, value);
        }
        #endregion

        #region only for insert values building 

        public Tuple<string,ExpandoObject> GetInsertValuesParameters<T>(IEnumerable<PropertyInfo> pi,List<T> items)
        {
             
            var valuesStrList = new List<string>();
            var tmpDict =new  ExpandoObject() as IDictionary<string,object>;

            var ps = pi;
            for (var i = 0; i < items.Count; i++)
            {
                var paramlist=new List<string>();
                var objs = items[i];
                foreach (PropertyInfo item in ps)
                {
                    object obj = item.GetValue(objs, null);

                    var propname = item.Name+"_"+i.ToString();

                    //var fieldAlias = propname;

                    //resolve custome column name
                    //var colAttr = item.GetCustomAttribute<DBColumnAttribute>();
                    var colIgnore = item.GetCustomAttribute<DBIgnoreAttribute>();
                    if (colIgnore != null)
                    {
                        continue;
                    }
                 
                    var paramId = _adapter.Parameter(propname);
                    paramlist.Add(paramId);

                    tmpDict.Add(paramId,obj);
                    

                }
                valuesStrList.Add($"({string.Join(",",paramlist)})");
            }

            var valuesStr = string.Join(",", valuesStrList);

            return new Tuple<string, ExpandoObject>(valuesStr,tmpDict as ExpandoObject);
        }

        #endregion
    }
}
