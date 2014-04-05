using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace pt_2Attemt
{
    public partial class Edit : Form
    {
        public Edit()
        {
            InitializeComponent();
            this.AcceptButton = button1;
        }

        //Properties to receive data to the Edit form
        public String editFirst
        {
            get { return firstNameTextBox.Text; }
            set { this.firstNameTextBox.Text = value; }
        }

        public String editLast
        {
            get { return lastNameTextBox.Text; }
            set { this.lastNameTextBox.Text = value; }
        }

        public String editMarks
        {
            get { return marksTextBox.Text; }
            set { this.marksTextBox.Text = value; }
        }

        //Method for data validation
        public bool validateData()
        {
            Regex nameRegex = new Regex(@"^[A-z][a-z]+$");
            Regex marksRegex = new Regex(@"^[0-9]+$");
            if (!nameRegex.IsMatch(firstNameTextBox.Text))
            {
                MessageBox.Show("The first name doesn't look nice...");
                return false;
            }
            else if (!nameRegex.IsMatch(lastNameTextBox.Text))
            {
                MessageBox.Show("The last name doesn't look nice...");
                return false;
            }
            else if (!marksRegex.IsMatch(marksTextBox.Text))
            {
                MessageBox.Show("The marks doesn't look like marks...");
                return false;
            }
            else
                return true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(firstNameTextBox.Text))
                MessageBox.Show("You forgot the First Name.", "Invalid Data!",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            else if (String.IsNullOrEmpty(lastNameTextBox.Text))
                MessageBox.Show("You forgot the Last Name.", "Invalid Data!",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            else if (String.IsNullOrEmpty(marksTextBox.Text))
                MessageBox.Show("You forgot the marks.", "Invalid Data!",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            else if(validateData())
                this.DialogResult = DialogResult.OK;
        }
    }
}
