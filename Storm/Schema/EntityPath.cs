using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Storm.Schema
{
    public class EntityPath
    {
        String[] tokens = new string[] { };

        public String Path => String.Join(".", tokens);
        public String Root => tokens[0];

        public EntityPath(string root, string path)
        {
            if (String.IsNullOrEmpty(path))
            {
                tokens = new string[] { root };
            }
            else
            {
                var _path = path.Split('.');
                _path = root == _path[0] ? _path : (new string[] { root }).Concat(_path).ToArray();
                tokens = _path.Where(t => !String.IsNullOrEmpty(t)).Select(t => t.Trim('.')).ToArray();
            }
        }

        public EntityPath(string root, IEnumerable<string> path)
        {
            if (path.Any())
            {
                var x = path.ToArray();
                x = root == x[0] ? x : (new string[] { root }).Concat(x).ToArray();
                tokens = x.Where(t => !String.IsNullOrEmpty(t)).Select(t => t.Trim('.')).ToArray();
            }
            else
            {
                tokens = new string[] { root };
            }
        }

        public static bool operator ==(EntityPath x, EntityPath y)
        {
            return x.tokens.Length == x.tokens.Length
                && x.Path == y.Path;
        }

        public static bool operator !=(EntityPath x, EntityPath y)
        {
            return !(x == y);
        }

        public override bool Equals(object obj)
        {
            return this == (EntityPath)obj;
        }

        public override int GetHashCode()
        {
            return this.Path.GetHashCode();
        }

        public override string ToString()
        {
            return this.Path;
        }

        internal EntityPath Head(int idx)
        {
            return new EntityPath(Root, tokens.Take(idx));
        }

        internal EntityPath Append(string s)
        {
            if (this.Root == s)
            {
                return new EntityPath(Root, (string)null);
            }
            else
            {
                return new EntityPath(Root, tokens.Concat(new string[] { s }));
            }
        }

        internal string ElementAt(int idx)
        {
            return tokens[idx];
        }

        internal IEnumerable<string> Skip(int v)
        {
            return tokens.Skip(v);
        }


        internal int Count()
        {
            return tokens.Length;
        }
    }
}
