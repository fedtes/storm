using System;
using System.Collections.Generic;
using System.Text;

namespace Storm.Execution
{
    class Column
    {
        public String Alias;
        public String Name;

        public Column() { }
        public Column(string rawname)
        {
            var a = rawname.Split('$');
            this.Alias = a[0];
            this.Name = a[1];
        }

        public override string ToString()
        {
            return $"{Alias}.{Name}";
        }
    }
}
