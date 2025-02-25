using AnnuaireApp.Helpers;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AnnuaireApp.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        // Commandes pour naviguer entre les vues
        public ICommand NavigateToEmploye { get; }
        public ICommand NavigateToService { get; }
        public ICommand NavigateToSite { get; }
        public ICommand NavigateToAdmin { get; }
        public ICommand LogoutCommand { get; }

        // Constructeur de la classe
        public MainViewModel()
        {
            // Liaison des commandes avec les méthodes de navigation
            NavigateToEmploye = new RelayCommand(_ => Navigate("Views/EmployeView.xaml"));
            NavigateToService = new RelayCommand(_ => Navigate("Views/ServiceView.xaml"));
            NavigateToSite = new RelayCommand(_ => Navigate("Views/SiteView.xaml"));
            NavigateToAdmin = new RelayCommand(_ => Navigate("Views/AdminView.xaml"));
            LogoutCommand = new RelayCommand(_ => Logout());
        }

        // Méthode pour changer dynamiquement la vue affichée
        private void Navigate(string view)
        {
            // Vérification de la fenêtre principale
            if (Application.Current.MainWindow.FindName("MainFrame") is Frame frame)
            {
                // Charge la nouvelle vue
                frame.Navigate(new Uri(view, UriKind.Relative));
            }
        }

        // Méthode pour gérer la déconnexion
        private void Logout()
        {
            // Ici, on pourra fermer la session et rediriger vers la connexion
            MessageBox.Show("Déconnexion réussie.");
        }
    }
}
