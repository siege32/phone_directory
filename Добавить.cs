using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Расчетная_работа1
{
    public partial class Добавить : Form
    {
        string name;
        private SqlConnection sqlConnection = null;
        DataSet dataSet; 
        SqlDataAdapter carDataAdapter, clientDataAdapter;
        DataGridView datagrid;

        public Добавить(string Name, DataGridView D)
        {
            InitializeComponent();
            name = Name; button1.Enabled = false;
            datagrid = D;
        }


        private void Добавить_Load(object sender, EventArgs e)
        {
            sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["connectDB"].ConnectionString);
            sqlConnection.Open();
            if (name == "tabPage1")
            {
                // Очищаем предыдущие элементы ComboBox перед добавлением новых
                comboBox1.Items.Clear();

                // Выполняем запрос к базе данных
                SqlCommand command = new SqlCommand(
                    "SELECT Surname, Name, Fathername FROM Client", sqlConnection);

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
                label1.Text = "Фамилия: ";
                label2.Text = "Имя: ";
                label3.Text = "Отчество: ";
                label4.Text = "Вод. удостоверение: ";
                label8.Text = "Номер: ";
                label12.Text = "Категория: ";
                label6.Visible = false;
                textBox5.Visible = true;
                comboBox1.Visible = false;
            }

            carDataAdapter = new SqlDataAdapter(
                "SELECT Id, Brand, Model, Year, Color, IdClient FROM Car",
                sqlConnection);

            dataSet = new DataSet();

            carDataAdapter.Fill(dataSet, "Car");

            clientDataAdapter = new SqlDataAdapter(
                "SELECT Id, Surname, Name, Fathername, Certificate, Number, Category FROM Client",
                sqlConnection);

            clientDataAdapter.Fill(dataSet, "Client");
        }

 

        private void button1_Click(object sender, EventArgs e)
        {

            if (name == "tabPage1")
            {
                SqlCommand command = new SqlCommand(
                $"INSERT INTO [Car] (Brand, Model, Year, Color, IdClient) VALUES (@Brand, @Model, @Year, @Color, @IdClient)"
                , sqlConnection);

                command.Parameters.AddWithValue("Brand", textBox1.Text);
                command.Parameters.AddWithValue("Model", textBox2.Text);
                command.Parameters.AddWithValue("Year", textBox3.Text);
                command.Parameters.AddWithValue("Color", textBox4.Text);
                command.Parameters.AddWithValue("IdClient", comboBox1.Text);

                command.ExecuteNonQuery();

                //UpdateCarDataGridView();

                MessageBox.Show("Запись успешно добавлена.");

                this.Close();
            }
            if (name == "tabPage2")
            {
                SqlCommand command = new SqlCommand(
                $"INSERT INTO [Client] (Surname,Name,Fathername,Certificate,Number,Category) VALUES (@Surname,@Name,@Fathername,@Certificate,@Number,@Category)" + $"SELECT SCOPE_IDENTITY();"                , sqlConnection);

                command.Parameters.AddWithValue("Surname", textBox1.Text);
                command.Parameters.AddWithValue("Name", textBox2.Text);
                command.Parameters.AddWithValue("Fathername", textBox3.Text);
                command.Parameters.AddWithValue("Certificate", textBox4.Text);
                command.Parameters.AddWithValue("Number", textBox5.Text);
                command.Parameters.AddWithValue("Category", textBox6.Text);

                command.ExecuteNonQuery();

                //UpdateClientDataGridView();

                MessageBox.Show("Запись успешно добавлена.");

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
            this.Close();
            sqlConnection.Close();
        }

        
    }
}
