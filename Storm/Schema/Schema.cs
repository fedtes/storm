﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
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

        public Schema()
        {
            _schemas = new Dictionary<long, SchemaInstance>();
            _schemas.Add(_current, new SchemaInstance());
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
                    Task.Delay(5 * 60 * 1000)
                        .ContinueWith((t) => Task.Factory.StartNew(Clear));
                }
            }
        }

        private void Clear()
        {
            lock (monitor)
            {
                _schemas.Keys
                    .Where(k => k != _current)
                    .ToList()
                    .ForEach(k => _schemas.Remove(k));
            }
         }

    }

}
