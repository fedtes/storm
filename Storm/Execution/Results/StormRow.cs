﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Storm.Execution.Results
{
    public class StormRow : IEnumerable<KeyValuePair<string, object>>
    {
        private StormDataSet parent;
        private int index;
        private readonly int min = 0;
        private readonly int max;

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

        private IEnumerable<KeyValuePair<string, int>> rowColumns => parent.ColumnMap.Where(x => x.Value >= min && x.Value <= max);

        public object this[string key] => parent.data[index][Map(key)];

        public int Count => 1 + max - min;

        public IEnumerable<string> Keys => rowColumns.Select(x => x.Key);

        public bool IsReadOnly => true;

        public bool ContainsKey(string key)
        {
            return rowColumns.Any(x => x.Key == parent.NKey(key));
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return rowColumns.Select(x => x.Key).Select(k => new KeyValuePair<string, object>(k, this[k])).GetEnumerator();
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
    }
}