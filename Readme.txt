# Dapper.LambdaExtension
this project provide the simple lambda expression support to Dapper as extension.

## Support database
* Microsoft SqlServer
* MySql/Mariadb
* Postgresql
* SqlAnywhere
* Sqlite3
* SqlServerCE

> PS: there only support above list database. and no custome interface to support other database and rewrite existing database adapter.


# Usage

## Attribute usage

* DBTableAttribute  `define alternative table name and schema`  
* DBKeyAttribute `define the column is the key field and set up is or not auto increment`
* DBIgnoreAttribute `define`
* DBColumnAttribute

this project followed POCO Entity usage. like this:

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


## For CRUD






















# License info note:
this project used lambda expression explain code from:
https://github.com/DomanyDusan/lambda-sql-builder

