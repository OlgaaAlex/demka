using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using demka.Models;
using Microsoft.EntityFrameworkCore;

namespace demka.Pages
{
    public partial class OrderWindow : Window
    {
        private readonly AppDbContext _db = new AppDbContext();
        private readonly User _currentUser;

        public OrderWindow(User user)
        {
            InitializeComponent();
            _currentUser = user;
            SetupRolePermissions();
            LoadOrders();
        }

        private void SetupRolePermissions()
        {
            bool isAdmin = _currentUser?.RoleNavigation?.RoleName?.ToLower().Contains("админ") == true ||
                           _currentUser?.RoleNavigation?.RoleName?.ToLower().Contains("admin") == true;

            BtnAdd.Visibility = isAdmin ? Visibility.Visible : Visibility.Collapsed;
            BtnEdit.Visibility = isAdmin ? Visibility.Visible : Visibility.Collapsed;
            BtnDelete.Visibility = isAdmin ? Visibility.Visible : Visibility.Collapsed;
        }

        private void LoadOrders()
        {
            var orders = _db.Orders
                .Include(o => o.StatusNavigation)
                .Include(o => o.AddressNavigation)
                .Include(o => o.UserNavigation)
                .ToList();

            OrdersGrid.ItemsSource = orders;
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LoadOrders();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            var wnd = new AddEditOrderWindow(_currentUser);  
            if (wnd.ShowDialog() == true)
                LoadOrders();
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            var selected = OrdersGrid.SelectedItem as Order;
            if (selected == null)
            {
                MessageBox.Show("Выберите заказ для редактирования", "Внимание");
                return;
            }

            var wnd = new AddEditOrderWindow(_currentUser, selected);   // ← передаём пользователя
            if (wnd.ShowDialog() == true)
                LoadOrders();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            var selected = OrdersGrid.SelectedItem as Order;
            if (selected == null) return;

            if (MessageBox.Show("Удалить выбранный заказ?\n\nВсе товары в заказе также будут удалены.",
                    "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                try
                {
                    // Удаляем связанные товары
                    var orderProducts = _db.OrderProducts.Where(op => op.OrderId == selected.OrderId);
                    _db.OrderProducts.RemoveRange(orderProducts);

                    _db.Orders.Remove(selected);
                    _db.SaveChanges();

                    LoadOrders();
                    MessageBox.Show("Заказ успешно удалён");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при удалении: " + ex.Message);
                }
            }
        }

        private void OrdersGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (_currentUser?.RoleNavigation?.RoleName?.ToLower().Contains("админ") == true ||
                _currentUser?.RoleNavigation?.RoleName?.ToLower().Contains("admin") == true)
            {
                Edit_Click(null, null);
            }
        }
    }
}