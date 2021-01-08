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
                bigTreeNode.Nodes.Add(DFS(curNode));
            }
            return bigTreeNode;
        }
        public TreeNode getTreeView()
        {
            TreeNode treeNode = DFS(tiny_syntax_analyzer.root);
            return treeNode;
        }

    }
    
}
