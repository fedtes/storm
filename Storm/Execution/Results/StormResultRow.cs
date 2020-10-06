using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Storm.Execution.Results
{
    public class StormResultRow : IEnumerable<KeyValuePair<string, object>>
    {
        private StormResult parent;
        private int index;

        internal StormResultRow(StormResult parent, int index)
        {
            this.parent = parent;
            this.index = index;
        }

        public object this[string key] => parent.data[index][parent.columnMap[parent.NKey(key)]];

        public ICollection<string> Keys => parent.columnMap.Keys;

        public ICollection<object> Values => parent.data[index];

        public int Count => parent.columnMap.Count;

        public bool IsReadOnly => true;

        public bool ContainsKey(string key)
        {
            return parent.columnMap.ContainsKey(parent.NKey(key));
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return parent.columnMap.Keys.Select(k => new KeyValuePair<string, object>(k, this[k])).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)this.GetEnumerator();
        }
    }
}
