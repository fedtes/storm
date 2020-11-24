using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Storm.Schema
{
    public class EntityBuilder
    {
        private List<EntityField> efs = new List<EntityField>();

        public EntityBuilder Add(FieldConfig field)
        {
            var f = new EntityField
            {
                CodeName = field.CodeName,
                DBName = field.DBName,
                CodeType = field.CodeType,
                DBType = field.DBType,
                Size = field.Size,
                ColumnAccess = field.ColumnAccess,
                DefaultIfNull = field.DefaultIfNull,
                IsPrimary = field.IsPrimary
            };

            efs.Add(f);
            return this;
        }

        internal bool HasPrimaryKey()
        {
            return efs.Any(x => x.IsPrimary);
        }

        internal SchemaItem GetEntity()
        {
            return new SchemaNode
            {
                entityFields = efs
            };
        }
    }

    public class FieldConfig
    {
        public string CodeName ;
        public string DBName ;
        public Type CodeType ;
        public DbType DBType ;
        public int Size ;
        public ColumnAccess ColumnAccess ;
        public Object DefaultIfNull ;
        public bool IsPrimary;
    }
}