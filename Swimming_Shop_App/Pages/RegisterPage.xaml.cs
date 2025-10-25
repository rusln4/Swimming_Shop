using System.Windows;
using System.Windows.Controls;
using System.Net.Http;
using System.Net.Http.Json;
using System;

namespace Swimming_Shop_App.Pages
{
    public partial class RegisterPage : Page
    {
        private readonly AuthWindow host;

        public RegisterPage(AuthWindow hostWindow)
        {
            InitializeComponent();
            host = hostWindow;
        }

        private async void Register_Click(object sender, RoutedEventArgs e)
        {
            var email = EmailTextBox.Text?.Trim();
            var password = PasswordBox.Password;
            var firstName = FirstNameTextBox.Text?.Trim();
            var lastName = LastNameTextBox.Text?.Trim();
            var phone = PhoneTextBox.Text?.Trim() ?? "";
            var address = AddressTextBox.Text?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
            {
                MessageBox.Show("Заполните обязательные поля: Email, Пароль, Имя, Фамилия", "Регистрация", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var handler = new HttpClientHandler { UseProxy = false, Proxy = null };
            using var http = new HttpClient(handler) { BaseAddress = new Uri(ApiConfig.BaseUrl) };

            try
            {
                var payload = new
                {
                    MailUser = email!,
                    PasswordUser = password!,
                    NameUser = firstName!,
                    LastnameUser = lastName!,
                    PhoneUser = phone,
                    AddressUser = address
                };

                var res = await http.PostAsJsonAsync("api/users/register", payload);
                if (res.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    MessageBox.Show("Email уже зарегистрирован", "Регистрация", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                res.EnsureSuccessStatusCode();

                MessageBox.Show("Регистрация выполнена. Войдите в аккаунт.", "Регистрация", MessageBoxButton.OK, MessageBoxImage.Information);
                host.NavigateToLogin();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сети: {ex.Message}", "Регистрация", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OpenLogin_Click(object sender, RoutedEventArgs e)
        {
            host.NavigateToLogin();
        }
    }
}