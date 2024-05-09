using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
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
    /// Логика взаимодействия для UserWindow.xaml
    /// </summary>
    public partial class UserWindow : Window
    {
        private const string ConnectionString = @"Data Source=MYPC;Initial Catalog=OrderMenagmentDB;Integrated Security=True";

        public UserWindow()
        {
            InitializeComponent();
            DisplayDataInGrid();


        }

        // Обработчик события нажатия на кнопку Logout
        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            AuthorizationWindow authorizationWindow = new AuthorizationWindow(); // Создаем новое окно авторизации
            authorizationWindow.Show(); // Показываем новое окно авторизации
            Close(); // Закрываем текущее окно
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
            string query = "SELECT * FROM Orders";
            DataTable dataTable = GetDataFromDatabase(query);
            dataGrid.ItemsSource = dataTable.DefaultView;
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in dataGrid.Items)
            {
                if (item is DataRowView rowView)
                {
                    DataRow row = rowView.Row;
                    int orderID = (int)row["OrderID"];
                    //DateTime? endDate = row["EndDate"] as DateTime?;
                    DateTime? endDate = endDateDatePicker.SelectedDate;

                    bool done = (bool)row["Done"];

                    // Обновляем только EndDate и Done
                    string query = $"UPDATE Orders SET EndDate='{endDate:yyyy-MM-dd}', Done='{done}' WHERE OrderID={orderID}";
                    GetDataFromDatabase(query);
                    DisplayDataInGrid();

                }
            }

            MessageBox.Show("Data updated successfully!");
        }




        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dataGrid.SelectedItem != null)
            {
                DataRowView selectedRow = (DataRowView)dataGrid.SelectedItem;
                //descriptionTextBox.Text = selectedRow["Description"].ToString();
                //startDateDatePicker.SelectedDate = (DateTime)selectedRow["StartDate"];
                endDateDatePicker.SelectedDate = selectedRow["EndDate"] as DateTime?;

/*                // Установка выбранных значений в ComboBox'ы
                int clientID = Convert.ToInt32(selectedRow["ClientID"]);
                clientComboBox.SelectedValue = clientID;

                int employeeID = Convert.ToInt32(selectedRow["EmployeeID"]);
                employeeComboBox.SelectedValue = employeeID;

                int serviceID = Convert.ToInt32(selectedRow["ServiceID"]);
                serviceComboBox.SelectedValue = serviceID;*/
            }
        }

    }
}
