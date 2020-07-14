using Storm.Schema;
using System;
using System.Collections.Generic;
using System.Text;

namespace Storm.Execution
{
    public class TableTree
    {
        public String FullPath;
        public SchemaEdge Edge;
        public SchemaNode Entity;
        public String Alias;
        public List<TableTree> children;
    }
}
