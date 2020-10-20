using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Storm.Execution.Results;
using Storm.Helpers;
using Storm.Schema;

namespace Storm.Execution
{
    public class SelectCommand : Command<SelectCommand>
    {
        const String valudationPath = @"^([^ .{},[\]*]\.?)*([^*.[\]]+|\*)$";
        protected List<SelectNode> selectFields = new List<SelectNode>();

        public SelectCommand(SchemaNavigator navigator, string from) : base(navigator, from) { }

        public SelectCommand Select(string requestPath)
        {
            var p = SelectCommandHelper.ValidatePath(requestPath);

            foreach (var item in p)
            {
                var x = from.Resolve(item.Item1);
                IEnumerable<SelectNode> fields;
                if (item.Item2 != "*") 
                {
                    fields = x.Entity.entityFields
                        .Where(ef => ef.CodeName == item.Item2)
                        .Select(ef => {
                            return new SelectNode()
                            {
                                FullPath = new FieldPath(x.FullPath.Root, x.FullPath.Path, ef.CodeName),
                                EntityField = ef,
                                FromNode = x
                            };
                        });
                }
                else //wildcard = select all
                {
                    fields = x.Entity.entityFields
                        .Select(ef => {
                            return new SelectNode()
                            {
                                FullPath = new FieldPath(x.FullPath.Root, x.FullPath.Path, ef.CodeName),
                                EntityField = ef,
                                FromNode = x
                            };
                        });
                }
                selectFields.AddRange(fields);
            }
            return this;
        }

        internal override void ParseSQL()
        {
            base.ParseSQL();

            foreach (var field in selectFields)
            {
                base.query.Select($"{field.Alias}.{field.DBName} AS {field.Alias}${field.CodeName}");
            }
        }

        internal override object Read(IDataReader dataReader)
        {
            StormDataSet sr = new StormDataSet(this.rootEntity);
            sr.ReadData(dataReader, this.selectFields);
            return sr;
        }

        public new StormDataSet Execute()
        {
            return (StormDataSet)base.Execute();
        }
    }
}
