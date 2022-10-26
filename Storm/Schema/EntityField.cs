using System;
using System.Data;

namespace Storm.Schema
{
    public enum ColumnAccess
    {
        Full = 0,
        CanInsert = 1,
        ReadOnly = 2
    }

    public class EntityField : SchemaItem
    {
        public string CodeName { get; internal set; }
        public string DBName { get; internal set; }
        public Type CodeType { get; internal set; }
        public DbType DBType { get; internal set; }
        public int Size { get; internal set; }
        public ColumnAccess ColumnAccess { get; internal set; }
        public Object DefaultIfNull { get; internal set; }
        public bool IsPrimary { get; internal set; }

        internal override SchemaItem Clone()
        {
            return new EntityField()
            {
                ID = ID,
                CodeName = CodeName,
                DBName = DBName,
                CodeType = CodeType,
                DBType = DBType,
                Size = Size,
                ColumnAccess = ColumnAccess,
                DefaultIfNull = DefaultIfNull,
                IsPrimary = IsPrimary
            };
        }
    }

}
