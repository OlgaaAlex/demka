// demka/Pages/ProductWindow.xaml.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using demka.Models;
using Microsoft.EntityFrameworkCore;

namespace demka.Pages
{
    public partial class ProductWindow : Window
    {
        private readonly AppDbContext _db = new AppDbContext();
        private readonly User _currentUser;

        public ProductWindow(User user)
        {
            InitializeComponent();
            _currentUser = user;
            LoadData();
            SetupRolePermissions();
        }

        private void SetupRolePermissions()
        {
            bool isAdmin = _currentUser?.RoleNavigation?.RoleName?.ToLower().Contains("админ") == true ||
                           _currentUser?.RoleNavigation?.RoleName?.ToLower().Contains("admin") == true;

            bool isManager = _currentUser?.RoleNavigation?.RoleName?.ToLower().Contains("менеджер") == true ||
                             _currentUser?.RoleNavigation?.RoleName?.ToLower().Contains("manager") == true;

            BtnAdd.Visibility = isAdmin ? Visibility.Visible : Visibility.Collapsed;
            BtnEdit.Visibility = isAdmin ? Visibility.Visible : Visibility.Collapsed;
            BtnDelete.Visibility = isAdmin ? Visibility.Visible : Visibility.Collapsed;
        }

        private void LoadData()
        {
            var products = _db.Products
                .Include(p => p.CategoryNavigation)
                .Include(p => p.ManufacturerNavigation)
                .Include(p => p.SupplierNavigation)
                .ToList();

            ProductsGrid.ItemsSource = products;   // ← исправлено
            LoadFilters();
        }

        private void LoadFilters()
        {
            CbCategory.ItemsSource = _db.Categories.ToList();
            CbManufacturer.ItemsSource = _db.Manufacturers.ToList();
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        private void ResetFilters_Click(object sender, RoutedEventArgs e)
        {
            TbSearch.Clear();
            CbCategory.SelectedIndex = -1;
            CbManufacturer.SelectedIndex = -1;
            CbSort.SelectedIndex = -1;
            LoadData();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            var wnd = new AddEditProductWindow();
            if (wnd.ShowDialog() == true)
                LoadData();
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            var selected = ProductsGrid.SelectedItem as Product;
            if (selected == null)
            {
                MessageBox.Show("Выберите товар для редактирования", "Внимание");
                return;
            }

            var wnd = new AddEditProductWindow(selected);
            if (wnd.ShowDialog() == true)
                LoadData();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            var selected = ProductsGrid.SelectedItem as Product;
            if (selected == null) return;

            if (MessageBox.Show("Удалить выбранный товар?", "Подтверждение",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    _db.Products.Remove(selected);
                    _db.SaveChanges();
                    LoadData();
                    MessageBox.Show("Товар удалён");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при удалении: " + ex.Message);
                }
            }
        }

        private void TbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void Filter_Changed(object sender, RoutedEventArgs e)
        {
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            var query = _db.Products
                .Include(p => p.CategoryNavigation)
                .Include(p => p.ManufacturerNavigation)
                .Include(p => p.SupplierNavigation)
                .AsQueryable();

            string search = TbSearch.Text?.ToLower() ?? "";

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p =>
                    (p.ProductName != null && p.ProductName.ToLower().Contains(search)) ||
                    (p.ProductArticul != null && p.ProductArticul.ToLower().Contains(search)));
            }

            if (CbCategory.SelectedItem is Category cat && cat != null)
                query = query.Where(p => p.Category == cat.CategoryId);

            if (CbManufacturer.SelectedItem is Manufacturer man && man != null)
                query = query.Where(p => p.Manufacturer == man.ManufacturerId);

            var products = query.ToList();

            // Сортировка
            if (CbSort.SelectedIndex == 0)
                products = products.OrderBy(p => p.ProductPrice).ToList();
            else if (CbSort.SelectedIndex == 1)
                products = products.OrderByDescending(p => p.ProductPrice).ToList();
            else if (CbSort.SelectedIndex == 2)
                products = products.OrderBy(p => p.ProductName).ToList();

            ProductsGrid.ItemsSource = products;
        }
    }
}