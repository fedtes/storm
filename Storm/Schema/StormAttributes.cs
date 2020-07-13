using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Storm.Schema
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class StormIgnore: Attribute
    {

    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class StormColumnName : Attribute
    {
        public string name;

        public StormColumnName(String name) => name = this.name;
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class StormPrimaryKey : Attribute
    {

    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class StormDefaultIfNull : Attribute
    {
        public object value;

        public StormDefaultIfNull(Object value) => value = this.value;
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class StormColumnType : Attribute
    {
        public DbType dbType;
        public int size;

        public StormColumnType(DbType dBType, int size)
        {
            dBType = this.dbType;
            size = this.size;
        }

    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class StormColumnAccess : Attribute
    {
        public ColumnAccess columnAccess;

        public StormColumnAccess(ColumnAccess columnAccess) => columnAccess = this.columnAccess;
    }

   

}
