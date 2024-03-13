using Storm.Schema;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;

namespace Storm.Execution
{
    /// <summary>
    /// Define a dynamic object that act as a proxy to access an Entity and its properties or to a subset of properties an entity in case of Select clause (projection)
    /// </summary>
    public class DynamicResult : DynamicObject
    {
        private StormRow datarow;
        private Entity _node;
        private Dictionary<string, string>  _propertyMap = new Dictionary<string, string>();

        internal Dictionary<string, IEnumerable<DynamicResult>> Relations = new Dictionary<string, IEnumerable<DynamicResult>>();


        internal DynamicResult(StormRow datarow, Entity node)
        {
            this.datarow = datarow;
            _node = node;
            String getField(string x)
            {
                var i = x.LastIndexOf('.');
                return x.Substring(i + 1);
            };

            foreach (var item in this.datarow.Keys)
            {
                var _fieldName = getField(item);
                if (_node.SimpleProperties.Any(x => x.CodeName.ToLowerInvariant() == _fieldName.ToLowerInvariant()))
                {
                    _propertyMap.Add(_fieldName, item);
                }
            }

        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (_propertyMap.ContainsKey(binder.Name) && datarow.ContainsKey(_propertyMap[binder.Name]))
            {
                result = datarow[_propertyMap[binder.Name]];
                return true;
            }
            else
            {
                result = GetRelation(binder.Name);
                return null != result;
            }
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            return false;
        }

        public object PrimaryKey =>datarow[_propertyMap[_node.PrimaryKey.CodeName]];

        public bool HasModel()
        {
            return _node.TModel != null;
        }

        public T GetModel<T>()
        {
            if (HasModel())
            {
                if (datarow == null) return default(T);

                var _val = datarow;
                T obj = (T)Activator.CreateInstance(_node.TModel);
                foreach (var fieldInfo in typeof(T).GetFields())
                {
                    if (_propertyMap.ContainsKey(fieldInfo.Name))
                        fieldInfo.SetValue(obj, _val[_propertyMap[fieldInfo.Name]]);
                }
                foreach (var fieldInfo in typeof(T).GetProperties())
                {
                    if (_propertyMap.ContainsKey(fieldInfo.Name))
                        fieldInfo.SetValue(obj, _val[_propertyMap[fieldInfo.Name]]);
                }
                return obj;
            }
            else
            {
                throw new ArgumentException($"Model not specified for Entity {_node.Id}.");
            }
        }

        public Dictionary<string,object> GetDictionary()
        {
            var result = new Dictionary<string, object>();
            if (datarow == null) return result;

            var _val = datarow;
            foreach (var item in _node.SimpleProperties)
            {
                result.Add(item.CodeName, _val[_propertyMap[item.CodeName]]);
            }
            return result;
        }

        public IEnumerable<DynamicResult> GetRelation(String Path)
        {
            if (Relations.ContainsKey(Path))
                return Relations[Path];
            else
                return null;
        }

        public override string ToString()
        {
            return $"PrimaryKey [{_node.PrimaryKey.CodeName}] = {PrimaryKey}";
        }

    }

}
