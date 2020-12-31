using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyCompiler
{
    public class Node
    {
        public string NodeName;
        public List<Node> childrenNodes;
        public Node(string name)
        {
            NodeName = name;
            childrenNodes = new List<Node>();
        }
    }
    class tiny_syntax_analyzer
    {
        int tokensPointer;
        List<TinyToken> myTokens;
        Node root;
        public tiny_syntax_analyzer(List<TinyToken>inputTokens)
        {
            tokensPointer = 0;
            root.NodeName = "Program";
            myTokens = inputTokens;
        }
        public Node Parser()
        {

            return root;
        }
    }
}
