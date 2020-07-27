using Storm.Filters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Storm.Schema
{
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


        public SchemaEditor Add(String identifier, String sourceTable, Func<EntityBuilder, EntityBuilder> builder)
        {
            if (!schemaInstance.ContainsKey(identifier))
            {
                var b = builder(new EntityBuilder());
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

        public SchemaEditor Connect(String identifier, String sourceIdentifier, String targetIdentifier, String sourceField, String targetField)
        {
            if (schemaInstance.ContainsKey(identifier))
            {
                throw new ArgumentException($"Entity Connection with identifier {identifier} already exists.");
            }
            else if (!schemaInstance.ContainsKey(sourceIdentifier))
            {
                throw new ArgumentException($"Entity with identifier {identifier} not exists.");
            }
            else if (!schemaInstance.ContainsKey(targetIdentifier))
            {
                throw new ArgumentException($"Entity with identifier {identifier} not exists.");
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
                schemaInstance.Add(identifier, x);
            }

            return this;
        }

        public SchemaEditor Connect(String identifier, String sourceIdentifier, String targetIdentifier, Func<Expression, Expression, Filter> joinExpression)
        {
            var x = new SchemaEdge
            {
                ID = identifier,
                OnExpression = joinExpression(new Expression(), new Expression()),
                SourceID = sourceIdentifier,
                TargetID = targetIdentifier
            };
            schemaInstance.Add(identifier, x);
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

                    if (schemaNode.entityFields == null)
                    {
                        schemaNode.entityFields = new List<EntityField>();
                    }

                    ((List<EntityField>)schemaNode.entityFields).Add(f);
                }
            }

            var ps = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);

            ps.Select(p => new { p.Name, p.PropertyType, attr = getAttributes(p) })
                .Where(p => p.PropertyType.IsValueType || p.PropertyType == typeof(string))
                .ToList()
                .ForEach(mapToField);

            var fs = type.GetFields(BindingFlags.Instance | BindingFlags.Public);

            fs.Select(p => new { p.Name, p.FieldType, attr = getAttributes(p) })
                .Where(p => p.FieldType.IsValueType || p.FieldType == typeof(string))
                .ToList()
                .ForEach(mapToField);

            return schemaNode;
        }

        static private Dictionary<Type, DbType> typeMap = new Dictionary<Type, DbType>();
        
        private DbType standardTypeMap(Type propertyType)
        {
            if (!typeMap.Any())
            {
                typeMap[typeof(byte)] = DbType.Byte;
                typeMap[typeof(sbyte)] = DbType.SByte;
                typeMap[typeof(short)] = DbType.Int16;
                typeMap[typeof(ushort)] = DbType.UInt16;
                typeMap[typeof(int)] = DbType.Int32;
                typeMap[typeof(uint)] = DbType.UInt32;
                typeMap[typeof(long)] = DbType.Int64;
                typeMap[typeof(ulong)] = DbType.UInt64;
                typeMap[typeof(float)] = DbType.Single;
                typeMap[typeof(double)] = DbType.Double;
                typeMap[typeof(decimal)] = DbType.Decimal;
                typeMap[typeof(bool)] = DbType.Boolean;
                typeMap[typeof(string)] = DbType.String;
                typeMap[typeof(char)] = DbType.StringFixedLength;
                typeMap[typeof(Guid)] = DbType.Guid;
                typeMap[typeof(DateTime)] = DbType.DateTime;
                typeMap[typeof(DateTimeOffset)] = DbType.DateTimeOffset;
                typeMap[typeof(byte[])] = DbType.Binary;
                typeMap[typeof(byte?)] = DbType.Byte;
                typeMap[typeof(sbyte?)] = DbType.SByte;
                typeMap[typeof(short?)] = DbType.Int16;
                typeMap[typeof(ushort?)] = DbType.UInt16;
                typeMap[typeof(int?)] = DbType.Int32;
                typeMap[typeof(uint?)] = DbType.UInt32;
                typeMap[typeof(long?)] = DbType.Int64;
                typeMap[typeof(ulong?)] = DbType.UInt64;
                typeMap[typeof(float?)] = DbType.Single;
                typeMap[typeof(double?)] = DbType.Double;
                typeMap[typeof(decimal?)] = DbType.Decimal;
                typeMap[typeof(bool?)] = DbType.Boolean;
                typeMap[typeof(char?)] = DbType.StringFixedLength;
                typeMap[typeof(Guid?)] = DbType.Guid;
                typeMap[typeof(DateTime?)] = DbType.DateTime;
                typeMap[typeof(DateTimeOffset?)] = DbType.DateTimeOffset;
            }
            return typeMap[propertyType];
        }
    }
}
