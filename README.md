# Dapper.LambdaExtension
基于Dapper dotnet 的lambda表达式扩展.

## 支持的数据库
* Microsoft SqlServer
* MySql/Mariadb
* Postgresql
* SqlAnywhere
* Sqlite3
* SqlServerCE

> PS: 内置支持以上数据库的适配器,暂时不开放适配器扩展功能和自定义适配器接口


# 用法

## Attributes 使用

* DBTableAttribute  `定义表的数据库映射别名,和Schema名称,若无schema 指定为null,或不设置即可`  
* DBKeyAttribute `定义字段为主键,并可设定是否为自增字段`
* DBIgnoreAttribute `定义属性是否忽略,即在数据库中不存在此属性对应字段时使用`
* DBColumnAttribute `定义字段的数据库属性,name-数据库字段映射名称,nullable-是否为空,dbType-数据类型,fieldlength-若设定数据类型为可指定数据长度的可使用此参数设定,具体参见用法实例`

扩展的所有方法都是作为 IDbconnection 对象的扩展方法出现.

扩展基于Code First(POCO) 规范 ,具体用法如下 :

## 定义POCO类

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

## 创建表

    connection.CreateTable<MyEntity>();




## 增删改

    var entity=new MyEntity()
    {
        Name = "myName"
    };

    connection.Insert(entity);

    connection.Update(entity);

    connection.Delete(entity);

## 查询

### 简单表达式查询

    var results=connection.Query<MyEntity>(p => p.Name.Contains("name"));

`生成的T-Sql 语句: select id,myname,created_date,is_delete from o_myentity where myname like '%name%' `

### 复杂查询 























# License info note:
this project used lambda expression explain code from:
https://github.com/DomanyDusan/lambda-sql-builder

