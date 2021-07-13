using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Storm.Filters;
using System.Linq;
using Storm.Plugin;

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

    public class SchemaEdge : SchemaItem
    {
        public string SourceID { get; internal set; }
        public string TargetID { get; internal set; }
        public Tuple<string,string> On { get; internal set; }
        public Filter OnExpression { get; internal set; }

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
        private Dictionary<Guid, ILogService> _logServices = null;
        private long _current = 0;

        public Schema()
        {
            _schemas = new Dictionary<long, SchemaInstance>();
            _schemas.Add(_current, new SchemaInstance());
        }

        public SchemaNavigator GetNavigator()
        {
            Logger logger = new Logger(_logServices.Values.ToArray());
            return new SchemaNavigator(this._schemas[_current], logger);
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

        /// <summary>
        /// Register a logger to Storm. Return the unique id (guid) that allow to de-register it later.
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        public Guid AddLogger(ILogService log)
        {
            lock (monitor)
            {
                var g = Guid.NewGuid();
                if (_logServices == null) _logServices = new Dictionary<Guid, ILogService>();
                _logServices.Add(g, log);
                return g;
            }
        }

        /// <summary>
        /// Unregister a previously registered logger with a given unique id (guid)
        /// </summary>
        /// <param name="serviceid"></param>
        /// <returns></returns>
        public bool RemoveLogger(Guid serviceid)
        {
            lock (monitor)
            {
                if (_logServices == null) _logServices = new Dictionary<Guid, ILogService>();
                if (_logServices.ContainsKey(serviceid))
                {
                    _logServices.Remove(serviceid);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

    }

}
