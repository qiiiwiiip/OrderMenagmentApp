using System;
using System.Collections.Generic;
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
using System.Data.SqlClient;
using System.Data;

namespace OrderMenagmentApp
{
    /// <summary>
    /// Логика взаимодействия для CustomersWindow.xaml
    /// </summary>
    public partial class CustomersWindow : Window
    {
        private const string ConnectionString = @"Data Source=MYPC;Initial Catalog=OrderMenagmentDB;Integrated Security=True";

        public CustomersWindow()
        {
            InitializeComponent();
            DisplayDataInGrid();
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
        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dataGrid.SelectedItem != null)
            {
                DataRowView selectedRow = (DataRowView)dataGrid.SelectedItem;
                firstNameTextBox.Text = selectedRow["FirstName"].ToString();
                lastNameTextBox.Text = selectedRow["LastName"].ToString();
                patronymicTextBox.Text = selectedRow["Patronymic"].ToString();
                phoneNumberTextBox.Text = selectedRow["PhoneNumber"].ToString();
                birthdateDatePicker.SelectedDate = selectedRow["Birthdate"] as DateTime?;

            }
        }
        private void DisplayDataInGrid()
        {
            string query = "SELECT * FROM Clients"; // Замените на ваш запрос SQL

            DataTable dataTable = GetDataFromDatabase(query);

            dataGrid.ItemsSource = dataTable.DefaultView;
        }



        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            string firstName = firstNameTextBox.Text;
            string lastName = lastNameTextBox.Text;
            string patronymic = patronymicTextBox.Text;
            string phoneNumber = phoneNumberTextBox.Text;
            string birthdate = birthdateDatePicker.SelectedDate?.ToString("yyyy-MM-dd");

            string query = $"INSERT INTO Clients (FirstName, LastName, Patronymic, PhoneNumber, Birthdate) VALUES ('{firstName}', '{lastName}', '{patronymic}', '{phoneNumber}', '{birthdate}')";
            ExecuteNonQuery(query);
            DisplayDataInGrid();
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            DataRowView selectedRow = (DataRowView)dataGrid.SelectedItem;
            if (selectedRow != null)
            {
                int clientId = (int)selectedRow["ClientID"];
                string firstName = firstNameTextBox.Text;
                string lastName = lastNameTextBox.Text;
                string patronymic = patronymicTextBox.Text;
                string phoneNumber = phoneNumberTextBox.Text;
                string birthdate = birthdateDatePicker.SelectedDate?.ToString("yyyy-MM-dd");

                string query = $"UPDATE Clients SET FirstName='{firstName}', LastName='{lastName}', Patronymic='{patronymic}', PhoneNumber='{phoneNumber}', Birthdate='{birthdate}' WHERE ClientID={clientId}";
                ExecuteNonQuery(query);
                DisplayDataInGrid();
            }

        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            DataRowView selectedRow = (DataRowView)dataGrid.SelectedItem;
            if (selectedRow != null)
            {
                int clientId = (int)selectedRow["ClientID"];
                string query = $"DELETE FROM Clients WHERE ClientID={clientId}";
                ExecuteNonQuery(query);
                DisplayDataInGrid();
            }
        }

        private void ExecuteNonQuery(string query)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(query, connection);
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }
    }
}