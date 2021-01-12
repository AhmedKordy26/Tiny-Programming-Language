using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Windows.Forms;
namespace TinyCompiler
{
    class  moderator
    {
        public moderator(string sourceCode)
        {
            tiny_scanner.errorsList.Clear();
            tiny_scanner.splittedStringsList.Clear();
            tiny_scanner.tinyTokensList.Clear();
            tiny_scanner.newSplitter(sourceCode);
            tiny_scanner.findTokensAndErrors();
            tiny_syntax_analyzer.Initialize(tiny_scanner.tinyTokensList);

        }
        public DataTable getDataTable()
        {
            DataTable tokensTable = new DataTable();
            tokensTable.Columns.Add("Lexeme", typeof(string));
            tokensTable.Columns.Add("Token Classes", typeof(string));
            foreach (KeyValuePair<string, TinyToken> st in tiny_scanner.tinyTokensList)
            {
                //Console.WriteLine(st.Key + "  " + st.Value.ToString());
                tokensTable.Rows.Add(st.Key, st.Value.ToString());
            }
            return tokensTable;
        }
        public List<string>getScannerErrorsList()
        {
            return tiny_scanner.errorsList;
        }
        public List<string> getParserErrorsList()
        {
            return tiny_syntax_analyzer.root.nodeErrors;
        }
        public TreeNode DFS(Node node)
        {
            if (node.childrenNodes.Count == 0) return new TreeNode(node.NodeName);
            TreeNode bigTreeNode = new TreeNode(node.NodeName);
            foreach (Node curNode in node.childrenNodes)
            {
                TreeNode tmpTree = DFS(curNode);
                if(tmpTree!=null &&curNode.NodeName!="Epsilon")
                    bigTreeNode.Nodes.Add(tmpTree);
            }
            if (bigTreeNode.Nodes.Count == 0) return null;
            return bigTreeNode;
        }
        public TreeNode getTreeView()
        {
            TreeNode treeNode =DFS(tiny_syntax_analyzer.root);
            /*Queue<Node> nodesQ=new Queue<Node>();
            nodesQ.Enqueue(tiny_syntax_analyzer.root);
            Dictionary<string, string> dict = new Dictionary<string, string>() ;
            while(nodesQ.Count>0)
            {
                Node tmpNode = nodesQ.Dequeue();
                foreach (Node curNode in tmpNode.childrenNodes)
                {
                    nodesQ.Enqueue(curNode);
                    dict[curNode.NodeName] = tmpNode.NodeName;
                }
            }*/
            return treeNode;
        }

    }
    
}
