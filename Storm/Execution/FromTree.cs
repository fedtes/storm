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
                if (this.nodes == null)
                {

                    this.nodes = new Dictionary<EntityPath, FromNode>() { { value.FullPath, value } };
                }
                _root = value;
            }
        }

        internal Dictionary<EntityPath, FromNode> nodes;

        internal EntityPath EnsurePath(String requestPath)
        {
            return new EntityPath(root.FullPath.Path, requestPath);
        }

        internal EntityPath EnsurePath(string[] requestPath)
        {
            return new EntityPath(root.FullPath.Path, String.Join(".", requestPath));
        }

        internal FromNode Resolve(string path)
        {
            return Resolve(root, 0, EnsurePath(path));
        }

        internal FromNode Resolve(IEnumerable<string> path)
        {
            return Resolve(root, 0, EnsurePath(path.ToArray()));
        }

        internal FromNode Resolve(FromNode subTree, int idx, EntityPath path)
        {
            var head = path.Head(idx);
            var current = path.ElementAt(idx);
            var tail = path.Skip(idx + 1);
            var partialPath = head.Append(current);

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
        public EntityPath FullPath;
        public SchemaEdge Edge;
        public SchemaNode Entity;
        public String Alias;
        public List<FromNode> children;
    }
}
