using System.Windows;
using System.Windows.Controls;
using System.Text.Json;
using System.Net.Http;
using System.Net.Http.Json;
using System;

namespace Swimming_Shop_App.Pages
{
    public partial class LoginPage : Page
    {
        private readonly AuthWindow host;

        public LoginPage(AuthWindow hostWindow)
        {
            InitializeComponent();
            host = hostWindow;
        }

        private void OpenRegister_Click(object sender, RoutedEventArgs e)
        {
            host.NavigateToRegister();
        }

        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            var email = EmailTextBox.Text?.Trim();
            var password = PasswordBox.Password;

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Введите email и пароль", "Авторизация", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var handler = new HttpClientHandler { UseProxy = false, Proxy = null };
            using var http = new HttpClient(handler) { BaseAddress = new Uri(ApiConfig.BaseUrl) };

            try
            {
                var payload = new { MailUser = email!, PasswordUser = password! };
                var res = await http.PostAsJsonAsync("api/users/login", payload);

                if (res.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    MessageBox.Show("Неверный email или пароль", "Авторизация", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                res.EnsureSuccessStatusCode();

                var json = await res.Content.ReadAsStringAsync();
                using (var document = JsonDocument.Parse(json))
                {
                    var root = document.RootElement;

                    Application.Current.Properties["CurrentUserId"] = root.GetProperty("idUsers").GetInt32();
                    Application.Current.Properties["CurrentUserEmail"] = root.GetProperty("mailUser").GetString();
                    Application.Current.Properties["CurrentUserName"] = root.GetProperty("nameUser").GetString();
                    Application.Current.Properties["CurrentUserLastName"] = root.GetProperty("lastnameUser").GetString();
                    Application.Current.Properties["CurrentUserPhone"] = root.GetProperty("phoneUser").GetString();
                    Application.Current.Properties["CurrentUserAddress"] = root.GetProperty("addressUser").GetString();
                    Application.Current.Properties["CurrentUserRole"] = root.GetProperty("roleUser").GetInt32();
                }

                var main = new MainWindow();
                main.Show();
                host.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сети: {ex.Message}", "Авторизация", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}