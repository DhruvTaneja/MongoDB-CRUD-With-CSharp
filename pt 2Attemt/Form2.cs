using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace pt_2Attemt
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
            AcceptButton = button1;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "connect")
                this.Close();
            else
                MessageBox.Show("Incorrect password!");
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
