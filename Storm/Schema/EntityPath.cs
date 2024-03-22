using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Storm.Schema
{

    public class Path : IEnumerable<String>
    {
        protected String[] tokens = new string[] { };
        public virtual String Stringify() => String.Join(".", tokens);

        public string this[int index]  => tokens[index];

        public Path(string pathWithRoot)
        {
            if (String.IsNullOrEmpty(pathWithRoot))
                throw new ArgumentNullException("pathWithRoot");
            tokens = pathWithRoot.Split(".");
        }

        public Path(string root, string path)
        {
            if (String.IsNullOrEmpty(root))
                throw new ArgumentNullException("root");
            if (String.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            tokens = (new string[] {root}).Concat(path.Split(".")).ToArray();
        }

        public Path(IEnumerable<string> pathWithRoot)
        {
            if (pathWithRoot == null)
                throw new ArgumentNullException("pathWithRoot");
            if (pathWithRoot.Any(x => string.IsNullOrEmpty(x)))
                throw new ArgumentNullException("pathWithRoot", "At least one element is null or empty.");
            tokens = pathWithRoot.ToArray();
        }

        public Path(string root, IEnumerable<string> path)
        {
            if (String.IsNullOrEmpty(root))
                throw new ArgumentNullException("root");
            if (path == null)
                throw new ArgumentNullException("path");
            if (path.Any(x => string.IsNullOrEmpty(x)))
                throw new ArgumentNullException("path", "At least one element is null or empty.");
            tokens =  (new string[] {root}).Concat(path).ToArray();
        }

        public static bool operator ==(Path x, Path y)
        {
            return x.tokens.Length == x.tokens.Length && x.Stringify() == y.Stringify();
        }

        public static bool operator !=(Path x, Path y)
        {
            return !(x == y);
        }

        public override bool Equals(object obj)
        {
            return this == (Path)obj;
        }

        public override int GetHashCode()
        {
            return this.Stringify().GetHashCode();
        }

        public IEnumerator<string> GetEnumerator()
        {
            return tokens.ToList().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return tokens.GetEnumerator();
        }
    
        public static Path operator + (Path p1, Path p2) => new Path(p1.tokens.Concat(p2.tokens).ToArray());
    
    }


    public class EntityPath
    {
        protected String[] tokens = new string[] { };

        public virtual String Path => String.Join(".", tokens);
        public String Root => tokens[0];
        public String Last => tokens.Last();

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

    public class FieldPath : EntityPath
    {
        public String Field => tokens.Last();
        public EntityPath OwnerEntityPath => new EntityPath(this.Root, tokens.Take(tokens.Length - 1));

        public FieldPath(string root, string path, string field) : base(root, path)
        {
            if (!String.IsNullOrEmpty(field))
            {
                tokens = tokens.Concat(new string[] { field }).ToArray();
            }
            else
            {
                throw new ArgumentNullException("field");
            }
        }

        public FieldPath(string root, IEnumerable<string> path, string field) : base(root, path)
        {
            if (!String.IsNullOrEmpty(field))
            {
                tokens = tokens.Concat(new string[] { field }).ToArray();
            }
            else
            {
                throw new ArgumentNullException("field");
            }
        }

        public static bool operator ==(FieldPath x, FieldPath y) => (EntityPath)x == (EntityPath)y;

        public static bool operator !=(FieldPath x, FieldPath y) => (EntityPath)x != (EntityPath)y;

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
