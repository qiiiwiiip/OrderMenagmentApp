using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace OrderMenagmentApp
{
    public partial class OrdersWindow : Window
    {
        private const string ConnectionString = @"Data Source=MYPC;Initial Catalog=OrderMenagmentDB;Integrated Security=True";

        public OrdersWindow()
        {
            InitializeComponent();
            DisplayDataInGrid();
            LoadClients(); // Загрузка списка клиентов
            LoadEmployees(); // Загрузка списка сотрудников
            LoadServices(); // Загрузка списка услуг
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
            string query = "SELECT Orders.*, Clients.FirstName AS ClientFirstName, Clients.LastName AS ClientLastName, Employees.FirstName AS EmployeeFirstName, Employees.LastName AS EmployeeLastName, Services.Name AS ServiceName FROM Orders INNER JOIN Clients ON Orders.ClientID = Clients.ClientID INNER JOIN Employees ON Orders.EmployeeID = Employees.EmployeeID INNER JOIN Services ON Orders.ServiceID = Services.ServiceID";
            DataTable dataTable = GetDataFromDatabase(query);
            dataGrid.ItemsSource = dataTable.DefaultView;
        }

        private void LoadClients()
        {
            string query = "SELECT * FROM Clients";
            DataTable dataTable = GetDataFromDatabase(query);

            clientComboBox.ItemsSource = dataTable.DefaultView;
            clientComboBox.DisplayMemberPath = "FirstName";
            clientComboBox.SelectedValuePath = "ClientID";
        }

        private void LoadEmployees()
        {
            string query = "SELECT * FROM Employees";
            DataTable dataTable = GetDataFromDatabase(query);

            employeeComboBox.ItemsSource = dataTable.DefaultView;
            employeeComboBox.DisplayMemberPath = "FirstName";
            employeeComboBox.SelectedValuePath = "EmployeeID";
        }

        private void LoadServices()
        {
            string query = "SELECT * FROM Services";
            DataTable dataTable = GetDataFromDatabase(query);

            serviceComboBox.ItemsSource = dataTable.DefaultView;
            serviceComboBox.DisplayMemberPath = "Name";
            serviceComboBox.SelectedValuePath = "ServiceID";
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dataGrid.SelectedItem != null)
            {
                DataRowView selectedRow = (DataRowView)dataGrid.SelectedItem;
                descriptionTextBox.Text = selectedRow["Description"].ToString();
                startDateDatePicker.SelectedDate = (DateTime)selectedRow["StartDate"];
                endDateDatePicker.SelectedDate = selectedRow["EndDate"] as DateTime?;

                // Установка выбранных значений в ComboBox'ы
                int clientID = Convert.ToInt32(selectedRow["ClientID"]);
                clientComboBox.SelectedValue = clientID;

                int employeeID = Convert.ToInt32(selectedRow["EmployeeID"]);
                employeeComboBox.SelectedValue = employeeID;

                int serviceID = Convert.ToInt32(selectedRow["ServiceID"]);
                serviceComboBox.SelectedValue = serviceID;
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (clientComboBox.SelectedValue != null && employeeComboBox.SelectedValue != null && serviceComboBox.SelectedValue != null)
            {
                string description = descriptionTextBox.Text;
                DateTime startDate = startDateDatePicker.SelectedDate ?? DateTime.Now;
                DateTime? endDate = endDateDatePicker.SelectedDate;
                int clientID = (int)clientComboBox.SelectedValue;
                int employeeID = (int)employeeComboBox.SelectedValue;
                int serviceID = (int)serviceComboBox.SelectedValue;
                bool done = false; // По умолчанию заказ не выполнен

                string query = $"INSERT INTO Orders (Description, StartDate, EndDate, ClientID, EmployeeID, ServiceID, Done) VALUES ('{description}', '{startDate:yyyy-MM-dd}', '{endDate}', {clientID}, {employeeID}, {serviceID}, '{done}')";

                GetDataFromDatabase(query);
                DisplayDataInGrid();
            }
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            DataRowView selectedRow = (DataRowView)dataGrid.SelectedItem;
            if (selectedRow != null && clientComboBox.SelectedValue != null && employeeComboBox.SelectedValue != null && serviceComboBox.SelectedValue != null)
            {
                int orderID = (int)selectedRow["OrderID"];
                string description = descriptionTextBox.Text;
                DateTime startDate = startDateDatePicker.SelectedDate ?? DateTime.Now;
                DateTime? endDate = endDateDatePicker.SelectedDate;
                int clientID = (int)clientComboBox.SelectedValue;
                int employeeID = (int)employeeComboBox.SelectedValue;
                int serviceID = (int)serviceComboBox.SelectedValue;
                bool done = (bool)selectedRow["Done"];

                string query = $"UPDATE Orders SET Description='{description}', StartDate='{startDate:yyyy-MM-dd}', EndDate='{endDate}', ClientID={clientID}, EmployeeID={employeeID}, ServiceID={serviceID}, Done='{done}' WHERE OrderID={orderID}";

                GetDataFromDatabase(query);
                DisplayDataInGrid();
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            DataRowView selectedRow = (DataRowView)dataGrid.SelectedItem;
            if (selectedRow != null)
            {
                int orderID = (int)selectedRow["OrderID"];
                string query = $"DELETE FROM Orders WHERE OrderID={orderID}";

                GetDataFromDatabase(query);
                DisplayDataInGrid();
            }
        }
    }
}
