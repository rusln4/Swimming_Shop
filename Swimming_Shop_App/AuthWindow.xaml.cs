using System.Windows;
using System.Windows.Navigation;

namespace Swimming_Shop_App
{
    public partial class AuthWindow : Window
    {
        public AuthWindow()
        {
            InitializeComponent();
            NavigateToLogin();
        }

        public void NavigateToLogin()
        {
            ContentFrame.Navigate(new Pages.LoginPage(this));
        }

        public void NavigateToRegister()
        {
            ContentFrame.Navigate(new Pages.RegisterPage(this));
        }
    }
}