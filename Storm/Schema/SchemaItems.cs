using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using Storm.Filters;

namespace Storm.Schema
{

    /// <summary>
    /// Basic element of the schema from which derive <see cref="Entity"/>, <see cref="NavigationProperty"/>, <see cref="SimpleProperty"/>
    /// </summary>
    public abstract class AbstractSchemaItem
    {
        /// <summary>
        /// Unique string name of the item in the schema.
        /// </summary>
        public String Id { get; internal set; }

        internal abstract AbstractSchemaItem Clone();
    }

    /// <summary>
    /// Definition of a datamodel manipulated from the system.
    /// </summary>
    public class Entity : AbstractSchemaItem
    {
        /// <summary>
        /// Runtime-type model if specified
        /// </summary>
        public Type TModel { get; internal set; }
        /// <summary>
        /// Table name where entity data are stored
        /// </summary>
        public string DBName { get; internal set; }
        public SimpleProperty PrimaryKey => SimpleProperties.First(x => x.IsPrimary);
        public IEnumerable<BaseProperty> Properties { get; internal set; }

        public IEnumerable<SimpleProperty> SimpleProperties => Properties.Where(x=> typeof(SimpleProperty)==x.GetType()).Cast<SimpleProperty>();

        internal override AbstractSchemaItem Clone()
        {
            return new Entity()
            {
                Id = Id,
                DBName = DBName,
                TModel = TModel,
                Properties = Properties.Select(x => x.Clone()).Cast<BaseProperty>().ToList()
            };
        }

        public static bool operator ==(Entity x, Entity y) => !(x is null) && !(y is null) && x.Id == y.Id;
        public static bool operator !=(Entity x, Entity y) => !(!(x is null) && !(y is null) && x.Id == y.Id);

        public override bool Equals(object obj)
        {
            return this == (Entity)obj;
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        public override string ToString()
        {
            return $"{Id} (PK:{PrimaryKey.CodeName})";
        }
    }

    public enum ColumnAccess
    {
        Full = 0,
        CanInsert = 1,
        ReadOnly = 2
    }


    /// <summary>
    /// Base class definition for properties of <see cref="Entity"/> 
    /// </summary>
    public abstract class BaseProperty : AbstractSchemaItem
    {
        public string OwnerEntityId {get; internal set; }

        public string PropertyName {get; internal set; }
    }

    /// <summary>
    /// Simple property descriptor. This describe a property that is mapped directly into a db column
    /// </summary>
    public class SimpleProperty : BaseProperty
    {
        public string CodeName { get; internal set; }
        public string DBName { get; internal set; }
        public Type CodeType { get; internal set; }
        public DbType DBType { get; internal set; }
        public int Size { get; internal set; }
        public ColumnAccess ColumnAccess { get; internal set; }
        public Object DefaultIfNull { get; internal set; }
        public bool IsPrimary { get; internal set; }

        public override string ToString()
        {
            return $"SimpleProperty [{(IsPrimary ? "PK " : "")}{OwnerEntityId}].{PropertyName} -> [{CodeType.Name}]";
        }

        internal override AbstractSchemaItem Clone()
        {
            return new SimpleProperty()
            {
                Id = Id,
                CodeName = CodeName,
                DBName = DBName,
                CodeType = CodeType,
                DBType = DBType,
                Size = Size,
                ColumnAccess = ColumnAccess,
                DefaultIfNull = DefaultIfNull,
                IsPrimary = IsPrimary,
                OwnerEntityId = OwnerEntityId,
                PropertyName =  PropertyName
            };
        }
    }


    /// <summary>
    /// Virtual property of an entity that link one <see cref="Entity"/> to another.
    /// </summary>
    public class NavigationProperty : BaseProperty
    {
        public string TargetEntity { get; internal set; }
        public Tuple<string,string> On { get; internal set; }
        public Filter OnExpression { get; internal set; }

        internal override AbstractSchemaItem Clone()
        {

            return new NavigationProperty()
            {
                Id = Id,
                OwnerEntityId = OwnerEntityId,
                PropertyName =  PropertyName,
                TargetEntity = TargetEntity,
                On = On,
                OnExpression = OnExpression
            };
        }

        public override string ToString()
        {
            return $"NavigationProperty [{OwnerEntityId}].{PropertyName} -> [{TargetEntity}]";
        }
    }

}
