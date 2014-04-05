using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MongoDB.Driver;
using MongoDB.Bson;

using Query = MongoDB.Driver.Builders.Query;

namespace pt_2Attemt
{
    public partial class Find : Form
    {
        public MongoClient client;
        public MongoServer server;
        MongoCollection collection;

        public Find()
        {
            InitializeComponent();
            dataGridView1.Visible = false;
            AcceptButton = button1;
        }

        public void delete()
        {
            var cursor = collection.FindAllAs<Form1.Doc>();

            if (dataGridView1.SelectedRows.Count == 0)
                MessageBox.Show("Select atleast one row to be deleted");
            else
            {
                foreach (DataGridViewRow item in this.dataGridView1.SelectedRows)
                {
                    string first = item.Cells[0].Value.ToString();
                    foreach (var cursoritem in cursor)
                    {
                        if ((item.Cells[0].Value.ToString() == cursoritem.first_name)
                            && (item.Cells[1].Value.ToString() == cursoritem.last_name))
                            collection.Remove(Query.And(Query.EQ("first_name", cursoritem.first_name),
                                Query.EQ("last_name", cursoritem.last_name)));
                    }

                    dataGridView1.Rows.Remove(item);
                }
            }
        }

        public void latchToDB()
        {
            var connectionString = "mongodb://localhost";
            client = new MongoClient(connectionString);
            server = client.GetServer();

            var db = server.GetDatabase("students");
            collection = db.GetCollection("boys");
        }

        public bool checkName(string name)
        {
            latchToDB();
            var findQuery = Query.EQ("first_name", textBox1.Text);
            var cursor = collection.FindAs<Form1.Doc>(findQuery);
            if (cursor.Count() == 0)
                return false;
            else
            {
                foreach (var item in cursor)
                {
                    string id = item.id.ToString();
                    string first = item.first_name;
                    string last = item.last_name;
                    string marks = item.marks.ToString();
                    string[] row = new string[] { first, last, marks };
                    dataGridView1.Rows.Add(row);
                }
                return true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(textBox1.Text))
                MessageBox.Show("Enter a name to be searched. Come on!");
            else
            {
                if (!(dataGridView1.Visible = checkName(textBox1.Text)))
                    MessageBox.Show("There is no document matching your search");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            delete();
        }

        private void Find_FormClosing(object sender, FormClosingEventArgs e)
        {

        }
    }
}
