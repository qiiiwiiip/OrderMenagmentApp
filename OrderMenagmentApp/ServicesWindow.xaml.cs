using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace OrderMenagmentApp
{
    /// <summary>
    /// Логика взаимодействия для ServicesWindow.xaml
    /// </summary>
    public partial class ServicesWindow : Window
    {
        private const string ConnectionString = @"Data Source=MYPC;Initial Catalog=OrderMenagmentDB;Integrated Security=True";


        public ServicesWindow()
        {
            InitializeComponent();
            DisplayDataInGrid();
            LoadMaterials(); // Загрузка списка материалов
        }

        private DataTable GetDataFromDatabase(string query)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                DataTable dataTable = new DataTable();

                try
                {
                    connection.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    adapter.Fill(dataTable);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }

                return dataTable;
            }
        }

        private void DisplayDataInGrid()
        {
            string query = "SELECT Services.*, Materials.Name AS MaterialName FROM Services INNER JOIN Materials ON Services.MaterialID = Materials.MaterialID";
            DataTable dataTable = GetDataFromDatabase(query);
            dataGrid.ItemsSource = dataTable.DefaultView;
        }

        private void LoadMaterials()
        {
            string query = "SELECT * FROM Materials";
            DataTable dataTable = GetDataFromDatabase(query);

            materialComboBox.ItemsSource = dataTable.DefaultView;
            materialComboBox.DisplayMemberPath = "Name";
            materialComboBox.SelectedValuePath = "MaterialID";
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dataGrid.SelectedItem != null)
            {
                DataRowView selectedRow = (DataRowView)dataGrid.SelectedItem;
                nameTextBox.Text = selectedRow["Name"].ToString();
                priceTextBox.Text = selectedRow["Price"].ToString();

                // Установка выбранного значения в ComboBox
                int materialID = Convert.ToInt32(selectedRow["MaterialID"]);
                materialComboBox.SelectedValue = materialID;
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            string name = nameTextBox.Text;
            decimal price = Convert.ToDecimal(priceTextBox.Text);
            int materialID = Convert.ToInt32(materialComboBox.SelectedValue);

            string query = $"INSERT INTO Services (Name, Price, MaterialID) VALUES ('{name}', {price}, {materialID})";
            GetDataFromDatabase(query);
            DisplayDataInGrid();
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            DataRowView selectedRow = (DataRowView)dataGrid.SelectedItem;
            if (selectedRow != null)
            {
                int serviceID = Convert.ToInt32(selectedRow["ServiceID"]);
                string name = nameTextBox.Text;
                decimal price = Convert.ToDecimal(priceTextBox.Text);
                int materialID = Convert.ToInt32(materialComboBox.SelectedValue);

                string query = $"UPDATE Services SET Name='{name}', Price={price}, MaterialID={materialID} WHERE ServiceID={serviceID}";
                GetDataFromDatabase(query);
                DisplayDataInGrid();
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            DataRowView selectedRow = (DataRowView)dataGrid.SelectedItem;
            if (selectedRow != null)
            {
                int serviceID = Convert.ToInt32(selectedRow["ServiceID"]);
                string query = $"DELETE FROM Services WHERE ServiceID={serviceID}";
                GetDataFromDatabase(query);
                DisplayDataInGrid();
            }
        }
    }
}