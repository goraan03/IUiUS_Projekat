using IUiUS_Projekat.Services;
using System.Windows;

namespace IUiUS_Projekat
{
    public partial class App : System.Windows.Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            //UserFileGenerator.GenerateUsersFile();

            var loginWindow = new Views.LoginWindow();
            loginWindow.Show();
        }
    }
}
