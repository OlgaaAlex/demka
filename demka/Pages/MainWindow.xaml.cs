// demka/MainWindow.xaml.cs
using System.Windows;
using demka.Models;
using demka.Pages;

namespace demka
{
    public partial class MainWindow : Window
    {
        private readonly User _currentUser;

        public MainWindow(User user)
        {
            InitializeComponent();
            _currentUser = user;

            if (user == null)
            {
                TbGreeting.Text = "Вы вошли как Гость";
                BtnProducts.Visibility = Visibility.Visible;
                // Guest can only view products (no advanced features)
            }
            else
            {
                TbGreeting.Text = $"Добро пожаловать, {user.FirstName} {user.LastName} ({user.RoleNavigation?.RoleName})";

                if (user.RoleNavigation?.RoleName?.ToLower().Contains("админ") == true ||
                    user.RoleNavigation?.RoleName?.ToLower().Contains("admin") == true)
                {
                    BtnProducts.Visibility = Visibility.Visible;
                    BtnOrders.Visibility = Visibility.Visible;
                    BtnAddProduct.Visibility = Visibility.Visible;
                }
                else if (user.RoleNavigation?.RoleName?.ToLower().Contains("менеджер") == true ||
                         user.RoleNavigation?.RoleName?.ToLower().Contains("manager") == true)
                {
                    BtnProducts.Visibility = Visibility.Visible;
                    BtnOrders.Visibility = Visibility.Visible;
                }
                else // Client
                {
                    BtnProducts.Visibility = Visibility.Visible;
                }
            }
        }

        private void Products_Click(object sender, RoutedEventArgs e)
        {
            ProductWindow wnd = new ProductWindow(_currentUser);
            wnd.ShowDialog();
        }

        private void Orders_Click(object sender, RoutedEventArgs e)
        {
            OrderWindow wnd = new OrderWindow(_currentUser);
            wnd.ShowDialog();
        }

        private void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            AddEditProductWindow wnd = new AddEditProductWindow();
            wnd.ShowDialog();
        }
    }
}