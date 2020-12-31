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
            moderator mdr = new moderator(textBox1.Text.ToString());
            dataGridView1.DataSource = mdr.getDataTable();
            textBox2.Text = String.Join(Environment.NewLine, mdr.getErrorsList());
        }
        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            textBox2.Clear();
            DataTable empytyTable = new DataTable();
            dataGridView1.DataSource = empytyTable;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
