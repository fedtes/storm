using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Storm.Execution
{
    public class SelectRow : IDictionary<String, Object>
    {
        private Dictionary<String, Object> data = new Dictionary<string, object>();
        internal String root;

        internal SelectRow(String root)
        {
            this.root = root;
        }

        private String NKey(String key)
        {
            return key.StartsWith($"{root}.") ? key : $"{root}.{key}";
        }

        public object this[string key] { get => data[NKey(key)]; set => data[NKey(key)] = value; }

        public ICollection<string> Keys => data.Keys;

        public ICollection<object> Values => data.Values;

        public int Count => data.Count;

        public bool IsReadOnly => ((IDictionary<String, Object>)data).IsReadOnly;

        public void Add(string key, object value)
        {
            data.Add(NKey(key), value);
        }

        public void Add(KeyValuePair<string, object> item)
        {
            data.Add(NKey(item.Key), item.Value);
        }

        public void Clear()
        {
            data.Clear();
        }

        public bool Contains(KeyValuePair<string, object> item)
        {
            return data.ContainsKey(NKey(item.Key));
        }

        public bool ContainsKey(string key)
        {
            return data.ContainsKey(NKey(key));
        }

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            ((IDictionary<String, Object>)data).CopyTo(array, arrayIndex);
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return data.GetEnumerator();
        }

        public bool Remove(string key)
        {
            return data.Remove(NKey(key));
        }

        public bool Remove(KeyValuePair<string, object> item)
        {
            return data.Remove(NKey(item.Key));
        }

        public bool TryGetValue(string key, out object value)
        {
            return data.TryGetValue(NKey(key), out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return data.GetEnumerator();
        }
    }

    public class SelectResult : List<SelectRow>
    {
        private string root;

        internal SelectResult(String root)
        {
            this.root = root;
        }

        public void Add(string key, object value)
        {
            var x = new SelectRow(root);
            x.Add(key, value);
            this.Add(x);
        }
    }

}
