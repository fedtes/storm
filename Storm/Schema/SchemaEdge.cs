using System;
using Storm.Filters;

namespace Storm.Schema
{
    public class SchemaEdge : SchemaItem
    {
        public string SourceID { get; internal set; }
        public string TargetID { get; internal set; }
        public Tuple<string,string> On { get; internal set; }
        public Filter OnExpression { get; internal set; }

        internal override SchemaItem Clone()
        {

            return new SchemaEdge()
            {
                ID = ID,
                SourceID = SourceID,
                TargetID = TargetID,
                On = On,
                OnExpression = OnExpression
            };
        }
    }

}
