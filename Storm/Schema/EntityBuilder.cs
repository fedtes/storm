using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Storm.Schema
{

    /// <summary>
    /// Use the Add methods to describe your entity by defining which fields it has.
    /// </summary>
    public class EntityBuilder
    {
        private List<BaseProperty> efs = new List<BaseProperty>();
        
        /// <summary>
        /// Add a field to the Entity. Only one field should be mark as primary and an Entity must has a primary field declared
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public EntityBuilder Add(FieldConfig field)
        {
            var f = new SimpleProperty
            {
                CodeName = field.CodeName,
                DBName = String.IsNullOrEmpty(field.DBName)? field.CodeName : field.DBName,
                CodeType = field.CodeType,
                DBType = field.DBType,
                Size = field.Size,
                ColumnAccess = field.ColumnAccess,
                DefaultIfNull = field.DefaultIfNull,
                IsPrimary = field.IsPrimary,
                Id= field.CodeName,
                PropertyName = field.CodeName
            };

            efs.Add(f);
            return this;
        }

        internal bool HasPrimaryKey()
        {
            return efs.Any(x => (x as SimpleProperty).IsPrimary);
        }

        internal AbstractSchemaItem GetEntity()
        {
            return new Entity
            {
                Properties = efs
            };
        }
    }

    public static class EntityBuilderExt
    {

        /// <summary>
        /// Add a field to the Entity
        /// </summary>
        /// <returns></returns>
        public static EntityBuilder Add(this EntityBuilder self, String codeName, Type codeType)
        {
            return self.Add(new FieldConfig()
            {
                CodeName = codeName,
                CodeType = codeType
            });
        }

        /// <summary>
        /// Add a field to the Entity
        /// </summary>
        /// <returns></returns>
        public static EntityBuilder Add(this EntityBuilder self, String codeName,Type codeType, String dbName, DbType dbType)
        {
            return self.Add(new FieldConfig()
            {
                CodeName = codeName,
                CodeType = codeType,
                DBName = dbName,
                DBType = dbType
            });
        }

        /// <summary>
        /// Add a field to the Entity
        /// </summary>
        /// <returns></returns>
        public static EntityBuilder Add(this EntityBuilder self, String codeName, Type codeType, String dbName, DbType dbType, Object defaultIfNull)
        {
            return self.Add(new FieldConfig()
            {
                CodeName = codeName,
                CodeType = codeType,
                DBName = dbName,
                DBType = dbType,
                DefaultIfNull = defaultIfNull
            });
        }

        /// <summary>
        /// Add a primary field to the Entity. Only one field should be mark as primary and an Entity must has a primary field declared
        /// </summary>
        /// <returns></returns>
        public static EntityBuilder AddPrimary(this EntityBuilder self, String codeName, Type codeType)
        {
            return self.Add(new FieldConfig()
            {
                CodeName = codeName,
                CodeType = codeType,
                IsPrimary = true
            });
        }

        /// <summary>
        /// Add a primary field to the Entity. Only one field should be mark as primary and an Entity must has a primary field declared
        /// </summary>
        /// <returns></returns>
        public static EntityBuilder AddPrimary(this EntityBuilder self, String codeName, Type codeType, String dbName, DbType dbType)
        {
            return self.Add(new FieldConfig()
            {
                CodeName = codeName,
                CodeType = codeType,
                DBName = dbName,
                DBType = dbType,
                IsPrimary = true
            });
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