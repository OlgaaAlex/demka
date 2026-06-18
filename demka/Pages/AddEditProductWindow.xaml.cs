// demka/Pages/AddEditProductWindow.xaml.cs
using demka.Models;
using System;
using System.Linq;
using System.Windows;

namespace demka.Pages
{
    public partial class AddEditProductWindow : Window
    {
        private readonly AppDbContext _db = new AppDbContext();
        private Product _product;

        public AddEditProductWindow(Product product = null)
        {
            InitializeComponent();
            _product = product;

            LoadComboBoxes();

            if (_product != null)
            {
                Title = "Редактирование товара";
                TbArticul.IsEnabled = false;
                FillForm();
            }
            else
            {
                Title = "Добавление товара";
                TbArticul.IsEnabled = true;
                TbArticul.Focus();
            }
        }

        private void LoadComboBoxes()
        {
            CbSupplier.ItemsSource = _db.Suppliers.ToList();
            CbManufacturer.ItemsSource = _db.Manufacturers.ToList();
            CbCategory.ItemsSource = _db.Categories.ToList();
        }

        private void FillForm()
        {
            if (_product == null) return;

            TbArticul.Text = _product.ProductArticul;
            TbName.Text = _product.ProductName ?? "";
            TbUnit.Text = _product.ProductUnit ?? "пара";
            TbPrice.Text = _product.ProductPrice.ToString();

            // Исправлено для C# 7.3
            if (_product.Discount.HasValue)
                TbDiscount.Text = _product.Discount.Value.ToString();
            else
                TbDiscount.Text = "";

            if (_product.CountInStock.HasValue)
                TbCount.Text = _product.CountInStock.Value.ToString();
            else
                TbCount.Text = "";

            TbDescription.Text = _product.Description ?? "";

            // Заполнение комбобоксов
            if (_product.Supplier.HasValue)
                CbSupplier.SelectedItem = _db.Suppliers.Find(_product.Supplier.Value);

            if (_product.Manufacturer.HasValue)
                CbManufacturer.SelectedItem = _db.Manufacturers.Find(_product.Manufacturer.Value);

            if (_product.Category.HasValue)
                CbCategory.SelectedItem = _db.Categories.Find(_product.Category.Value);
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(TbArticul.Text))
                {
                    MessageBox.Show("Артикул обязателен!", "Ошибка");
                    TbArticul.Focus();
                    return;
                }

                if (_product == null)
                {
                    _product = new Product
                    {
                        ProductArticul = TbArticul.Text.Trim().ToUpper()
                    };
                    _db.Products.Add(_product);
                }

                _product.ProductName = TbName.Text.Trim();
                _product.ProductUnit = TbUnit.Text.Trim();

                if (!decimal.TryParse(TbPrice.Text, out decimal price))
                {
                    MessageBox.Show("Неверный формат цены!", "Ошибка");
                    return;
                }
                _product.ProductPrice = price;

                if (int.TryParse(TbDiscount.Text, out int discount))
                    _product.Discount = discount;
                else
                    _product.Discount = null;

                if (int.TryParse(TbCount.Text, out int count))
                    _product.CountInStock = count;
                else
                    _product.CountInStock = null;

                _product.Description = TbDescription.Text.Trim();

                if (CbSupplier.SelectedItem is Supplier sup)
                    _product.Supplier = sup.SupplierId;

                if (CbManufacturer.SelectedItem is Manufacturer man)
                    _product.Manufacturer = man.ManufacturerId;

                if (CbCategory.SelectedItem is Category cat)
                    _product.Category = cat.CategoryId;

                _db.SaveChanges();

                MessageBox.Show("Товар успешно сохранён!");
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }
    }
}