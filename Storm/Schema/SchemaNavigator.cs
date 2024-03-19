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

        /// <summary>
        /// Return the target <see cref="AbstractSchemaItem"/> based on the path passed as argument. Support dot notation. 
        /// Returns either <see cref="Entity"/>, <see cref="SimpleProperty"/> or <see cref="NavigationProperty"/> based on the path resolution.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public AbstractSchemaItem Get(String path)
        {
            return GetFullPath(path).Last();
        }

        /// <summary>
        /// Return the target element based on the path passed as argument and cast into the specified type if possibile.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public T Get<T>(String path) where T:AbstractSchemaItem
        {
            return GetFullPath(path).Last() as T;
        }


        /// <summary>
        /// Return the target <see cref="AbstractSchemaItem"/> based on the path passed as argument. Support dot notation. 
        /// Returns either <see cref="Entity"/>, <see cref="SimpleProperty"/> or <see cref="NavigationProperty"/> based on the path resolution.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public AbstractSchemaItem Get(Path path)
        {
            return GetFullPath(path).Last();
        }

        /// <summary>
        /// Return the target element based on the path passed as argument and cast into the specified type if possibile.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public T Get<T>(Path path) where T:AbstractSchemaItem
        {
            return GetFullPath(path).Last() as T;
        }

        /// <summary>
        /// Return the full navigated path to resolve the argument path as a <see cref="IEnumerable<AbstractSchemaItem>"/> based on the path passed as argument. Support dot notation.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public IEnumerable<AbstractSchemaItem> GetFullPath(String path)
        {
            if(String.IsNullOrEmpty(path)) throw new ArgumentNullException("path");
            return GetFullPath(new Path(path));
        }

        /// <summary>
        /// Return the full navigated path to resolve the argument path as a <see cref="IEnumerable<AbstractSchemaItem>"/> based on the path passed as argument. Support dot notation.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public IEnumerable<AbstractSchemaItem> GetFullPath(Path path)
        {
            Entity startingEntity = null;
            Entity currentEntity = null;
            List<AbstractSchemaItem> result = new List<AbstractSchemaItem>();
            for (int i = 0; i < path.Count(); i++)
            {
                if (0==i) 
                {
                    /* First item expected to be an entity */
                    startingEntity = this.GetEntity(path[i]);
                    currentEntity = startingEntity;
                    result.Add(startingEntity);
                }
                else if (i < path.Count() - 1)
                {
                    var prop = currentEntity.Properties.First(x => x.PropertyName == path[i]);
                    result.Add(prop);
                    if (typeof(NavigationProperty)== prop.GetType()) 
                    {
                        currentEntity = GetEntity((prop as NavigationProperty).TargetEntity);
                    } 
                    else 
                    {
                        throw new ArgumentException($"Invalid path specified. {path[i]} expected to be a navigation property.");
                    }
                } 
                else 
                {
                    var prop = currentEntity.Properties.First(x => x.PropertyName == path[i]);
                    result.Add(prop);
                }
            }
            return result;
        }


        /// <summary>
        /// Return the Entity by the identifier
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Return a navigation property if found else null
        /// </summary>
        /// <returns></returns>
        public NavigationProperty GetNavigationProperty(String EntityDotProperty)
        {
            if (EntityDotProperty.IndexOf(".") == -1) {
                throw new ArgumentException("Expected string like Enitity.PropertyName");
            }

            var _splitted = EntityDotProperty.Split(".");
            return GetNavigationProperty(_splitted[0], _splitted[1]);
        }

        /// <summary>
        /// Return a navigation property if found else null
        /// </summary>
        /// <returns></returns>
        public NavigationProperty GetNavigationProperty(string Entity, string Property)
        {
            if (p.ContainsKey(Entity))
            {
                var ownerEntity = p[Entity];
                if (ownerEntity.GetType() == typeof(Entity))
                {   
                    var prop = (ownerEntity as Entity).Properties
                        .FirstOrDefault(x => typeof(NavigationProperty)==x.GetType() && (x as NavigationProperty).PropertyName == Property);

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
