using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Swimming_Shop_App
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ContentFrame.Navigate(new Pages.CatalogPage());
        }

        private void CatalogMenu_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(new Pages.CatalogPage());
        }

        private void CartMenu_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(new Pages.CartPage());
        }

        private void OrdersMenu_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(new Pages.OrdersPage());
        }

        private void ProfileMenu_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(new Pages.ProfilePage());
        }
    }
}