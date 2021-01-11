using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
public enum Token_Class
{
    
}
namespace TinyCompiler
{
    public partial class Form1 : Form
    {
        
        public Form1()
        {
            InitializeComponent();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox2.Clear();
            textBox3.Clear();
            treeView1.Nodes.Clear();
            moderator mdr = new moderator(textBox1.Text.ToString());
            dataGridView1.DataSource = mdr.getDataTable();
            textBox2.Text = String.Join(Environment.NewLine, mdr.getScannerErrorsList());
            textBox3.Text = String.Join(Environment.NewLine, mdr.getParserErrorsList());
            TreeNode tree= mdr.getTreeView();
            treeView1.Nodes.Add(tree);
        }
        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            treeView1.Nodes.Clear();
            DataTable empytyTable = new DataTable();
            dataGridView1.DataSource = empytyTable;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            // Test Here please
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            if(treeView1!=null)
            {
                treeView1.ExpandAll();
            }
        }
    }
}
