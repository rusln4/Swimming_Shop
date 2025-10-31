using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Text.Json;
using System.Net.Http;
using System.Net.Http.Json;
using Swimming_Shop_App;

namespace Swimming_Shop_App.Pages
{
    public partial class ProfilePage : Page
    {
        // Убрано: private readonly ApiClient _api = new ApiClient();
        private UserItem? _currentUser;
        private int _currentUserId;

        public ProfilePage()
        {
            InitializeComponent();
            Loaded += async (s, e) => await LoadUserProfileAsync();
        }

        private HttpClient CreateHttp()
        {
            var handler = new HttpClientHandler { UseProxy = false, Proxy = null };
            return new HttpClient(handler) { BaseAddress = new Uri(ApiConfig.BaseUrl) };
        }

        private async Task LoadUserProfileAsync()
        {
            ShowLoading(true);
            try
            {
                _currentUserId = GetCurrentUserId();
                if (_currentUserId <= 0)
                {
                    MessageBox.Show("Пользователь не авторизован.", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                using var http = CreateHttp();
                _currentUser = await http.GetFromJsonAsync<UserItem>($"api/users/{_currentUserId}");
                if (_currentUser == null)
                {
                    MessageBox.Show("Не удалось загрузить данные профиля.", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                DisplayUserInfo();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки профиля: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                ShowLoading(false);
            }
        }

        private void DisplayUserInfo()
        {
            if (_currentUser == null) return;

            EmailText.Text = _currentUser.MailUser;
            PhoneText.Text = _currentUser.PhoneUser;
            RoleText.Text = GetRoleDisplayName(_currentUser.RoleUser);

            FirstNameBox.Text = _currentUser.NameUser;
            LastNameBox.Text = _currentUser.LastnameUser;
            AddressBox.Text = _currentUser.AddressUser;
            NewPasswordBox.Password = string.Empty;
        }

        private string GetRoleDisplayName(int roleId)
        {
            return roleId switch
            {
                1 => "Администратор",
                2 => "Пользователь",
                _ => "Неизвестная роль"
            };
        }

        private int GetCurrentUserId()
        {
            // Здесь должна быть логика получения ID текущего пользователя из сессии
            // Пока возвращаем тестовое значение
            // В реальном приложении это может быть из Properties.Settings или другого хранилища сессии
            
            // Попробуем получить из Application.Current.Properties
            if (Application.Current.Properties.Contains("CurrentUserId"))
            {
                if (int.TryParse(Application.Current.Properties["CurrentUserId"]?.ToString(), out int userId))
                {
                    return userId;
                }
            }
            
            // Если не найден, попробуем получить из сохраненной сессии входа
            // Это временное решение - в реальном приложении нужна более надежная система сессий
            return 1; // Временно возвращаем ID = 1 для тестирования
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentUser == null) return;

            if (string.IsNullOrWhiteSpace(FirstNameBox.Text))
            {
                MessageBox.Show("Имя не может быть пустым.", "Ошибка валидации",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                FirstNameBox.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(LastNameBox.Text))
            {
                MessageBox.Show("Фамилия не может быть пустой.", "Ошибка валидации",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                LastNameBox.Focus();
                return;
            }

            ShowLoading(true);
            try
            {
                using var http = CreateHttp();

                var payload = new
                {
                    IdUsers = _currentUserId,
                    NameUser = FirstNameBox.Text.Trim(),
                    LastnameUser = LastNameBox.Text.Trim(),
                    AddressUser = AddressBox.Text.Trim(),
                    PasswordUser = NewPasswordBox.Password
                };

                var res = await http.PutAsJsonAsync($"api/users/{_currentUserId}", payload);
                if (!res.IsSuccessStatusCode)
                {
                    var body = await res.Content.ReadAsStringAsync();
                    MessageBox.Show($"Код: {(int)res.StatusCode} {res.StatusCode}\n{body}",
                        "Ответ сервера", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                MessageBox.Show("Профиль успешно обновлен!", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                await LoadUserProfileAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                ShowLoading(false);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            // Восстанавливаем исходные значения
            DisplayUserInfo();
            MessageBox.Show("Изменения отменены.", "Информация", 
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            // Очистка сохранённых данных пользователя
            Application.Current.Properties.Remove("CurrentUserId");
            Application.Current.Properties.Remove("CurrentUserEmail");
            Application.Current.Properties.Remove("CurrentUserName");
            Application.Current.Properties.Remove("CurrentUserLastName");
            Application.Current.Properties.Remove("CurrentUserPhone");
            Application.Current.Properties.Remove("CurrentUserAddress");
            Application.Current.Properties.Remove("CurrentUserRole");

            // Переход на окно авторизации
            var auth = new AuthWindow();
            auth.Show();

            // Закрываем текущее главное окно
            var main = Window.GetWindow(this);
            main?.Close();
        }

        private void ShowLoading(bool isLoading)
        {
            LoadingOverlay.Visibility = isLoading ? Visibility.Visible : Visibility.Collapsed;
            
            // Блокируем/разблокируем элементы управления
            FirstNameBox.IsEnabled = !isLoading;
            LastNameBox.IsEnabled = !isLoading;
            AddressBox.IsEnabled = !isLoading;
            NewPasswordBox.IsEnabled = !isLoading;
            SaveButton.IsEnabled = !isLoading;
            CancelButton.IsEnabled = !isLoading;
        }
    }
}