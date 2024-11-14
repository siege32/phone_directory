using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Расчетная_работа1
{
    public partial class Изменить : Form
    {
        private SqlConnection SqlConnection = null;
        string name; DataGridView datagrid; int stroke;

        public Изменить(DataGridView D, int Stroke, string Name)
        {
            InitializeComponent();
            name = Name;
            datagrid = D;
            stroke = Stroke;
        }

        private void Изменить_Load(object sender, EventArgs e)
        {
            SqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["connectDB"].ConnectionString);
            SqlConnection.Open();
            if (name == "tabPage1")
            {
                textBox1.Text = datagrid[1, stroke].Value.ToString();
                textBox2.Text = datagrid[2, stroke].Value.ToString();
                textBox3.Text = datagrid[3, stroke].Value.ToString();
                textBox4.Text = datagrid[4, stroke].Value.ToString();
                comboBox1.Text = datagrid[5, stroke].Value.ToString();
                // Очищаем предыдущие элементы ComboBox перед добавлением новых
                comboBox1.Items.Clear();

                // Выполняем запрос к базе данных
                SqlCommand command = new SqlCommand(
                    "SELECT Surname, Name, Fathername FROM Client", SqlConnection);

                SqlDataReader reader = command.ExecuteReader();

                // Читаем результаты запроса и добавляем их в Items коллекцию ComboBox
                while (reader.Read())
                {
                    string fullName = $"{reader["Surname"]} {reader["Name"]} {reader["Fathername"]}";
                    comboBox1.Items.Add(fullName);
                }

                // Закрываем SqlDataReader
                reader.Close();

                textBox5.Visible = false;
                textBox6.Visible = false;
                label10.Visible = false;
                label11.Visible = false;
                label12.Visible = false;
                comboBox1.Visible = true;
            }
            if (name == "tabPage2")
            {
                textBox1.Text = datagrid[0, stroke].Value.ToString();
                textBox2.Text = datagrid[1, stroke].Value.ToString();
                textBox3.Text = datagrid[2, stroke].Value.ToString();
                textBox4.Text = datagrid[3, stroke].Value.ToString();
                textBox5.Text = datagrid[4, stroke].Value.ToString();
                label1.Text = "Фамилия: ";
                label2.Text = "Имя: ";
                label3.Text = "Отчество: ";
                label4.Text = "Вод. удостоверение: ";
                label8.Text = "Номер: ";
                label12.Text = "Категория: ";
                label6.Visible = false;
                textBox5.Visible = true;
                textBox6.Visible = true;
                comboBox1.Visible = false;
                textBox6.Text = datagrid[5, stroke].Value.ToString();
            }

            DataSet dataSet;
            SqlDataAdapter carDataAdapter, clientDataAdapter;

            carDataAdapter = new SqlDataAdapter(
                "SELECT Id, Brand, Model, Year, Color, IdClient FROM Car",
                SqlConnection);

            dataSet = new DataSet();

            carDataAdapter.Fill(dataSet, "Car");

            clientDataAdapter = new SqlDataAdapter(
                "SELECT Id, Surname, Name, Fathername, Certificate, Number, Category FROM Client",
                SqlConnection);

            clientDataAdapter.Fill(dataSet, "Client");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (name == "tabPage1")
            {
                datagrid[1, stroke].Value = textBox1.Text;
                datagrid[2, stroke].Value = textBox2.Text;
                datagrid[3, stroke].Value = textBox3.Text;
                datagrid[4, stroke].Value = textBox4.Text;
                datagrid[5, stroke].Value = comboBox1.Text; // Это строковое значение клиента, а не его ID

                // Здесь нужно получить ID выбранного клиента по его имени (comboBox1.Text)
                int clientId; // Переменная для хранения ID клиента
                string selectClientIdQuery = "SELECT Id FROM Client WHERE CONCAT(Surname, ' ', Name, ' ', Fathername) = @FullName";
                using (SqlCommand cmdSelectClientId = new SqlCommand(selectClientIdQuery, SqlConnection))
                {
                    // Передаем значение имени и фамилии для поиска ID клиента
                    cmdSelectClientId.Parameters.AddWithValue("@FullName", comboBox1.Text);
                    clientId = (int)cmdSelectClientId.ExecuteScalar(); // Получаем ID клиента
                }

                string updateQuery = "UPDATE Car SET Brand = @Brand, Model = @Model, Year = @Year, Color = @Color, IdClient = @IdClient WHERE Id = @OldId";
                using (SqlCommand cmd = new SqlCommand(updateQuery, SqlConnection))
                {
                    cmd.Parameters.AddWithValue("Brand", textBox1.Text);
                    cmd.Parameters.AddWithValue("Model", textBox2.Text);
                    cmd.Parameters.AddWithValue("Year", textBox3.Text);
                    cmd.Parameters.AddWithValue("Color", textBox4.Text);
                    cmd.Parameters.AddWithValue("IdClient", comboBox1.Text); // Используем полученный ID клиента
                    cmd.Parameters.AddWithValue("OldId", Convert.ToInt32(datagrid[0, stroke].Value)); // Ensure this is an int
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Запись успешно добавлена.");

                this.Close();
            }

            if (name == "tabPage2")
            {
                datagrid[0, stroke].Value = textBox1.Text;
                datagrid[1, stroke].Value = textBox2.Text;
                datagrid[2, stroke].Value = textBox3.Text;
                datagrid[3, stroke].Value = textBox4.Text;
                datagrid[4, stroke].Value = textBox5.Text;
                datagrid[5, stroke].Value = textBox6.Text;

                // Convert the datagrid value to an integer for @OldId parameter
                int oldId = Convert.ToInt32(datagrid[6, stroke].Value);

                string updateQuery = "UPDATE Client SET Surname = @Surname, Name = @Name, Fathername = @Fathername, Certificate = @Certificate, Number = @Number, Category = @Category WHERE Id = @OldId";
                using (SqlCommand cmd = new SqlCommand(updateQuery, SqlConnection))
                {
                    // Установка параметров запроса
                    cmd.Parameters.AddWithValue("@Surname", textBox1.Text);
                    cmd.Parameters.AddWithValue("@Name", textBox2.Text);
                    cmd.Parameters.AddWithValue("@Fathername", textBox3.Text);
                    cmd.Parameters.AddWithValue("@Certificate", textBox4.Text);
                    cmd.Parameters.AddWithValue("@Number", textBox5.Text);
                    cmd.Parameters.AddWithValue("@Category", textBox6.Text);
                    cmd.Parameters.AddWithValue("@OldId", oldId); // Ensure this is an int

                    // Выполнение команды SQL
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Запись успешно обновлена.");

                SqlConnection.Close();
                this.Close();
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (name == "tabPage1")
            {
                if (textBox1.Text != "" && textBox3.Text != "" && textBox4.Text != "" && comboBox1.Text != "" &&
                int.TryParse(textBox3.Text, out int a) /*&& int.TryParse(textBox5.Text, out a)*/)
                    button1.Enabled = true;
                if (int.TryParse(textBox3.Text, out a) == false && textBox3.Text.Length > 0)
                {
                    MessageBox.Show("«Год» введен некорректно");
                    button1.Enabled = false;
                    textBox3.Text = "";
                }
                /*if (int.TryParse(textBox5.Text, out a) == false && textBox5.Text.Length > 0)
                {
                    MessageBox.Show("Идентификатор клиента введен некорректно");
                    button1.Enabled = false;
                    textBox5.Text = "";
                }*/
            }
            if (name == "tabPage2")
            {
                if (textBox1.Text != "" && textBox2.Text != "" && textBox4.Text != "" && textBox5.Text != "" && textBox6.Text != "" &&
                int.TryParse(textBox4.Text, out int a) && int.TryParse(textBox5.Text, out a))
                    button1.Enabled = true;
                if (int.TryParse(textBox4.Text, out a) == false && textBox4.Text.Length > 0)
                {
                    MessageBox.Show("«Вод. удостоверение» введен некорректно");
                    button1.Enabled = false;
                    textBox4.Text = "";
                }
                if (int.TryParse(textBox5.Text, out a) == false && textBox5.Text.Length > 0)
                {
                    MessageBox.Show("«Номер» введен некорректно");
                    button1.Enabled = false;
                    textBox5.Text = "";
                }
            }
        }


        private void button2_Click(object sender, EventArgs e)
        {
            SqlConnection.Close();
            this.Close();
        }
    }
}
