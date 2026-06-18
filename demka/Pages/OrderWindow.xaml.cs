// demka/Pages/OrderWindow.xaml.cs
using System.Linq;
using System.Windows;
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
            LoadOrders();
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
    }
}