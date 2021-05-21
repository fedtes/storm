using Storm.Filters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Storm.Schema
{
    /// <summary>
    /// Allow to manipulate the schema that storm uses for generating queries
    /// </summary>
    public class SchemaEditor
    {
        internal SchemaInstance schemaInstance;
        internal long ticks;

        public SchemaEditor(SchemaInstance schemaInstance, long ticks)
        {
            this.schemaInstance = schemaInstance;
            this.ticks = ticks;
        }

        /*
        public SchemaEditor Add(String identifier, String sourceTable)
        {
            if (!schemaInstance.ContainsKey(identifier))
            {
                schemaInstance.Add(identifier, new SchemaNode { ID = identifier, DBName = sourceTable });
            } else
            {
                throw new ArgumentException($"Entity with identifier {identifier} already exists.");
            }

            return this;
        }
        */

        /// <summary>
        /// Add new Entity to the schema. Use StormAttributes to decorate the fields and properties of the TModel to describe the entity.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="identifier"></param>
        /// <param name="sourceTable"></param>
        /// <returns></returns>
        public SchemaEditor Add<TModel>(String identifier, String sourceTable)
        {
            if (!schemaInstance.ContainsKey(identifier))
            {
                schemaInstance.Add(identifier, parseModel(this, identifier, sourceTable, typeof(TModel)));
            }
            else
            {
                throw new ArgumentException($"Entity with identifier {identifier} already exists.");
            }
            return this;
        }

        /// <summary>
        /// Add new Entity to the schema. This Entity does not has a strong-typed model so use the EntityBuilder to describe it.
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="sourceTable"></param>
        /// <param name="builder"></param>
        /// <returns></returns>
        public SchemaEditor Add(String identifier, String sourceTable, Func<EntityBuilder, EntityBuilder> builder)
        {
            if (!schemaInstance.ContainsKey(identifier))
            {
                var b = builder(new EntityBuilder());
                if (!b.HasPrimaryKey())
                {
                    throw new NoPrimaryKeySpecifiedException("No primary key specified for " + identifier);
                }
                var e = b.GetEntity();
                ((SchemaNode)e).DBName = sourceTable;
                e.ID = identifier;
                schemaInstance.Add(identifier, e);
            }
            else
            {
                throw new ArgumentException($"Entity with identifier {identifier} already exists.");
            }
            return this;
        }

        /// <summary>
        /// Remove an object from the schema, can be either an Entity or an Edge. For edges use SourceEntityIdentifier.Identifier as argument of this method. You should be sure that removing an items all other elements already registered are consistent.
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        public SchemaEditor Remove(String identifier)
        {
            if (schemaInstance.ContainsKey(identifier))
            {
                schemaInstance.Remove(identifier);
            }
            else
            {
                throw new ArgumentException($"Entity with identifier {identifier} not exists.");
            }
            return this;
        }

        /// <summary>
        /// Connect 2 Entities give a specific identifier.
        /// </summary>
        /// <param name="identifier">Identifier of the relation</param>
        /// <param name="sourceIdentifier">Source (or left) entity identifier</param>
        /// <param name="targetIdentifier">Target (or right) entity identifier</param>
        /// <param name="joinExpression">Expression that describe how to join. In the sintax refer as 'source' for the source object and 'target' for the target</param>
        /// <returns></returns>
        public SchemaEditor Connect(String identifier, String sourceIdentifier, String targetIdentifier, String sourceField, String targetField)
        {
            
            if (!schemaInstance.ContainsKey(sourceIdentifier))
            {
                throw new ArgumentException($"Entity with identifier {sourceIdentifier} not exists.");
            }
            else if (!schemaInstance.ContainsKey(targetIdentifier))
            {
                throw new ArgumentException($"Entity with identifier {targetIdentifier} not exists.");
            }
            else
            {
                var x = new SchemaEdge
                {
                    ID = $"{sourceIdentifier}.{identifier}",
                    On = (sourceField, targetField).ToTuple(),
                    SourceID = sourceIdentifier,
                    TargetID = targetIdentifier
                };
                schemaInstance.Add(x.ID, x);
            }

            return this;
        }

        /// <summary>
        /// Connect 2 Entities give a specific identifier.
        /// </summary>
        /// <param name="identifier">Identifier of the relation</param>
        /// <param name="sourceIdentifier">Source (or left) entity identifier</param>
        /// <param name="targetIdentifier">Target (or right) entity identifier</param>
        /// <param name="joinExpression">Expression that describe how to join. In the sintax refer as 'source' for the source object and 'target' for the target</param>
        /// <returns></returns>
        public SchemaEditor Connect(String identifier, String sourceIdentifier, String targetIdentifier, Func<JoinContext, Filter> joinExpression)
        {

            if (!schemaInstance.ContainsKey(sourceIdentifier))
            {
                throw new ArgumentException($"Entity with identifier {sourceIdentifier} not exists.");
            }
            else if (!schemaInstance.ContainsKey(targetIdentifier))
            {
                throw new ArgumentException($"Entity with identifier {targetIdentifier} not exists.");
            }
            else
            {
                var x = new SchemaEdge
                {
                    ID = $"{sourceIdentifier}.{identifier}",
                    OnExpression = joinExpression(new JoinContext()),
                    SourceID = sourceIdentifier,
                    TargetID = targetIdentifier
                };
                schemaInstance.Add(x.ID, x);
            }

            return this;
        }

        private SchemaItem parseModel(SchemaEditor schemaEditor, string identifier, string sourceTable, Type type)
        {
            SchemaNode schemaNode = new SchemaNode
            {
                TModel = type,
                DBName = sourceTable,
                ID = identifier
            };

            object[] getAttributes(MemberInfo mi) {
                return mi.GetCustomAttributes(true);
            }

            bool hasAttribute<StormAttribute>(object[] vs)
            {
                return vs.Any(a => a.GetType() == typeof(StormAttribute));
            }

            StormAttribute getAttribute<StormAttribute>(object[] vs)
            {
                return (StormAttribute)vs.First(a => a.GetType() == typeof(StormAttribute));
            }

            void mapToField(dynamic x)
            {
                if (!hasAttribute<StormIgnore>(x.attr))
                {
                    var f = new EntityField();
                    f.CodeName = x.Name;
                    f.CodeType = x.PropertyType;
                    f.DBName = hasAttribute<StormColumnName>(x.attr) ? getAttribute<StormColumnName>(x.attr).name : x.Name;
                    f.DBType = hasAttribute<StormColumnType>(x.attr) ? getAttribute<StormColumnType>(x.attr).dbType : standardTypeMap(x.PropertyType);
                    f.Size = hasAttribute<StormColumnType>(x.attr) ? getAttribute<StormColumnType>(x.attr).size : 0;
                    f.ColumnAccess = hasAttribute<StormColumnAccess>(x.attr) ? getAttribute<StormColumnAccess>(x.attr).columnAccess : ColumnAccess.Full;
                    f.DefaultIfNull = hasAttribute<StormDefaultIfNull>(x.attr) ? getAttribute<StormDefaultIfNull>(x.attr).value : null;
                    f.IsPrimary = hasAttribute<StormPrimaryKey>(x.attr);

                    if (schemaNode.entityFields == null)
                    {
                        schemaNode.entityFields = new List<EntityField>();
                    }

                    ((List<EntityField>)schemaNode.entityFields).Add(f);
                }
            }

            var ps = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);

            ps.Select(p => new { p.Name, PropertyType = p.PropertyType, attr = getAttributes(p) })
                .Where(p => p.PropertyType.IsValueType || p.PropertyType == typeof(string))
                .ToList()
                .ForEach(mapToField);

            var fs = type.GetFields(BindingFlags.Instance | BindingFlags.Public);

            fs.Select(p => new { p.Name, PropertyType = p.FieldType, attr = getAttributes(p) })
                .Where(p => p.PropertyType.IsValueType || p.PropertyType == typeof(string))
                .ToList()
                .ForEach(mapToField);

            if (!schemaNode.entityFields.Any(x => x.IsPrimary))
            {
                throw new NoPrimaryKeySpecifiedException("No primary key specified for " + identifier);
            }

            return schemaNode;
        }

        static private Dictionary<Type, DbType> _typeMap;
        static private Object monitor = new object();

        static private Dictionary<Type, DbType> typeMap 
        {
            get 
            {
                if (_typeMap == null)
                {
                    lock (monitor)
                    {
                        if (_typeMap == null)
                        {
                            _typeMap = new Dictionary<Type, DbType>();
                            _typeMap[typeof(byte)] = DbType.Byte;
                            _typeMap[typeof(sbyte)] = DbType.SByte;
                            _typeMap[typeof(short)] = DbType.Int16;
                            _typeMap[typeof(ushort)] = DbType.UInt16;
                            _typeMap[typeof(int)] = DbType.Int32;
                            _typeMap[typeof(uint)] = DbType.UInt32;
                            _typeMap[typeof(long)] = DbType.Int64;
                            _typeMap[typeof(ulong)] = DbType.UInt64;
                            _typeMap[typeof(float)] = DbType.Single;
                            _typeMap[typeof(double)] = DbType.Double;
                            _typeMap[typeof(decimal)] = DbType.Decimal;
                            _typeMap[typeof(bool)] = DbType.Boolean;
                            _typeMap[typeof(string)] = DbType.String;
                            _typeMap[typeof(char)] = DbType.StringFixedLength;
                            _typeMap[typeof(Guid)] = DbType.Guid;
                            _typeMap[typeof(DateTime)] = DbType.DateTime;
                            _typeMap[typeof(DateTimeOffset)] = DbType.DateTimeOffset;
                            _typeMap[typeof(byte[])] = DbType.Binary;
                            _typeMap[typeof(byte?)] = DbType.Byte;
                            _typeMap[typeof(sbyte?)] = DbType.SByte;
                            _typeMap[typeof(short?)] = DbType.Int16;
                            _typeMap[typeof(ushort?)] = DbType.UInt16;
                            _typeMap[typeof(int?)] = DbType.Int32;
                            _typeMap[typeof(uint?)] = DbType.UInt32;
                            _typeMap[typeof(long?)] = DbType.Int64;
                            _typeMap[typeof(ulong?)] = DbType.UInt64;
                            _typeMap[typeof(float?)] = DbType.Single;
                            _typeMap[typeof(double?)] = DbType.Double;
                            _typeMap[typeof(decimal?)] = DbType.Decimal;
                            _typeMap[typeof(bool?)] = DbType.Boolean;
                            _typeMap[typeof(char?)] = DbType.StringFixedLength;
                            _typeMap[typeof(Guid?)] = DbType.Guid;
                            _typeMap[typeof(DateTime?)] = DbType.DateTime;
                            _typeMap[typeof(DateTimeOffset?)] = DbType.DateTimeOffset;
                        }
                    }
                }
                return _typeMap;
            }
        }

        
        private DbType standardTypeMap(Type propertyType)
        {
            return typeMap[propertyType];
        }
    }
}
