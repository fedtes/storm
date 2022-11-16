using Storm.Schema;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Storm.Execution
{
    public class StormRow : IEnumerable<KeyValuePair<string, object>>
    {
        internal readonly StormDataSet parent;
        internal readonly int index;
        internal readonly int min = 0;
        internal readonly int max;

        internal StormRow(StormDataSet parent, int index)
        {
            this.parent = parent;
            this.index = index;
            this.max = parent.data[index].Length - 1;
        }

        internal StormRow(StormDataSet parent, int index, int min, int max)
        {
            this.parent = parent;
            this.index = index;
            this.min = min;
            this.max = max > parent.data[index].Length -1 ? parent.data[index].Length - 1 : max;
        }

        internal int Map(String key)
        {
            var idx = parent.Map(key);
            if (idx < min || idx > max)
                throw new IndexOutOfRangeException();
            else
                return idx;
        }

        internal int Map(FieldPath key)
        {
            var idx = parent.Map(key);
            if (idx < min || idx > max)
                throw new IndexOutOfRangeException();
            else
                return idx;
        }

        private IEnumerable<KeyValuePair<FieldPath, int>> rowColumns => parent.ColumnMap.Where(x => x.Value >= min && x.Value <= max);

        public object this[FieldPath key] => parent.data[index][Map(key)];

        public object this[string key] => parent.data[index][Map(key)];

        public int Count => 1 + max - min;

        public IEnumerable<string> Keys => rowColumns.Select(x => x.Key.Path);

        public bool IsReadOnly => true;

        public bool ContainsKey(string key)
        {
            return rowColumns.Any(x => x.Key == parent.NFieldKey(key));
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return rowColumns.Select(x => x.Key).Select(k => new KeyValuePair<string, object>(k.Path, this[k])).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)this.GetEnumerator();
        }

        /// <summary>
        /// Persist the row data into a dictionary structure.
        /// </summary>
        /// <returns></returns>
        public Dictionary<string,object> ToDictionary()
        {
            return this.ToDictionary(x => x.Key, x => x.Value);
        }

        public override string ToString()
        {
            return $"[{ String.Join(", ",this.Select(kv => "{\"" + kv.Key + "\": " + "\"" + (kv.Value == null ? "" : kv.Value.ToString()) + "\"}")) }]";
        }
    }
}
