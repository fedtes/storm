using Storm.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Storm.Execution
{
    public class Command
    {
        protected String from;
        protected SchemaNavigator navigator;
        protected CommandTree tree;
    }

    public class CommandNode
    {
        public String FullPath;
        public SchemaEdge fromParent;
        public SchemaNode node;
        public String Alias;
        public List<CommandNode> children;
    }

    public class CommandTree
    {
        public CommandNode root;
        public Dictionary<string, CommandNode> nodes;
    }


    public class GetCommand : Command
    {

        public GetCommand()
        {
            tree = new CommandTree()
            {
                root = new CommandNode()
                {
                    Alias = "A0",
                    FullPath = from,
                    fromParent = null,
                    node = navigator.GetEntity(from),
                    children = new List<CommandNode>()
                }
            };
            tree.nodes = new Dictionary<string, CommandNode>() { { tree.root.FullPath, tree.root } };
        }

        private object Add(CommandNode node, string prev, string current, IEnumerable<string> head, IEnumerable<string> tail)
        {
            var e = navigator.GetEdge($"{prev}.{current}");
            node.children.Add(new CommandNode
            {
                Alias = $"A{tree.nodes.Count}",
                children = new List<CommandNode>(),
                fromParent = e,
                node = navigator.GetEntity(e.TargetID)
            });
        }

        public GetCommand With(String requestPath)
        {
            var path = requestPath.Split('.');
            path = path[0] == from ? path : (new string[] { from }).Concat(path).ToArray();
            Add(tree.root, path[0], path[1], path.Skip(2));

            return this;
        }

    }
}
