using System.Collections.Generic;

namespace Storm.Schema
{
    public class SchemaInstance : Dictionary<string, SchemaItem>
    {
        public SchemaInstance Clone()
        {
            var i = new SchemaInstance();

            foreach (var si in this)
            {
                i.Add(si.Key, si.Value.Clone());
            }
            return i;
        }
    }

}
