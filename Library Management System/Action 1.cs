using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.Data.SQLite;
using System.IO;
using Microsoft.VisualBasic;

namespace Library_Management_System
{
    public partial class Action_1 : Form
    {
        private SerialPort serial = new SerialPort();
        private SQLiteConnection db;
        public Action_1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            groupBox1.Visible = false;
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }


        private void rfidProcess(string rfid_id)
        {
            Boolean found = false;
            using (SQLiteCommand fmd = db.CreateCommand())
            {
                fmd.CommandText = @"SELECT id FROM book WHERE id = @id;";
                fmd.CommandType = CommandType.Text;
                fmd.Parameters.AddWithValue("@id", rfid_id);
                SQLiteDataReader r = fmd.ExecuteReader();
                while (r.Read())
                {
                    found = true;
                }
            }


            if (!found)
            {
                // first case, book was not found, we need to add a new book with rfid_id ID 
                AddBook memes = new AddBook();
                memes.ShowDialog();
                if (memes.everythingOk)
                {
                    {
                        string sql = @"INSERT INTO book (id,title,subject, year, publisher) VALUES (@rfid,@title,@subject,@year,@publisher);";
                        SQLiteCommand cmd = new
                            SQLiteCommand(sql, db);

                        cmd.Parameters.AddWithValue("@rfid", rfid_id);
                        cmd.Parameters.AddWithValue("@title", memes.title);
                        cmd.Parameters.AddWithValue("@subject", memes.subject);
                        cmd.Parameters.AddWithValue("@year", memes.year);
                        cmd.Parameters.AddWithValue("@publisher", memes.publisher);
                        cmd.ExecuteNonQuery();
                        // book not issued before
                    }
                }
                listBooks();
            }
            else
            {
                int issue_id = -1;
                using (SQLiteCommand fmd = db.CreateCommand())
                {
                    fmd.CommandText = @"SELECT id_loan FROM loaned_books WHERE id_book = @id AND active=1;";
                    fmd.CommandType = CommandType.Text;
                    fmd.Parameters.AddWithValue("@id", rfid_id);
                    SQLiteDataReader r = fmd.ExecuteReader();
                    while (r.Read())
                    {
                        issue_id = Convert.ToInt32(r["id_loan"]);
                    }
                }

                if (issue_id == -1)
                {
                    string sql = @"INSERT INTO loaned_books (id_book, date, active) VALUES (@id, date('now'), 1);";
                    SQLiteCommand cmd = new
                        SQLiteCommand(sql, db);

                    cmd.Parameters.AddWithValue("@id", rfid_id);
                    cmd.ExecuteNonQuery();
                    // book not issued before
                }
                else
                {
                    // yup, book has been issued, we're de-issuin' it, whatever that is supposed to mean
                    string sql = @"UPDATE loaned_books SET active=0 WHERE id_loan = @id";
                    SQLiteCommand cmd = new
                        SQLiteCommand(sql, db);

                    cmd.Parameters.AddWithValue("@id", issue_id);
                    cmd.ExecuteNonQuery();
                }
                listIssued();
            }
        }
    private void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            //Console.WriteLine(serial.ReadExisting());
            string rfid_id = serial.ReadLine();
            rfidProcess(rfid_id);
        }


        private void button1_Click_2(object sender, EventArgs e)
        {
            if (serial.IsOpen)
            {
                serial.Close();
           
            }
            else
            {
                serial = new SerialPort("COM3", 57600);
                serial.DataReceived += new SerialDataReceivedEventHandler(port_DataReceived);
                serial.Open(); 
            }
        }

        private void Action_1_Load(object sender, EventArgs e)
        {
            if (!File.Exists("library.db"))
            {
                SQLiteConnection.CreateFile("library.db");

            }
            db = new SQLiteConnection("Data Source=library.db;Version=3");
            db.Open();
            string sql = "CREATE TABLE IF NOT EXISTS book(id text primary key," +
                "title text not null," +
                " subject text, year text," +
                " publisher text);";
            SQLiteCommand cmd = new
                SQLiteCommand(sql, db);
            cmd.ExecuteNonQuery();
            string sql2 = "CREATE TABLE IF NOT EXISTS loaned_books(id_loan integer primary key," +
                "id_book text not null," +
                "date text," +
                "active integer not null," +
                "foreign key(id_book)references book(id));";
            SQLiteCommand cmd2 = new
                SQLiteCommand(sql2, db);
            cmd2.ExecuteNonQuery();
            listBooks();
        }
        private void listBooks()
        {
            UpdateDataGrid("SELECT * FROM book;");
        }
        private void UpdateDataGrid(string sql)
        {
            DataSet dataSet = new DataSet();
            SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(sql, db);
            dataAdapter.Fill(dataSet);

            Datagrid.DataSource = dataSet.Tables[0].DefaultView;
        }

        private void button1_Click_3(object sender, EventArgs e)
        {
            listBooks();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            listIssued();
        }

        private void listIssued()
        {
            UpdateDataGrid("SELECT * FROM book JOIN loaned_books ON loaned_books.id_book = book.id WHERE loaned_books.active = 1;");
        }
    }
}
