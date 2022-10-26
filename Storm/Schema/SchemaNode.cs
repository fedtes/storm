using System;
using System.Collections.Generic;
using System.Linq;

namespace Storm.Schema
{
    public class SchemaNode : SchemaItem
    {
        public Type TModel { get; internal set; }
        public string DBName { get; internal set; }
        public EntityField PrimaryKey => entityFields.First(x => x.IsPrimary);
        public IEnumerable<EntityField> entityFields { get; internal set; }


        internal override SchemaItem Clone()
        {
            return new SchemaNode()
            {
                ID = ID,
                DBName = DBName,
                TModel = TModel,
                entityFields = entityFields.Select(x => x.Clone()).Cast<EntityField>().ToList()
            };
        }

        public static bool operator ==(SchemaNode x, SchemaNode y) => !(x is null) && !(y is null) && x.ID == y.ID;
        public static bool operator !=(SchemaNode x, SchemaNode y) => !(!(x is null) && !(y is null) && x.ID == y.ID);

        public override bool Equals(object obj)
        {
            return this == (SchemaNode)obj;
        }

        public override int GetHashCode()
        {
            return this.ID.GetHashCode();
        }
    }

}
