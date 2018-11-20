using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Library_Management_System
{
    public partial class AddBook : Form
    {
        public bool everythingOk { get; internal set; }
        public string title { get; internal set; }
        public string subject { get; internal set; }
        public string year { get; internal set; }
        public string publisher { get; internal set; }

        public AddBook()
        {
            InitializeComponent();
            everythingOk = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            everythingOk = true;
            this.Close();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null)
            {
                title = textBox.Text;
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null)
            {
                subject = textBox.Text;
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null)
            {
                year = textBox.Text;
            }
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null)
            {
                publisher = textBox.Text;
            }
        }
    }
}
