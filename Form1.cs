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
using System.Configuration;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Расчетная_работа1
{
    public partial class Form1 : Form
    {
        private SqlConnection sqlConnection = null;

        public Form1()
        {
            InitializeComponent();
        }

        DataSet dataSet;
        SqlDataAdapter carDataAdapter, clientDataAdapter;
        DataView dv1, dv2;

        private void Form1_Load(object sender, EventArgs e)
        {
            

            // Получаем данные для таблицы Car
            carDataAdapter = new SqlDataAdapter(
                /*"SELECT Brand, Model, Year, Color, IdClient, Id FROM Car"*/
                "SELECT * FROM ViewCar",
                sqlConnection);

            dataSet = new DataSet();

            carDataAdapter.Fill(dataSet, "Car");

            // Очищаем все столбцы в dataGridView1
            dataGridView1.Columns.Clear();

            // Устанавливаем источник данных для dataGridView1
            dataGridView1.DataSource = dataSet.Tables["Car"];

            // Получаем данные для таблицы Client
            clientDataAdapter = new SqlDataAdapter(
                "SELECT * FROM ViewClient",
                sqlConnection);

            clientDataAdapter.Fill(dataSet, "Client");

            // Очищаем все столбцы в dataGridView2
            dataGridView2.Columns.Clear();

            // Устанавливаем источник данных для dataGridView2
            dataGridView2.DataSource = dataSet.Tables["Client"];

            dataGridView1.Columns[0].Visible = false;
            dataGridView1.Columns[1].HeaderText = "Марка";
            dataGridView1.Columns[2].HeaderText = "Модель";
            dataGridView1.Columns[3].HeaderText = "Год выпуска";
            dataGridView1.Columns[4].HeaderText = "Цвет";
            dataGridView1.Columns[5].HeaderText = "Владелец";

            dataGridView2.Columns[0].HeaderText = "Фамилия";
            dataGridView2.Columns[1].HeaderText = "Имя";
            dataGridView2.Columns[2].HeaderText = "Отчество";
            dataGridView2.Columns[3].HeaderText = "Водительское уд.";
            dataGridView2.Columns[4].HeaderText = "Номер";
            dataGridView2.Columns[5].HeaderText = "Категория";
            dataGridView2.Columns[6].Visible = false;
            dataGridView2.Columns[7].Visible = false;

            fullname();
        }

        private void добавитьЗаписьВВыбраннуюТаблицуToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab.Name == "tabPage1")
            {
                Добавить form = new Добавить(tabControl1.SelectedTab.Name, dataGridView1);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    form.FormClosed += (s, args) =>
                    {
                        updateDG1();
                        dataGridView1.Rows[0].Selected = false;
                        dataGridView1.Rows[dataGridView1.RowCount - 1].Selected = true; // Выделение только что добавленной строки
                        dataGridView1.FirstDisplayedScrollingRowIndex = dataGridView1.RowCount - 1; // Прокрутка до последней строки
                    };
                }
                

            }
            if (tabControl1.SelectedTab.Name == "tabPage2")
            {
                Добавить form = new Добавить(tabControl1.SelectedTab.Name, dataGridView2);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    form.FormClosed += (s, args) =>
                    {
                        updateDG2();
                        dataGridView2.Rows[0].Selected = false;
                        dataGridView2.Rows[dataGridView2.RowCount - 1].Selected = true; // Выделение только что добавленной строки
                        dataGridView2.FirstDisplayedScrollingRowIndex = dataGridView2.RowCount - 1; // Прокрутка до последней строки
                        fullname();
                    };
                }
            }
        }

        private int StrokeDG1;
        private int StrokeDG2;

        private void изменитьЗаписьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StrokeDG1 = dataGridView1.SelectedCells[0].RowIndex;
            StrokeDG2 = dataGridView2.SelectedCells[0].RowIndex;
            if (tabControl1.SelectedTab.Name == "tabPage1")
            {
                Изменить form = new Изменить(dataGridView1, dataGridView1.SelectedCells[0].RowIndex, tabControl1.SelectedTab.Name);
                if (form.ShowDialog() == DialogResult.OK)
                { 
                    form.FormClosed += (s, args) =>
                    {
                        updateDG1();
                        dataGridView1.Rows[0].Selected = false;
                        dataGridView1.Rows[StrokeDG1].Selected = true;
                        dataGridView1.FirstDisplayedScrollingRowIndex = StrokeDG1;
                    };
                }
            }
            if (tabControl1.SelectedTab.Name == "tabPage2")
            {
                Изменить form = new Изменить(dataGridView2, dataGridView2.SelectedCells[0].RowIndex, tabControl1.SelectedTab.Name);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    form.FormClosed += (s, args) =>
                    {
                        updateDG2();
                        dataGridView2.Rows[0].Selected = false;
                        dataGridView2.Rows[StrokeDG2].Selected = true;
                        dataGridView2.FirstDisplayedScrollingRowIndex = StrokeDG2;
                    };
                }

            }
        }

        bool isFilterActive = false;

        private void найтиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!isFilterActive)
            {
                if (tabControl1.SelectedTab.Name == "tabPage1" && dataGridView1.SelectedRows.Count > 0)
                {
                    string idClient = dataGridView1.SelectedRows[0].Cells["IdClient"].Value.ToString();

                    // Фильтрация таблицы Car
                    dv1 = dataSet.Tables["Car"].DefaultView;
                    dv1.RowFilter = $"IdClient = '{idClient}'";
                    dataGridView1.DataSource = dv1.ToTable();

                    // Фильтрация таблицы Client
                    dv2 = dataSet.Tables["Client"].DefaultView;
                    dv2.RowFilter = $"Fullname = '{idClient}'";
                    dataGridView2.DataSource = dv2.ToTable();

                    isFilterActive = true;
                    найтиToolStripMenuItem.Text = "Отменить";
                }
                else if (tabControl1.SelectedTab.Name == "tabPage2" && dataGridView2.SelectedRows.Count > 0)
                {
                    string idClient = dataGridView2.SelectedRows[0].Cells["Fullname"].Value.ToString();

                    // Фильтрация таблицы Car
                    dv1 = dataSet.Tables["Car"].DefaultView;
                    dv1.RowFilter = $"IdClient = '{idClient}'";
                    dataGridView1.DataSource = dv1.ToTable();

                    // Фильтрация таблицы Client
                    dv2 = dataSet.Tables["Client"].DefaultView;
                    dv2.RowFilter = $"Fullname = '{idClient}'";
                    dataGridView2.DataSource = dv2.ToTable();

                    isFilterActive = true;
                    найтиToolStripMenuItem.Text = "Отменить";
                }
                else
                {
                    MessageBox.Show("Выберите запись для отображения.");
                }
            }
            else
            {
                // Сброс фильтрации и возвращение к исходным данным
                dataGridView1.DataSource = dataSet.Tables["Car"];
                dataGridView2.DataSource = dataSet.Tables["Client"];

                dv1.RowFilter = string.Empty;
                dv2.RowFilter = string.Empty;

                updateDG1();
                updateDG2();

                isFilterActive = false;
                найтиToolStripMenuItem.Text = "Найти";
            }
        }

        private void синхронизироватьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            updateDG1();
            updateDG2();
        }


        private void удалитьЗаписьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StrokeDG1 = dataGridView1.SelectedCells[0].RowIndex;
            StrokeDG2 = dataGridView2.SelectedCells[0].RowIndex;

            if (tabControl1.SelectedTab.Name == "tabPage1" && dataGridView1.SelectedRows.Count > 0)
            {
                int idToDelete = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["Id"].Value);

                string deleteQuery = "DELETE FROM Car WHERE Id = @Id";

                using (SqlCommand command = new SqlCommand(deleteQuery, sqlConnection))
                {
                    command.Parameters.AddWithValue("@Id", idToDelete);
                    command.ExecuteNonQuery();
                }

                // Запомним индекс строки перед удалением
                int deletedRowIndex = dataGridView1.SelectedRows[0].Index;

                updateDG1();

                dataGridView1.Rows[0].Selected = false;

                // Проверяем, не удалена ли последняя строка
                if (deletedRowIndex == dataGridView1.RowCount)
                {
                    // Если удалена последняя строка, то выделяем строку выше
                    dataGridView1.Rows[Math.Max(0, deletedRowIndex - 1)].Selected = true;
                }
                else
                {
                    // Если удалена не последняя строка, то выделяем строку, которая была после удаленной
                    dataGridView1.Rows[deletedRowIndex].Selected = true;
                }
            }
            else if (tabControl1.SelectedTab.Name == "tabPage2" && dataGridView2.SelectedRows.Count > 0)
            {
                string idToDelete = Convert.ToString(dataGridView2.SelectedRows[0].Cells["Fullname"].Value);

                string deleteQuery = "DELETE FROM Car WHERE IdClient = @Fullname";

                using (SqlCommand command = new SqlCommand(deleteQuery, sqlConnection))
                {
                    command.Parameters.AddWithValue("@Fullname", idToDelete);
                    command.ExecuteNonQuery();
                }

                int idToDelete2 = Convert.ToInt32(dataGridView2.SelectedRows[0].Cells["Id"].Value);

                string deleteClientQuery = "DELETE FROM Client WHERE Id = @Id";

                using (SqlCommand commandClient = new SqlCommand(deleteClientQuery, sqlConnection))
                {
                    commandClient.Parameters.AddWithValue("@Id", idToDelete2);
                    commandClient.ExecuteNonQuery();
                }

                updateDG1();
                updateDG2();

                fullname();
            }
            else
            {
                MessageBox.Show("Выберите запись для удаления.");
            }
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sqlConnection.Close();
            Close();
        }

        private void updateDG1()
        {
            dataSet.Tables["Car"].Clear();
            carDataAdapter.Fill(dataSet, "Car");
            dataGridView1.DataSource = dataSet.Tables["Car"];
            fullname();
        }

        private void updateDG2()
        {
            dataSet.Tables["Client"].Clear();
            clientDataAdapter.Fill(dataSet, "Client");
            dataGridView2.DataSource = dataSet.Tables["Client"];
        }

        private void fullname()
        {
            // Проверка наличия столбца fullname
            if (!dataSet.Tables["Client"].Columns.Contains("fullname"))
            {
                dataSet.Tables["Client"].Columns.Add("fullname", typeof(string));
            }

            foreach (DataRow row in dataSet.Tables["Client"].Rows)
            {
                string fullName = row["Surname"].ToString() + " " + row["Name"].ToString();

                // Добавляем отчество, если оно доступно
                if (dataSet.Tables["Client"].Columns.Contains("Fathername") && !string.IsNullOrEmpty(row["Fathername"].ToString()))
                {
                    fullName += " " + row["Fathername"].ToString();
                }

                row["fullname"] = fullName;
            }

            // Обновление DataGridView2
            if (dataGridView2.DataSource != null)
            {
                dataGridView2.Columns["fullname"].Visible = false;
            }
        }

    }
}
