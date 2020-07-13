using System;
using System.Collections.Generic;
using System.Text;

namespace Storm.Schema
{
    public class SchemaNavigator
    {
        private SchemaInstance p;

        public SchemaNavigator(SchemaInstance p)
        {
            this.p = p;
        }

        public SchemaItem Get(String identifier)
        {
            if (p.ContainsKey(identifier))
                return p[identifier];
            else
                return null;
        }

        public SchemaNode GetEntity(String identifier)
        {
            if (p.ContainsKey(identifier))
            {
                var x = p[identifier];
                if (x.GetType() == typeof(SchemaNode))
                {
                    return (SchemaNode)x;
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

        public SchemaEdge GetEdge(String identifier)
        {
            if (p.ContainsKey(identifier))
            {
                var x = p[identifier];
                if (x.GetType() == typeof(SchemaEdge))
                {
                    return (SchemaEdge)x;
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
