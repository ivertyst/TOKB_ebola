using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;

namespace Bi_31_windows
{
    public partial class AuthWin : Form
    {
        int counter = 0;
        public AuthWin()
        {
            InitializeComponent();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\admin\Desktop\Bi-31 windows\Bi-31 windows\Database1.mdf;Integrated Security=True;MultipleActiveResultSets=True";
            SqlConnection sqlConnection;
            sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();
            SqlDataReader sqlReader = null;
            SqlCommand command = new SqlCommand("SELECT * FROM [Users]", sqlConnection);
            sqlReader = command.ExecuteReader();
            bool flag = false;
            int block = 0;
            while (sqlReader.Read())
            {
                string i = Convert.ToString(sqlReader["Login"]).Replace(" ", "");
                string j = Convert.ToString(sqlReader["Password"]).Replace(" ", "");
                block = Convert.ToInt32(sqlReader["Suspended"]);
                if (i == textBox1.Text && (j == textBox2.Text | j == Program.Encode(textBox2.Text, Program.olkey)))
                {
                    if (block == 0)
                    {
                        this.Hide();
                        string[] data = new string[3];
                        data[0] = i;
                        data[1] = j;
                        data[2] = Convert.ToString(sqlReader["Admin"]).Replace(" ", "");
                        if (sqlReader != null)
                            sqlReader.Close();
                        all newForm = new all(data);
                        newForm.ShowDialog();
                        this.Show();
                        flag = true;
                        break;
                    }
                    else
                    {
                        MessageBox.Show("Вы были поражены в правах на вход");
                        if (sqlReader != null)
                            sqlReader.Close();
                        return;
                    }
                }
            }
            if (flag == false)
            {
                MessageBox.Show("Неверный пароль");
                counter++;
            }
            if (sqlReader != null)
                sqlReader.Close();
            if (counter == 3)
            {
                MessageBox.Show("Истекло количество попыток войти");
                Application.Exit();
            }
        }

        private void Cancel_Click(object sender, EventArgs e)
        {

            Application.Exit();
        }

        private void AuthWin_Load(object sender, EventArgs e)
        {
            try
            {
                string[] key = fleshd.ReadString(@"E:").Replace("\0", "").Split(new char[] { ' ' });
                for (int i = 0; i < 16; i++)
                {
                    Program.olkey[i] = Convert.ToInt32(key[i]);
                }
            }
            catch
            {
                MessageBox.Show("Вставьте USB-ключ");
            }
        }
    }
}
