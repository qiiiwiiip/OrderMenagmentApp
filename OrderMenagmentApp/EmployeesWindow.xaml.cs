using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace OrderMenagmentApp
{
    public partial class EmployeesWindow : Window
    {
        private const string ConnectionString = @"Data Source=MYPC;Initial Catalog=OrderMenagmentDB;Integrated Security=True";

        public EmployeesWindow()
        {
            InitializeComponent();
            DisplayDataInGrid();
            LoadJobTitles(); // Загрузка списка должностей


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
            string query = "SELECT Employees.*, JobTitles.JobTitles FROM Employees INNER JOIN JobTitles ON Employees.JobTitleID = JobTitles.JobTitleID";
            DataTable dataTable = GetDataFromDatabase(query);
            dataGrid.ItemsSource = dataTable.DefaultView;
        }


        private void LoadJobTitles()
        {
            string query = "SELECT * FROM JobTitles";
            DataTable dataTable = GetDataFromDatabase(query);

            jobTitleComboBox.ItemsSource = dataTable.DefaultView;
            jobTitleComboBox.DisplayMemberPath = "JobTitles";
            jobTitleComboBox.SelectedValuePath = "JobTitleID";
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

                // Установка выбранного значения в ComboBox
                int jobTitleID = Convert.ToInt32(selectedRow["JobTitleID"]);
                jobTitleComboBox.SelectedValue = jobTitleID;
            }
        }



        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (jobTitleComboBox.SelectedValue != null)
            {
                string firstName = firstNameTextBox.Text;
                string lastName = lastNameTextBox.Text;
                string patronymic = patronymicTextBox.Text;
                string phoneNumber = phoneNumberTextBox.Text;
                string birthdate = birthdateDatePicker.SelectedDate?.ToString("yyyy-MM-dd");
                int jobTitleId = (int)jobTitleComboBox.SelectedValue;

                string query = $"INSERT INTO Employees (FirstName, LastName, Patronymic, PhoneNumber, Birthdate, JobTitleID) VALUES ('{firstName}', '{lastName}', '{patronymic}', '{phoneNumber}', '{birthdate}', '{jobTitleId}')";
                
                ExecuteNonQuery(query);
                DisplayDataInGrid();
            }

        }
        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            DataRowView selectedRow = (DataRowView)dataGrid.SelectedItem;
            if (selectedRow != null && jobTitleComboBox.SelectedValue != null)
            {
                int employeeId = (int)selectedRow["EmployeeID"];
                string firstName = firstNameTextBox.Text;
                string lastName = lastNameTextBox.Text;
                string patronymic = patronymicTextBox.Text;
                string phoneNumber = phoneNumberTextBox.Text;
                string birthdate = birthdateDatePicker.SelectedDate?.ToString("yyyy-MM-dd");
                int jobTitleId = (int)jobTitleComboBox.SelectedValue;

                string query = $"UPDATE Employees SET FirstName='{firstName}', LastName='{lastName}', Patronymic='{patronymic}', PhoneNumber='{phoneNumber}', Birthdate='{birthdate}', JobTitleID={jobTitleId} WHERE EmployeeID={employeeId}";
                ExecuteNonQuery(query);
                DisplayDataInGrid();
            }

        }
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            DataRowView selectedRow = (DataRowView)dataGrid.SelectedItem;
            if (selectedRow != null)
            {
                int employeeId = (int)selectedRow["EmployeeID"];
                string query = $"DELETE FROM Employees WHERE EmployeeID={employeeId}";
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