using Storm.Origins;
using Storm.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Reflection;
using System.Text;
using Storm.Helpers;

namespace Storm.Execution
{
    public class SetCommand : BaseCommand
    {
        protected IList<string> columns = new List<string>();
        protected IList<object> values = new List<object>();
        protected Dictionary<string, object> data = new Dictionary<string, object>();
        protected object id = null;
        protected SchemaNode entity;

        internal SetCommand(SchemaNavigator navigator, String from):base(navigator, from)
        {
            entity = navigator.GetEntity(this.rootEntity);
            ((BaseCommand)this).CommandLog(LogLevel.Info, "SetCommand", $"{{\"Action\":\"Update\", \"Entity\":\"{from}\"}}");
        }

        internal SetCommand(SchemaNavigator navigator, String from, Object id) : base(navigator, from)
        {
            this.id = id;
            entity = navigator.GetEntity(this.rootEntity);
            ((BaseCommand)this).CommandLog(LogLevel.Info, "SetCommand", $"{{\"Action\":\"Insert\", \"Entity\":\"{from}\"}}");
        }

        public SetCommand Value(Object model)
        {
            var modelfields = model.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);
            var modelprops = model.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
            var dmodel = new Dictionary<string, object>();

            foreach (var field in modelfields)
            {
                dmodel.Add(field.Name, field.GetValue(model));
            }

            foreach (var prop in modelprops)
            {
                dmodel.Add(prop.Name, prop.GetValue(model));
            }

            foreach (var item in entity.entityFields.Where(ef => !ef.IsPrimary && ef.ColumnAccess != ColumnAccess.ReadOnly))
            {
                if (dmodel.ContainsKey(item.CodeName))
                {
                    data.Add(item.DBName, dmodel[item.CodeName]);
                }
            }

            return this;
        }

        public SetCommand Value(IDictionary<String,Object> model)
        {
            foreach (var item in entity.entityFields.Where(ef => !ef.IsPrimary && ef.ColumnAccess != ColumnAccess.ReadOnly))
            {
                if (model.ContainsKey(item.CodeName))
                {
                    data.Add(item.DBName, model[item.CodeName]);
                }
            }
            return this;
        }

        internal override void ParseSQL()
        {
            if (IsInsert())
                query = query.AsInsert(data, true);
            else
                query = query.AsUpdate(data).Where(entity.PrimaryKey.DBName, this.id);

        }

        private bool IsInsert()
        {
            return null == this.id;
        }

        internal override object Read(IDataReader dataReader)
        {
            object output_id = null;
            if (IsInsert())
                while (dataReader.Read())
                {
                    output_id = dataReader.GetValue(0);
                }
            
            return new StormSetResult(output_id ?? this.id, dataReader.RecordsAffected);
        }

        public new StormSetResult Execute()
        {
            return (StormSetResult)base.Execute();
        }
    }
}
