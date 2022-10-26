using System;

namespace Storm.Schema
{
    public abstract class SchemaItem
    {
        public String ID { get; internal set; }

        internal abstract SchemaItem Clone();
    }

}
