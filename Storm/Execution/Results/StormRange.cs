using Storm.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Storm.Execution.Results
{
    internal class StormRange
    {
        private readonly Point ul;
        private readonly Point br;
        private readonly StormDataSet parent;

        public StormRange(Point ul, Point br, StormDataSet parent)
        {
            this.ul = ul;
            this.br = br;
            this.parent = parent;
        }

        public IEnumerable<StormRow> Rows => parent.data.Take(br.y).Skip(ul.y).Select((x, i) => new StormRow(parent, i, ul.x, br.x));
    }

    public class Point
    {
        public int x;
        public int y;
    }
}
