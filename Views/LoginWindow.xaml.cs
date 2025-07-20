using IUiUS_Projekat.Models;
using IUiUS_Projekat.Services;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace IUiUS_Projekat.Views
{
    public partial class LoginWindow : Window
    {
        private readonly List<User> users;

        public LoginWindow()
        {
            InitializeComponent();
            users = UserService.LoadUsers();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameBox.Text;
            string password = PasswordBox.Password;

            var user = users.FirstOrDefault(u => u.Username == username && u.Password == password);

            if (user == null)
            {
                MessageBox.Show("Pogrešno korisničko ime ili lozinka.", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var mainWindow = new MainWindow(user);
            mainWindow.Show();
            this.Close();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
