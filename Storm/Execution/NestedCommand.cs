using Storm.Helpers;
using Storm.Schema;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Storm.Execution
{
    public class NestedCommand : Command<NestedCommand>
    {

        internal SelectNode outputField = null;

        internal NestedCommand(Context ctx, string from) : base(ctx, from) { }

        internal override void ParseSQL()
        {
            base.ParseSQL();

            // if no output field specified use the primary key as fallback
            if (outputField is null)
            {
                outputField = new SelectNode()
                {
                    FullPath = new FieldPath(from.root.Entity.ID, from.root.Entity.ID, from.root.Entity.PrimaryKey.CodeName),
                    EntityField = from.root.Entity.PrimaryKey,
                    FromNode = from.root
                };
            }

            base.query.Select($"{outputField.Alias}.{outputField.DBName} AS {outputField.Alias}${outputField.CodeName}");
        }

        public NestedCommand Select(string requestPath)
        {
            var _requestPath = new EntityPath(from.root.Entity.ID, requestPath).Path;
            var p = SelectCommandHelper.ValidatePath(_requestPath);

            (string[], string) item;

            if (p.Count() > 1)
                throw new ArgumentException("Only one field should be returned by sub queries");
            else if (p.Count() == 0)
                throw new ArgumentException("At least one field should be returned by sub queries");
            else
                item = p.First();

            if (item.Item2 == "*")
                throw new ArgumentException("Select all not allowed in sub queries.");

            outputField = SelectCommandHelper.GenerateSingleSelectNode(item, from);

            return this;
        }

        internal override object Read(IDataReader dataReader)
        {
            throw new NotImplementedException("This method should not be used here");
        }
    }
}
