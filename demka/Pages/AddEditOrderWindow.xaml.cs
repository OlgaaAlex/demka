using demka.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace demka.Pages
{
    public partial class AddEditOrderWindow : Window
    {
        private readonly AppDbContext _db = new AppDbContext();
        private readonly User _currentUser;        // ← Новый
        private Order _order;
        private List<OrderProduct> _orderProducts = new List<OrderProduct>();

        // Обновлённый конструктор
        public AddEditOrderWindow(User currentUser, Order order = null)
        {
            InitializeComponent();
            _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
            _order = order;
            LoadData();
        }

        private void LoadData()
        {
            CbStatus.ItemsSource = _db.Statuses.ToList();
            CbAddress.ItemsSource = _db.Addresses.ToList();

            if (_order != null)
            {
                Title = $"Редактирование заказа №{_order.OrderId}";

                if (_order.OrderStatus.HasValue)
                    CbStatus.SelectedItem = _db.Statuses.Find(_order.OrderStatus.Value);

                if (_order.OrderAddress.HasValue)
                    CbAddress.SelectedItem = _db.Addresses.Find(_order.OrderAddress.Value);

                DpOrderDate.SelectedDate = _order.OrderDate;
                DpOrderDateIssue.SelectedDate = _order.OrderDateIssue;
                TbOrderCode.Text = _order.OrderCode?.ToString() ?? "";

                _orderProducts = _db.OrderProducts
                    .Include(op => op.ProductNavigation)
                    .Where(op => op.OrderId == _order.OrderId)
                    .ToList();
            }
            else
            {
                Title = "Новый заказ";
                DpOrderDate.SelectedDate = DateTime.Now;
            }

            RefreshOrderProductsGrid();
            UpdateTotalSum();
        }

        private void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            string articul = TbArticul.Text?.Trim().ToUpper();
            if (string.IsNullOrEmpty(articul))
            {
                MessageBox.Show("Введите артикул товара", "Внимание");
                return;
            }
            if (!int.TryParse(TbQuantity.Text, out int qty) || qty <= 0)
            {
                MessageBox.Show("Укажите корректное количество", "Ошибка");
                return;
            }

            var product = _db.Products.FirstOrDefault(p => p.ProductArticul == articul);
            if (product == null)
            {
                MessageBox.Show("Товар с таким артикулом не найден", "Ошибка");
                return;
            }

            var existing = _orderProducts.FirstOrDefault(op => op.Product == articul);
            if (existing != null)
            {
                existing.OrderProductCount = qty;
            }
            else
            {
                _orderProducts.Add(new OrderProduct
                {
                    Product = articul,
                    OrderProductCount = qty,
                    ProductNavigation = product
                });
            }

            RefreshOrderProductsGrid();
            UpdateTotalSum();
            TbArticul.Clear();
            TbQuantity.Text = "1";
        }

        private void RemoveProduct_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is OrderProduct item)
            {
                _orderProducts.Remove(item);
                RefreshOrderProductsGrid();
                UpdateTotalSum();
            }
        }

        private void RefreshOrderProductsGrid()
        {
            OrderProductsGrid.ItemsSource = null;
            OrderProductsGrid.ItemsSource = _orderProducts;
        }

        private void UpdateTotalSum()
        {
            decimal total = _orderProducts.Sum(op =>
                (op.ProductNavigation?.ProductPrice ?? 0) * (op.OrderProductCount ?? 0));
            TbTotalSum.Text = total.ToString("N2") + " ₽";
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CbStatus.SelectedItem == null || CbAddress.SelectedItem == null)
                {
                    MessageBox.Show("Выберите статус и адрес выдачи!", "Ошибка");
                    return;
                }

                if (_order == null)
                {
                    // === НОВЫЙ ЗАКАЗ ===
                    _order = new Order
                    {
                        OrderDate = DpOrderDate.SelectedDate ?? DateTime.Now,
                        OrderUser = _currentUser.Userid   // ← Главное изменение!
                    };
                    _db.Orders.Add(_order);
                    _db.SaveChanges();
                }

                // Обновляем данные заказа
                _order.OrderStatus = (CbStatus.SelectedItem as Status)?.StatusId;
                _order.OrderAddress = (CbAddress.SelectedItem as Address)?.AddressId;
                _order.OrderDate = DpOrderDate.SelectedDate;
                _order.OrderDateIssue = DpOrderDateIssue.SelectedDate;

                if (int.TryParse(TbOrderCode.Text, out int code))
                    _order.OrderCode = code;

                // Товары
                var existing = _db.OrderProducts
                                  .Where(op => op.OrderId == _order.OrderId)
                                  .ToList();
                _db.OrderProducts.RemoveRange(existing);

                foreach (var item in _orderProducts)
                {
                    var newOp = new OrderProduct
                    {
                        OrderId = _order.OrderId,
                        Product = item.Product,
                        OrderProductCount = item.OrderProductCount
                    };
                    _db.OrderProducts.Add(newOp);
                }

                _db.SaveChanges();

                MessageBox.Show("Заказ успешно сохранён!", "Успех",
                               MessageBoxButton.OK, MessageBoxImage.Information);

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                string msg = ex.InnerException?.Message ?? ex.Message;
                MessageBox.Show($"Ошибка при сохранении:\n{msg}", "Ошибка",
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Двойной клик по товару (для удобства редактирования)
        private void OrderProductsGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (OrderProductsGrid.SelectedItem is OrderProduct selected)
            {
                TbArticul.Text = selected.Product;
                TbQuantity.Text = (selected.OrderProductCount ?? 1).ToString();
            }
        }
    }
}