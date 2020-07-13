using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Storm.Filters;
using System.Linq;

namespace Storm.Schema
{

    public enum ColumnAccess
    {
        Full = 0,
        CanInsert = 1,
        ReadOnly = 2
    }

    public abstract class SchemaItem
    {
        public String ID { get; internal set; }

        internal abstract SchemaItem Clone();
    }

    public class SchemaNode : SchemaItem
    {
        public Type TModel { get; internal set; }
        public string DBName { get; internal set; }
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
    }

    public class SchemaEdge : SchemaItem
    {
        public string SourceID { get; internal set; }
        public string TargetID { get; internal set; }
        public Tuple<string,string> On { get; internal set; }
        public FilterExpression OnExpression { get; internal set; }

        internal override SchemaItem Clone()
        {

            return new SchemaEdge()
            {
                ID = ID,
                SourceID = SourceID,
                TargetID = TargetID,
                On = On,
                OnExpression = OnExpression
            };
        }
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
                DefaultIfNull = DefaultIfNull
            };
        }
    }

    public class SchemaInstance : Dictionary<string, SchemaItem>
    {
        public SchemaInstance Clone()
        {
            var i = new SchemaInstance();

            foreach (var si in this)
            {
                i.Add(si.Key, si.Value.Clone());
            }
            return i;
        }
    }

    class Schema
    {
        private Object monitor = new object();
        private Dictionary<long, SchemaInstance> _schemas;
        private long _current = 0;

        public Schema()
        {
            _schemas = new Dictionary<long, SchemaInstance>();
        }

        public SchemaNavigator GetNavigator()
        {
            return new SchemaNavigator(this._schemas[_current]);
        }

        public void EditSchema(Func<SchemaEditor, SchemaEditor> editor)
        {
            SchemaEditor schemaEditor = new SchemaEditor(this._schemas[_current].Clone(), DateTime.Now.Ticks);
            var r = editor(schemaEditor);
            if (r.ticks > _current)
            {
                lock (monitor)
                {
                    _schemas.Add(r.ticks, r.schemaInstance);
                    _current = r.ticks;
                }
            }
        }
    }

}
