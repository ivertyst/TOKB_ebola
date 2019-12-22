using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Drawing.Imaging;

namespace Bi_31_windows
{
    public partial class all : Form
    {
        public SqlDataReader sqlReader = null;
        public SqlConnection sqlConnection;
        public all(string[] incData)
        {
            InitializeComponent();
            label4.Text = incData[0];
            label5.Text = incData[1];
            label6.Text = incData[2];
        }

        private void All_Load_1(object sender, EventArgs e)
        {
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\admin\Desktop\Bi-31 windows\Bi-31 windows\Database1.mdf;Integrated Security=True";
            sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();
            
            if (label6.Text == "0")
            {
                tabControl1.TabPages.Remove(tabPage2);
                tabControl1.TabPages.Remove(tabPage3);
                tabControl1.TabPages.Remove(tabPage4);
                tabControl1.TabPages.Remove(tabPage5);
            }
            else
            {
                SqlCommand read = new SqlCommand("SELECT * FROM [Users]", sqlConnection);

                sqlReader = read.ExecuteReader();
                List<string[]> data = new List<string[]>();
                while (sqlReader.Read())
                {
                    data.Add(new string[7]);

                    data[data.Count - 1][0] = sqlReader[0].ToString();
                    data[data.Count - 1][1] = sqlReader[1].ToString().Replace(" ", ""); ;
                    data[data.Count - 1][2] = sqlReader[2].ToString().Replace(" ", ""); ;
                    data[data.Count - 1][3] = sqlReader[3].ToString();
                    data[data.Count - 1][4] = sqlReader[4].ToString();
                    data[data.Count - 1][5] = sqlReader[5].ToString();
                    data[data.Count - 1][6] = sqlReader[6].ToString();
                }

                sqlReader.Close();


                foreach (string[] s in data)
                    dataGridView1.Rows.Add(s);
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {

        }

        private void Button2_Click(object sender, EventArgs e)
        {
        }

        private void Button3_Click(object sender, EventArgs e)
        {
        }

        private void Button4_Click(object sender, EventArgs e) //добавить пользователя
        {
            if (textBox3.Text.Contains(' '))
            {
                MessageBox.Show("Имя пользователя не должно содержать пробелов");
                return;
            }
            bool flag = false;
            SqlCommand find = new SqlCommand("SELECT * FROM [Users]", sqlConnection);
            sqlReader = find.ExecuteReader();
            while (sqlReader.Read())
            {
                if (textBox3.Text==Convert.ToString(sqlReader["Login"]).Replace(" ", ""))
                {
                    flag = true;
                    MessageBox.Show("Этот пользователь уже существует");
                }
            }
            sqlReader.Close();
            if (flag!=true)
            {
                SqlCommand add = new SqlCommand("INSERT INTO Users (Login, Password, Admin, PassRestr) VALUES (@Name, @Pass, @Admin,@PassRestr)", sqlConnection);
                add.Parameters.AddWithValue("Name", textBox3.Text);
                add.Parameters.AddWithValue("Pass", "TempPass12");
                if (checkBox1.Checked)
                    add.Parameters.AddWithValue("Admin", 1);
                else
                    add.Parameters.AddWithValue("Admin", 0);
                if (checkBox2.Checked)
                    add.Parameters.AddWithValue("PassRestr", 1);
                else
                    add.Parameters.AddWithValue("PassRestr", 0);
                add.ExecuteNonQuery();
                MessageBox.Show("Добавлен пользователь " + textBox3.Text + " с временным паролем TempPass12");
            }
        }

        private void Button2_Click_1(object sender, EventArgs e)//выйти
        {
            if (sqlReader != null)
                sqlReader.Close();
            this.Close();
        }

        private void PassChangeButton_Click(object sender, EventArgs e)//сменить пароль
        {
            
            if (textBox1.Text==textBox2.Text)
            {
                int id = 0;
                int PassRestr = 0;
                SqlCommand find = new SqlCommand("SELECT * FROM [Users]", sqlConnection);
                sqlReader = find.ExecuteReader();
                while (sqlReader.Read())
                {
                    if (label4.Text == Convert.ToString(sqlReader["Login"]).Replace(" ", ""))
                    {
                        id = Convert.ToInt32(sqlReader["ID"]);
                        PassRestr= Convert.ToInt32(sqlReader["PassRestr"]);
                        break;
                    }
                }
                if(textBox1.Text.Contains(' '))
                {
                    MessageBox.Show("Пароль не должен содержать пробелов");
                    return;
                }
                sqlReader.Close();
                SqlCommand change = new SqlCommand("UPDATE Users SET Password = @Password, PassEncoded = 1 WHERE ID = @ID", sqlConnection);
                if (PassRestr == 1)
                {
                    if (textBox1.Text.Any(char.IsUpper) &&
                      textBox1.Text.Any(char.IsLower) &&
                      textBox1.Text.Any(char.IsDigit))
                    {
                        change.Parameters.AddWithValue("Password", Program.Encode(textBox1.Text, Program.olkey));
                        change.Parameters.AddWithValue("ID", id);
                        change.ExecuteNonQuery();
                        MessageBox.Show("Пароль изменен");
                    }
                    else
                        MessageBox.Show("Пароль должен содержать хотя бы одну цифру, одну строчную и одну прописную букву");
                }
                else
                {
                    change.Parameters.AddWithValue("Password", Program.Encode(textBox1.Text, Program.olkey));
                    change.Parameters.AddWithValue("ID", id);
                    change.ExecuteNonQuery();
                    MessageBox.Show("Пароль изменен");
                }
            }
            else
            {
                MessageBox.Show("Пароли не совпадают");
            }

        }

        private void Reload_Table_Click(object sender, EventArgs e)//загрузить данные в базу
        {   
            int cou = 0;
            List<string[]> data = new List<string[]>();
            for (int rows = 0; rows < dataGridView1.Rows.Count-1; rows++)
            {
                data.Add(new string[7]);
                data[rows][0] = dataGridView1.Rows[rows].Cells[0].Value.ToString();
                data[rows][1] = dataGridView1.Rows[rows].Cells[1].Value.ToString().Replace(" ", ""); 
                data[rows][2] = dataGridView1.Rows[rows].Cells[2].Value.ToString().Replace(" ", ""); 
                data[rows][3] = dataGridView1.Rows[rows].Cells[3].Value.ToString();
                data[rows][4] = dataGridView1.Rows[rows].Cells[4].Value.ToString();
                data[rows][5] = dataGridView1.Rows[rows].Cells[5].Value.ToString();
                data[rows][6] = dataGridView1.Rows[rows].Cells[6].Value.ToString();
            }
            for(int rows = 0; rows < dataGridView1.Rows.Count - 1; rows++)
                for (int cells = 3; cells < 7; cells++)
                {
                    if (!dataGridView1.Rows[rows].Cells[cells].Value.ToString().All(Char.IsDigit))
                    {
                        MessageBox.Show("Одно из значений таблицы не соответствует требуемому типу данных");
                        return;
                    }
                }
            SqlCommand change = new SqlCommand("UPDATE Users SET Password = @Pass, Admin=@Admin, Suspended=@Block, PassRestr=@PassRestr, PassEncoded=@EncPass WHERE Login = @Name", sqlConnection);
            change.Parameters.Add(new SqlParameter("@Pass", ""));
            change.Parameters.Add(new SqlParameter("@Admin", 0));
            change.Parameters.Add(new SqlParameter("@Name", ""));
            change.Parameters.Add(new SqlParameter("@Block", 0));
            change.Parameters.Add(new SqlParameter("@PassRestr", 0));
            change.Parameters.Add(new SqlParameter("@EncPass", 0));
            foreach (string[] s in data)
            {
                change.Parameters["@Pass"].Value=s[2].Replace(" ", ""); ;
                change.Parameters["@Admin"].Value = s[3];
                change.Parameters["@Name"].Value = s[1].Replace(" ", ""); ;
                change.Parameters["@Block"].Value = s[5];
                change.Parameters["@PassRestr"].Value = s[4];
                change.Parameters["@EncPass"].Value = s[6];
                change.ExecuteNonQuery();
                cou++;
            }
        }

        private void Button1_Click_1(object sender, EventArgs e)//перезагрузить таблицу
        {
            dataGridView1.Rows.Clear();
            dataGridView1.Refresh();

            SqlCommand read = new SqlCommand("SELECT * FROM [Users]", sqlConnection);

            sqlReader = read.ExecuteReader();
            List<string[]> data = new List<string[]>();
            while (sqlReader.Read())
            {
                data.Add(new string[7]);

                data[data.Count - 1][0] = sqlReader[0].ToString();
                data[data.Count - 1][1] = sqlReader[1].ToString().Replace(" ", ""); ;
                data[data.Count - 1][2] = sqlReader[2].ToString().Replace(" ", ""); ;
                data[data.Count - 1][3] = sqlReader[3].ToString();
                data[data.Count - 1][4] = sqlReader[4].ToString();
                data[data.Count - 1][5] = sqlReader[5].ToString();
                data[data.Count - 1][6] = sqlReader[6].ToString();
            }

            sqlReader.Close();


            foreach (string[] s in data)
                dataGridView1.Rows.Add(s);
        }

        private void TabPage2_Click(object sender, EventArgs e)
        {

        }

        private void Button3_Click_1(object sender, EventArgs e)//удалить
        {
            if (textBox4.Text=="admin")
            {
                MessageBox.Show("Нельзя удалить пользователя admin");
                return;
            }
            SqlCommand delete = new SqlCommand("DELETE FROM Users WHERE Login=@Login", sqlConnection);
            delete.Parameters.AddWithValue("Login", textBox4.Text);
            delete.ExecuteNonQuery();
            MessageBox.Show("Пользователь удален");
        }

        public int[] texboxes_into_array()
        {
            try
            {
                int[] res = new int[16];
                res[0] = Convert.ToInt32(square0.Text);
                res[1] = Convert.ToInt32(square1.Text);
                res[2] = Convert.ToInt32(square2.Text);
                res[3] = Convert.ToInt32(square3.Text);
                res[4] = Convert.ToInt32(square4.Text);
                res[5] = Convert.ToInt32(square5.Text);
                res[6] = Convert.ToInt32(square6.Text);
                res[7] = Convert.ToInt32(square7.Text);
                res[8] = Convert.ToInt32(square8.Text);
                res[9] = Convert.ToInt32(square9.Text);
                res[10] = Convert.ToInt32(square10.Text);
                res[11] = Convert.ToInt32(square11.Text);
                res[12] = Convert.ToInt32(square12.Text);
                res[13] = Convert.ToInt32(square13.Text);
                res[14] = Convert.ToInt32(square14.Text);
                res[15] = Convert.ToInt32(square15.Text);
                int min = 100;
                for (int k = 0; k < 16; k++)
                    if (res[k] < min)
                        min = res[k];
                if (min == 1)
                {
                    for (int k = 0; k < 16; k++)
                    {
                        res[k] = res[k] - 1;
                    }
                }
                return res;
            }
            catch
            {
                int[] res = new int[1];
                res[0] = -1;
                MessageBox.Show("Переданы неверные параметры");
                return res;
            }
        }

        private void Button4_Click_1(object sender, EventArgs e)//проверить квадрат
        {
            if (texboxes_into_array()[0] != -1 && Program.CheckSquare(texboxes_into_array()) != -1)
            {
                square0.ReadOnly = true;
                square1.ReadOnly = true;
                square2.ReadOnly = true;
                square3.ReadOnly = true;
                square4.ReadOnly = true;
                square5.ReadOnly = true;
                square6.ReadOnly = true;
                square7.ReadOnly = true;
                square8.ReadOnly = true;
                square9.ReadOnly = true;
                square10.ReadOnly = true;
                square11.ReadOnly = true;
                square12.ReadOnly = true;
                square13.ReadOnly = true;
                square14.ReadOnly = true;
                square15.ReadOnly = true;
                button5.Enabled = true;
            }
            else
                MessageBox.Show("Квадрат не магический");
        }

        private void Button5_Click(object sender, EventArgs e)//шифрование
        {
            int[] newkey = texboxes_into_array();
            List<string[]> data = new List<string[]>();
            for (int rows = 0; rows < dataGridView1.Rows.Count - 1; rows++)
            {
                data.Add(new string[7]);
                data[rows][0] = dataGridView1.Rows[rows].Cells[0].Value.ToString();
                data[rows][1] = dataGridView1.Rows[rows].Cells[1].Value.ToString().Replace(" ", "");
                data[rows][2] = dataGridView1.Rows[rows].Cells[2].Value.ToString().Replace(" ", "");
                data[rows][3] = dataGridView1.Rows[rows].Cells[3].Value.ToString();
                data[rows][4] = dataGridView1.Rows[rows].Cells[4].Value.ToString();
                data[rows][5] = dataGridView1.Rows[rows].Cells[5].Value.ToString();
                data[rows][6] = dataGridView1.Rows[rows].Cells[6].Value.ToString();
            }
            SqlCommand change = new SqlCommand("UPDATE Users SET Password = @Pass, PassEncoded=1 WHERE Login = @Name", sqlConnection);
            change.Parameters.Add(new SqlParameter("@Name", ""));
            change.Parameters.Add(new SqlParameter("@Pass", ""));
            foreach (string[] s in data)
            {
                change.Parameters["@Name"].Value = s[1];
                string pass;
                if (s[6] == "1")
                {
                    string buf = Program.Decode(s[2].ToString(), Program.olkey);
                    pass = Program.Encode(buf, newkey);
                    change.Parameters["@Pass"].Value=pass;
                }
                else
                {
                    pass = Program.Encode(s[2].ToString(), newkey);
                    change.Parameters["@Pass"].Value = pass;
                }
                change.ExecuteNonQuery();
            }
            
            Program.olkey = newkey;
            string key = String.Join(" ", newkey.Select(p => p.ToString()).ToArray());
            
            Bitmap bmp = new Bitmap(textBox5.Text);
            int[] pic_key = newkey;
            for (int j = 0; j < pic_key.Length; j++)
            {
                Color pixel = bmp.GetPixel(0, j);
                bmp.SetPixel(0, j, Color.FromArgb(pic_key[j], pixel.G, pixel.B));
            }
            SaveFileDialog savefile = new SaveFileDialog();
            savefile.Filter = "Image Files(*.png)|*.png";
            savefile.InitialDirectory = @"C:\Users\admin\Desktop\Bi-31 windows";
            if (savefile.ShowDialog() == DialogResult.OK)
            {
                textBox5.Text = savefile.FileName.ToString();
                pictureBox1.ImageLocation = textBox5.Text;
                bmp.Save(textBox5.Text);
                fleshd.WriteString(@"E:", key);
                Button1_Click_1(sender, e);
                textBox5.ReadOnly = true;
            }
        }

        private void Button6_Click(object sender, EventArgs e)//проверить картинку
        {
            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.Filter = "Image Files(*.png)|*.png";
            openDialog.InitialDirectory = @"C:\Users\admin\Desktop\Bi-31 windows";

            if(openDialog.ShowDialog()==DialogResult.OK)
            {
                textBox5.Text = openDialog.FileName.ToString();
                pictureBox1.ImageLocation = textBox5.Text;

                Bitmap bmp = new Bitmap(textBox5.Text);
                int[] pic_key = new int[16];
                for (int j = 0; j < Program.olkey.Length; j++)
                {
                    Color pixel = bmp.GetPixel(0, j);
                    int value = pixel.R;
                    pic_key[j] = value;
                }
                if (Program.olkey.SequenceEqual(pic_key) != true)
                {
                    MessageBox.Show("Неверное изображение");
                    Environment.Exit(0);
                }
                else
                {
                    groupBox1.Visible = true;
                    button7.Visible = true;
                    textBox6.Visible = true;
                }
            }

            
        }

        private void Button7_Click(object sender, EventArgs e)//заменить картинку
        {
            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.Filter = "Image Files(*.png)|*.png";
            openDialog.InitialDirectory = @"C:\Users\admin\Desktop\Bi-31 windows";

            if (openDialog.ShowDialog() == DialogResult.OK)
            {
                textBox6.Text = openDialog.FileName.ToString();
                pictureBox1.ImageLocation = textBox6.Text;
                Bitmap bmp = new Bitmap(textBox6.Text);
                int[] pic_key = Program.olkey;
                for (int j = 0; j < pic_key.Length; j++)
                {
                    Color pixel = bmp.GetPixel(0, j);
                    bmp.SetPixel(0, j, Color.FromArgb(pic_key[j], pixel.G, pixel.B));
                    Console.WriteLine(pic_key[j] + " " + bmp.GetPixel(0, j).R);
                }
                SaveFileDialog savefile = new SaveFileDialog();
                savefile.Filter = "Image Files(*.png)|*.png";
                savefile.InitialDirectory = @"C:\Users\admin\Desktop\Bi-31 windows";
                if (savefile.ShowDialog() == DialogResult.OK)
                {
                    textBox6.Text = savefile.FileName.ToString();
                    pictureBox1.ImageLocation = textBox6.Text;
                    bmp.Save(textBox6.Text);
                }
            }
        }
    }
}
