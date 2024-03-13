using Storm.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Storm.Origins
{
    /// <summary>
    /// Describe the from part of the query. It's modelled as a tree of origins where the Root is the first table/view in the from statement and the children are all the tables/views in join.
    /// </summary>
    public class OriginTree
    {
        internal Context ctx;

        private Origin _root;
        internal Origin root 
        {
            get => _root;
            set
            {
                if (this.nodes == null)
                    this.nodes = new Dictionary<EntityPath, Origin>() { { value.FullPath, value } };
                _root = value;
            }
        }

        internal Dictionary<EntityPath, Origin> nodes;
        /// <summary>
        /// Return the orgin corrisponding to the path specified. If the origin is missing, it is added to the tree.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        internal Origin Resolve(string path)
        {
            return Resolve(root, 0, EnsurePath(path));
        }

        /// <summary>
        /// Return the orgin corrisponding to the path specified. If the origin is missing, it is added to the tree.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        internal Origin Resolve(IEnumerable<string> path)
        {
            return Resolve(root, 0, EnsurePath(path.ToArray()));
        }

        /// <summary>
        /// Used internally to recursive resolve the specified path
        /// </summary>
        /// <param name="subTree"></param>
        /// <param name="idx"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        private Origin Resolve(Origin subTree, int idx, EntityPath path)
        {
            var head = path.Head(idx);
            var current = path.ElementAt(idx);
            var tail = path.Skip(idx + 1);
            var partialPath = head.Append(current);

            if (!nodes.ContainsKey(partialPath))
            {
                var _edge = ctx.Navigator.GetNavigationProperty($"{subTree.Entity.Id}.{current}");
                var node = new Origin()
                {
                    Alias = $"A{nodes.Count}",
                    children = new List<Origin>(),
                    Edge = _edge,
                    Entity = ctx.Navigator.GetEntity(_edge.TargetEntity),
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

        private EntityPath EnsurePath(String requestPath)
        {
            return new EntityPath(root.FullPath.Path, requestPath);
        }

        private EntityPath EnsurePath(string[] requestPath)
        {
            return new EntityPath(root.FullPath.Path, requestPath);
        }

    }
}
