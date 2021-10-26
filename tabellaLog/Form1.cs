﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.IO;
using System.Data.SqlClient;

namespace tabellaLog
{
    public partial class Form1 : Form
    {
        SqlConnection Conn = new SqlConnection("Data Source=DESKTOP-MVIRBBM;Initial Catalog=compuViola;Integrated Security=True");
        SqlCommand cmd;
        Account account;

        public Form1()
        {
            InitializeComponent();
        }

       

       

        private void Form1_Load(object sender, EventArgs e)
        {
            //verifica se esistono le tabelle e la crea

            CreaTabelle();

        }

        void CreaTabelle()
        {
            string query = "IF OBJECT_ID('dbo.Log') IS NOT NULL begin PRINT 'tabella log esiste' end else begin CREATE TABLE Log (ID int IDENTITY(1, 1) PRIMARY KEY,Utente nvarchar(50),Data DateTime,LogMsg nvarchar(50),Dato int,); end IF OBJECT_ID('dbo.Account') IS NOT NULL begin PRINT 'tabella account esiste' end else begin Create Table Account(Username nvarchar(100) PRIMARY KEY, Password nvarchar(100) NOT NULL) end IF OBJECT_ID('dbo.TabellaProdotti') IS NOT NULL begin PRINT 'tabella prodotti esiste' end else begin Create Table TabellaProdotti(Prodotto nvarchar(50) PRIMARY KEY, Quantità int NOT NULL, Prezzo float NOT NULL) end; ";
            cmd = new SqlCommand(query, Conn);
            try
            {
                Conn.Open();
                cmd.ExecuteNonQuery();
                MessageBox.Show("creazione tabelle riuscita");

            }
            catch (SqlException error)
            {
                MessageBox.Show($"si è verificato un errore durante la creazione delle tabelle:\n{error.Message}");

            }
            cmd.Dispose();
            Conn.Close();
        }
      

       

       

        

       

        /// <summary>
        /// inserimento nella tabella di log dell'azione effettuata
        /// </summary>
        /// <param name="azione"></param>
        /// <param name="valore"></param>
        void InserimentoDB(string azione, string valore)
        {
            
            string query = $"INSERT INTO Log (Utente,Data,LogMsg,Dato) VALUES('{account.Username}','{DateTime.Now}','{azione}','{valore}');";
            cmd = new SqlCommand(query, Conn);

            try
            {
                Conn.Open();
                cmd.ExecuteNonQuery();
                

            }
            catch (SqlException error)
            {
                MessageBox.Show($"errore durante l'aggiunta nella tabella log:\n{error.Message}");

            }
            cmd.Dispose();
            Conn.Close();
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text)| string.IsNullOrWhiteSpace(textBox2.Text))
            {
                MessageBox.Show("I campi non possono essere vuoti");
                return;
            }
            string query = $"if(not EXISTS(Select * from dbo.Account where Username='{textBox1.Text}'))  begin insert into Account(Username,Password) Values('{textBox1.Text}','{textBox2.Text}')end else	begin print('username già esistente') end;";
            cmd = new SqlCommand(query, Conn);
            try
            {
                Conn.Open();

                if (cmd.ExecuteNonQuery() != -1)
                {
                    Account reg = new Account();
                    reg.Username = textBox1.Text;
                    reg.Password = textBox2.Text;

                    File.WriteAllText($"Account\\{reg.Username}.json", JsonConvert.SerializeObject(reg));

                    MessageBox.Show("registrazione avvenuta con successo");
                }
                else
                {
                    MessageBox.Show("username non valido");
                }
               
                
            }
            catch (SqlException err)
            {
                MessageBox.Show(err.Message);
                
            }
            cmd.Dispose();
            Conn.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text)|string.IsNullOrWhiteSpace(textBox2.Text))
            {
                MessageBox.Show("i campi non possono essere vuoti");
                return;
            }
            string query = $"SELECT * FROM dbo.Account WHERE Username='{textBox1.Text}' AND Password='{textBox2.Text}';";
            cmd = new SqlCommand(query, Conn);
            SqlDataReader reader;
            try
            {
                Conn.Open();
                reader= cmd.ExecuteReader();
            }
            catch(SqlException err)
            {
                MessageBox.Show(err.Message);
                if (Conn.State == ConnectionState.Open)
                    Conn.Close();
                return;
            }
            
            
            if (reader.HasRows)
            {
                
                account = new Account();
                reader.Read();
                account.Username = reader["Username"].ToString();
                MessageBox.Show("accesso eseguito correttamente");
                button1.Enabled = false;
                button2.Enabled = false;
                button3.Enabled = true;
                textBox1.Enabled = false;
                textBox2.Enabled = false;
                reader.Close();
                cmd.Dispose();
                Conn.Close();
                InserimentoDB("LOG IN", "");
            }
            else
            {
                MessageBox.Show("credenziali non valide");
                Conn.Close();
            }
        }

        private void tabelleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreaTabelle();
        }

        private void dbToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SqlConnection creaDb = new SqlConnection("Data Source=DESKTOP-MVIRBBM;Initial Catalog=master;Integrated Security=True");
            string query = "CREATE DATABASE compuViola";
            cmd = new SqlCommand(query, creaDb);
            try
            {
                creaDb.Open();
                cmd.ExecuteNonQuery();
                MessageBox.Show("creazione db riuscita");

            }
            catch (SqlException error)
            {
                MessageBox.Show($"si è verificato un errore durante la creazione del db:\n{error.Message}");

            }
            cmd.Dispose();
            creaDb.Close();
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            InserimentoDB("LOG OUT", "");
            account = null;
            button3.Enabled = false;
            button2.Enabled = true;
            button1.Enabled = true;
            textBox1.Enabled = true;
            textBox2.Enabled = true;
            MessageBox.Show("log out eseguito correttamente");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (account==null)
            {
                MessageBox.Show("devi effettuare prima l'accesso");
                return;
            }
            try
            {
                Conn.Open();
                insert form = new insert(Conn,account);
                form.ShowDialog();
                Conn.Close();
                
            }
            catch (SqlException err)
            {
                MessageBox.Show(err.Message);
                Conn.Close();

            }
            
        }

        private void creaToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void tabelleToolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private void dbToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SqlConnection creaDb = new SqlConnection("Data Source=DESKTOP-MVIRBBM;Initial Catalog=master;Integrated Security=True");
            string query = "DROP DATABASE compuViola";
            cmd = new SqlCommand(query, creaDb);
            try
            {
                creaDb.Open();
                cmd.ExecuteNonQuery();
                MessageBox.Show("eliminazione db riuscita");

            }
            catch (SqlException error)
            {
                MessageBox.Show($"si è verificato un errore durante la cancellazione del db:\n{error.Message}");

            }
            cmd.Dispose();
            creaDb.Close();
        }
    }

    //classe utilizzata per contenere i dati dell'utente che ha fatto l'accesso
    public class Account
    {
        public string Username = "";
        public string Password = "";
    }
    public class ListaProdotti
    {
        public List<ProdottoSingolo> prodotti;
    }
    public class ProdottoSingolo
    {
        public string Prodotto="";
        public int Quantità = 0;
        public float Prezzo = 0;
    }
}
