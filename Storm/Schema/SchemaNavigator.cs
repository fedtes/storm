using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Storm.Schema
{
    public class SchemaNavigator
    {
        private SchemaInstance p;

        internal SchemaNavigator(SchemaInstance p) => this.p = p;

        public AbstractSchemaItem Get(String identifier)
        {
            if (p.ContainsKey(identifier))
                return p[identifier];
            else
                return null;
        }

        public Entity GetEntity(String identifier)
        {
            if (p.ContainsKey(identifier))
            {
                var x = p[identifier];
                if (x.GetType() == typeof(Entity))
                {
                    return (Entity)x;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public NavigationProperty GetNavigationProperty(String EntityDotProperty)
        {
            if (EntityDotProperty.IndexOf(".") == -1) {
                throw new ArgumentException("Expected string like EnitityName.PropertyName");
            }

            var _splitted = EntityDotProperty.Split(".");
            var _entityId = _splitted[0];
            var _propertyName = _splitted[1];

            if (p.ContainsKey(_entityId))
            {
                var ownerEntity = p[_entityId];
                if (ownerEntity.GetType() == typeof(Entity))
                {   
                    var prop = (ownerEntity as Entity).Properties.FirstOrDefault(x => typeof(NavigationProperty)==x.GetType() && (x as NavigationProperty).PropertyName == _propertyName);
                    return (NavigationProperty)prop;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

    }
}
