using Storm.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Storm.Execution
{
    public class FromTree
    {
        internal SchemaNavigator navigator;

        private FromNode _root;
        internal FromNode root 
        {
            get => _root;
            set
            {
                if (this.nodes == null) this.nodes = new Dictionary<string, FromNode>() { { value.FullPath, value } };
                _root = value;
            }
        }

        internal Dictionary<string, FromNode> nodes;

        internal IEnumerable<string> EnsurePath(String requestPath)
        {
            var path = requestPath.Split('.');
            return EnsurePath(path);
        }

        internal IEnumerable<string> EnsurePath(string[] requestPath)
        {
            return requestPath[0] == root.Entity.ID ? requestPath : (new string[] { root.Entity.ID }).Concat(requestPath).ToArray();
        }

        internal FromNode Resolve(string path)
        {
            return Resolve(root, 0, EnsurePath(path));
        }

        internal FromNode Resolve(IEnumerable<string> path)
        {
            return Resolve(root, 0, EnsurePath(path.ToArray()));
        }

        internal FromNode Resolve(FromNode subTree, int idx, IEnumerable<string> path)
        {
            var head = path.Take(idx);
            var current = path.ElementAt(idx);
            var tail = path.Skip(idx + 1);
            var partialPath = String.Join(".", head.Concat(new string[] { current }));

            if (!nodes.ContainsKey(partialPath))
            {
                var _edge = navigator.GetEdge($"{subTree.Entity.ID}.{current}");
                var node = new FromNode()
                {
                    Alias = $"A{nodes.Count}",
                    children = new List<FromNode>(),
                    Edge = _edge,
                    Entity = navigator.GetEntity(_edge.TargetID),
                    FullPath = partialPath
                };
                nodes.Add(partialPath, node);
                subTree.children.Add(node);
            }

            if (tail.Count() == 0)
                return nodes[partialPath];
            else
                return Resolve(nodes[partialPath], idx + 1, path);
        }
    }

    public class FromNode
    {
        public String FullPath;
        public SchemaEdge Edge;
        public SchemaNode Entity;
        public String Alias;
        public List<FromNode> children;
    }
}
