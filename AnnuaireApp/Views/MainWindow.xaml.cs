using AnnuaireApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AnnuaireApp.Views
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel(); // Ca c'est que lie à MainViewModel pour que ca régisse au changements.

        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) && Keyboard.IsKeyDown(Key.LeftShift) && e.Key == Key.A && !AdminManager.IsAdmin)
            {
                var loginWindow = new LoginView();
                loginWindow.ShowDialog();
                UpdateAdminUI(); 
            }
        }


        private void UpdateAdminUI()
        {
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                if (mainWindow.MainFrame.Content is EmployeView employeView)
                {
                    employeView.UpdateAdminUI();
                }
                else if (mainWindow.MainFrame.Content is ServiceView serviceView)
                {
                    serviceView.UpdateAdminUI();
                }
                else if (mainWindow.MainFrame.Content is SiteView siteView)
                {
                    siteView.UpdateAdminUI();
                }
            }

            LogoutButton.Visibility = AdminManager.IsAdmin ? Visibility.Visible : Visibility.Collapsed;

        }


        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            AdminManager.Logout();
            MessageBox.Show("Déconnexion réussie !", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            UpdateAdminUI(); 
        }



    }
}
