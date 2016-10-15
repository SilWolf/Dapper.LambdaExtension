﻿using System;

namespace AZ.Dapper.LambdaExtension.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ColumnAttribute : Attribute
    {
        public ColumnAttribute(string name)
        {
            this.Name = name;
        }
        public string Name { get; set; }
    }
}
