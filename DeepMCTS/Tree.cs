using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepMCTS
{
    public class Node
    {
        public Node parent = null;
        public List<Node> children = new List<Node>();
        public int value = 0;
        public int visits = 0;
        public byte action = 0;
        public byte PlayerTookAction = 0;
        public int depth = 0;
        public bool ignore = false;

        public double ucb1 { get; set; } = 0d;

        //Game specific
        public byte[] state = new byte[9];

        public Node(Node parent, byte action, int PlayerTookAction, int depth)
        {
            this.parent = parent;
            this.action = action;
            this.PlayerTookAction = (byte)PlayerTookAction;
            this.depth = depth;
        }

        public override string ToString()
        {
            if (parent == null)
                return "Root Node";

            return $"p{PlayerTookAction}'s move: {action} Vi/Va: {visits}/{value} ucb1: {ucb1} depth: {depth}";
        }
 
        public string DrawTree()
        {
            return DrawTree("", true);
        }

        private string DrawTree(string indent, bool last)
        {
            var sb = new StringBuilder();

            sb.Append(indent);
            if (last)
            {
                sb.Append("\\-");
                indent += "  ";
            }
            else
            {
                sb.Append("|-");
                indent += "| ";
            }
            sb.AppendLine(ToString());

            for (int i = 0; i < children.Count; i++)
            {
                var childText = children[i].DrawTree(indent, i == children.Count - 1);
                sb.Append(childText);
            }

            return sb.ToString();
        }
    }
}
