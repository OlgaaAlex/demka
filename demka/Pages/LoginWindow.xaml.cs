using demka.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Windows;

namespace demka.Pages
{
    public partial class LoginWindow : Window
    {
        private readonly AppDbContext _db = new AppDbContext();

        public LoginWindow()
        {
            InitializeComponent();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string login = TbLogin.Text.Trim();
            string password = PbPassword.Password;

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                TbError.Text = "Введите логин и пароль";
                return;
            }

            var user = _db.Users
                .Include(u => u.RoleNavigation)
                .FirstOrDefault(u => u.Login == login && u.Password == password);

            if (user == null)
            {
                TbError.Text = "Неверный логин или пароль";
                return;
            }

            OpenMainWindow(user);
        }

        private void Guest_Click(object sender, RoutedEventArgs e)
        {
            OpenMainWindow(null);
        }

        private void OpenMainWindow(User user)
        {
            MainWindow main = new MainWindow(user);
            main.Show();
            Close();
        }
    }
}