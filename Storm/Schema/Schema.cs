using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Storm.Filters;
using System.Linq;
using Storm.Plugin;

namespace Storm.Schema
{
    class Schema
    {
        internal Object monitor = new object();
        internal Dictionary<long, SchemaInstance> _schemas;
        internal long _current = 0;
#pragma warning disable IDE0052 // Remove unread private members
        private readonly System.Threading.Timer timer;
#pragma warning restore IDE0052 // Remove unread private members

        public Schema()
        {
            _schemas = new Dictionary<long, SchemaInstance>();
            _schemas.Add(_current, new SchemaInstance());
            timer = new System.Threading.Timer(Clear, this, 0, 1800 * 1000);
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

        private static void Clear(object state)
        {
            lock (((Schema)state).monitor)
            {
                ((Schema)state)._schemas.Keys
                    .Where(k => k != ((Schema)state)._current)
                    .ToList()
                    .ForEach(k => ((Schema)state)._schemas.Remove(k));
            }
         }

    }

}
