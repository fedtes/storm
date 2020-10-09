using Storm.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Storm.Execution.Results
{
    internal class Range
    {
        private readonly Point ul;
        private readonly Point br;
        private readonly SchemaNode entity;
        private readonly StormResult parent;

        public Range(Point ul, Point br, SchemaNode entity, StormResult parent)
        {
            this.ul = ul;
            this.br = br;
            this.entity = entity;
            this.parent = parent;
        }

        public IEnumerable<StormResultRow> Rows => parent.data.Take(br.y).Skip(ul.y).Select((x, i) => new StormResultRow(parent, i, ul.x, br.x));
    }

    public class Point
    {
        public int x;
        public int y;
    }
}
