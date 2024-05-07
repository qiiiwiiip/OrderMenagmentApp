using System;
using System.Collections.Generic;
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
    /// Логика взаимодействия для AuthorizationWindow.xaml
    /// </summary>
    public partial class AuthorizationWindow : Window
    {
        private const string ConnectionString = @"Data Source=MYPC;Initial Catalog=OrderMenagmentDB;Integrated Security=True";

        public AuthorizationWindow()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e) // Кнопка входа
        {
            string login = UsernameTextBox.Text;
            string password = PasswordBox.Password;

            if (AuthenticateUser(login, password))
            {
                string status = GetUserStatus(login);

                if (status == "Admin")
                {
                    AdminWindow adminWindow = new AdminWindow();
                    adminWindow.Show();
                    Close(); // Закрываем окно авторизации
                }
                else if (status == "User")
                {
                    UserWindow userWindow = new UserWindow();
                    userWindow.Show();
                    Close(); // Закрываем окно авторизации
                }
            }
            else
            {
                MessageBox.Show("Неправильное имя пользователя или пароль!");
            }
        }

        private bool AuthenticateUser(string login, string password) // Проверка пользователя
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                string query = "SELECT COUNT(*) FROM Users WHERE Login = @Login AND Password = @Password";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Login", login);
                command.Parameters.AddWithValue("@Password", password);
                int count = (int)command.ExecuteScalar();
                return count > 0;
            }
        }

        private string GetUserStatus(string login) // Проверка статуса пользователя
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                string query = "SELECT s.Status FROM Users u JOIN Statuses s ON u.StatusID = s.StatusID WHERE u.Login = @Login";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Login", login);
                return (string)command.ExecuteScalar();
            }
        }
    }
}
