using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions; //for regex

using MongoDB.Bson;
using MongoDB.Driver;

using Query = MongoDB.Driver.Builders.Query;
using Updation = MongoDB.Driver.Builders.Update;

namespace pt_2Attemt
{
    public partial class Form1 : Form
    {

        Find findform = new Find();

        //edit variables
        public string editfirst;
        public string editlast;
        public string editmarks;

        //retrieving values from the Add Form
        public string first;
        public string last;
        public string marks;

        //client, collection and server variables for connection
        public MongoClient client;
        public MongoServer server;
        MongoCollection collection;

        //Method to check for duplicacy
        public int checkDuplicate(string fname, string lname)
        {
            var dupQuery = Query.And(Query.EQ("first_name", fname), Query.EQ("last_name", lname));
            var cursor = collection.FindAs<Form1.Doc>(dupQuery);
            if (cursor.Count() != 0)
                return 0;
            else
                return 1;
        }

        //Running batch file to open the mongodb shell
        public void runBatch()
        {
            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            proc.StartInfo.FileName = "C:\\connect.bat";
            proc.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            proc.Start();
        }

        //Method to establish server connection
        public void connectToServer()
        {
            var connectionString = "mongodb://localhost";
            client = new MongoClient(connectionString);
            server = client.GetServer();
            server.Connect();

            var db = server.GetDatabase("students");
            collection = db.GetCollection("boys");
        }
        
        //Method to insert data into the collection
        public void insertToCollection(MongoCollection collection, Doc doc)
        {
            
            collection.Insert(doc);
        }

        //Document type entity matching the database collection
        public class Doc
        {
            public ObjectId id { get; set; }
            public string first_name { get; set; }
            public string last_name { get; set; }
            public double marks { get; set; }
        }

        //Method to populate the data grid view
        //with the documents in the collection
        public void populateGrid()
        {
            MongoCursor<Doc> cursor = collection.FindAllAs<Doc>();

            foreach (var item in cursor)
            {
                string id = item.id.ToString();
                string first = item.first_name;
                string last = item.last_name;
                string marks = item.marks.ToString();
                string[] row = new string[] { first, last, marks };
                dataGridView1.Rows.Add(row);
            }
        }

        //Refreshing the datagrid with updated information
        public void updateData()
        {
            dataGridView1.Rows.Clear();
            populateGrid();
        }

        //Method to delete the selected document(s)
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

        //Method to edit the selected document(s)
        public void EditData()
        {
            if (dataGridView1.SelectedRows.Count == 0)
                MessageBox.Show("Select atleast one Row to be edited.");
            else
            {
                foreach (DataGridViewRow item in this.dataGridView1.SelectedRows)
                {
                    Edit editform = new Edit();
                    editform.editFirst = item.Cells[0].Value.ToString();
                    editform.editLast = item.Cells[1].Value.ToString();
                    editform.editMarks = item.Cells[2].Value.ToString();

                    var query = Query.And(Query.EQ("first_name", editform.editFirst),
                        Query.EQ("last_name", editform.editLast));

                    editform.ShowDialog();
                    if (editform.DialogResult == DialogResult.OK)
                    {
                        //check for duplicacy
                        int check = checkDuplicate(editform.editFirst, editform.editLast);
                        //The or condition checks if the data is sent back without any edition
                        if ((check == 1)||((editform.editFirst == item.Cells[0].Value.ToString())
                            &&(editform.editLast == item.Cells[1].Value.ToString())
                            &&(editform.editMarks == item.Cells[2].Value.ToString())))
                        {
                            collection.Update(query,
                            Updation.Set("first_name", editform.editFirst)
                                .Set("last_name", editform.editLast)
                                .Set("marks", Double.Parse(editform.editMarks)));

                                updateData();
                        }
                        else
                            MessageBox.
                                Show("The document with the first and last names already exists");
                    }
                }
            }
        }

        public void AddData()
        {
            Form3 addForm = new Form3();
            addForm.ShowDialog(this);

            if (addForm.DialogResult == DialogResult.OK)
            {
                //Retrieving data from the child form
                first = addForm.addFirst;
                last = addForm.addLast;
                marks = addForm.addMarks;

                //Check for duplicacy
                int check = checkDuplicate(addForm.addFirst, addForm.addLast);
                if (check == 1)
                {
                    try
                    {
                        Doc doc = new Doc();
                        doc.first_name = first;
                        doc.last_name = last;
                        doc.marks = Double.Parse(marks);

                        insertToCollection(collection, doc);
                    }
                    catch (FormatException e)
                    {
                        MessageBox.Show("Enter valid marks!");
                    }

                    //Updating datagrid
                    updateData();
                }
                else
                {
                    MessageBox.Show("The document with the first and last names already exists");
                }
            }
        }
        
        public Form1()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
            Text = "C# With MongoDB";
            Add.Enabled = false;
            Edit.Enabled = false;
            Del.Enabled = false;
            button1.Enabled = false;
            addToolStripMenuItem.Enabled = false;
            editToolStripMenuItem.Enabled = false;
            deleteToolStripMenuItem.Enabled = false;
            findToolStripMenuItem.Enabled = false;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void connectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            runBatch();
            connectToServer();
            populateGrid();
            Add.Enabled = true;
            Edit.Enabled = true;
            Del.Enabled = true;
            button1.Enabled = true;
            addToolStripMenuItem.Enabled = true;
            editToolStripMenuItem.Enabled = true;
            deleteToolStripMenuItem.Enabled = true;
            findToolStripMenuItem.Enabled = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Form2 form2 = new Form2();
            form2.ShowDialog(this);
        }

        private void Del_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure to delete the document?",
                "Confirm Deletion", MessageBoxButtons.YesNo);
            if(result == DialogResult.Yes)
                delete();
        }

        private void Add_Click(object sender, EventArgs e)
        {
            AddData();
        }

        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddData();
        }

        private void Edit_Click(object sender, EventArgs e)
        {
            EditData();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            delete();
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EditData();
        }

        private void findToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Find findForm = new Find();
            
            findForm.ShowDialog();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            foreach (System.Diagnostics.Process proc in 
                System.Diagnostics.Process.GetProcessesByName("mongod"))
                proc.Kill();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            updateData();
        }

    }
}