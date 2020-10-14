using Storm.Schema;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;

namespace Storm.Execution.Results
{

    public class StormResult
    {
        public Dictionary<string, StormResult> Relations = new Dictionary<string, StormResult>();

        private StormRange _range;
        private SchemaNode _node;
        private Dictionary<string, string>  _propertyMap = new Dictionary<string, string>();

        internal StormResult(StormRange range, SchemaNode node)
        {
            _range = range;
            _node = node;
            String getField(string x)
            {
                var i = x.LastIndexOf('.');
                return x.Substring(i + 1);
            };

            foreach (var item in _range.Rows.First().Keys)
            {
                var _fieldName = getField(item);
                if (_node.entityFields.Any(x => x.CodeName.ToLowerInvariant() == _fieldName.ToLowerInvariant()))
                {
                    _propertyMap.Add(_fieldName, item);
                }
            }

        }

        private dynamic _dynamicModel = null;
        private dynamic createDynamicModel()
        {
            _dynamicModel =  new ModelItem(_range.Rows.First(), _propertyMap);
            return _dynamicModel;
        }

        public dynamic Value => _dynamicModel == null ? createDynamicModel() : _dynamicModel;

        public bool HasModel()
        {
            return _node.TModel != null;
        }

        public T GetModel<T>()
        {
            if (HasModel())
            {
                if (!_range.Rows.Any()) return default(T);

                var _val = _range.Rows.First();
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
                throw new ArgumentException($"Model not specified for Entity {_node.ID}.");
            }
        }

        public Dictionary<string,object> GetDictionary()
        {
            var result = new Dictionary<string, object>();
            if (!_range.Rows.Any()) return result;

            var _val = _range.Rows.First();
            foreach (var item in _node.entityFields)
            {
                result.Add(item.CodeName, _val[_propertyMap[item.CodeName]]);
            }
            return result;
        }

    }

    public class ModelItem : DynamicObject
    {

        private readonly StormRow _data;
        private readonly Dictionary<string, string> propertyMap;

        internal ModelItem(StormRow data, Dictionary<string,string> PropertyMap)
        {
            _data = data;
            propertyMap = PropertyMap;
        }
        // If you try to get a value of a property
        // not defined in the class, this method is called.
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            // If the property name is found in a dictionary,
            // set the result parameter to the property value and return true.
            // Otherwise, return false.
            if (_data.ContainsKey(binder.Name))
            {
                result = _data[propertyMap[binder.Name]];
                return true;
            } else
            {
                result = null;
                return false;
            }
        }

        // If you try to set a value of a property that is
        // not defined in the class, this method is called.
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            return false;
        }
    }
}
