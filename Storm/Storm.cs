using Storm.Schema;
using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Storm.Test")]

namespace Storm
{
    public class Storm
    {
        internal readonly Schema.Schema schema;

        public Storm()
        {
            this.schema = new Schema.Schema();
        }

        public void EditSchema(Func<SchemaEditor, SchemaEditor> editor)
        {
            schema.EditSchema(editor);
        }
    }
}
