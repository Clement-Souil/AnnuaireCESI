using AnnuaireApp.Helpers;
using System;
using System.Net.Http;
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

        private bool _isAdmin;
        public bool IsAdmin
        {
            get => _isAdmin;
            set
            {
                _isAdmin = value;
                OnPropertyChanged(nameof(IsAdmin));
            }
        }

        // Constructeur de la classe
        public MainViewModel()
        {
            // Liaison des commandes avec les méthodes de navigation
            NavigateToEmploye = new RelayCommand(_ => Navigate("Views/EmployeView.xaml"));
            NavigateToService = new RelayCommand(_ => Navigate("Views/ServiceView.xaml"));
            NavigateToSite = new RelayCommand(_ => Navigate("Views/SiteView.xaml"));
            LogoutCommand = new RelayCommand(ExecuteLogout);

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

        private async void ExecuteLogout(object? parameter)
        {
            try
            {
                using var httpClient = new HttpClient();
                var response = await httpClient.PostAsync("https://localhost:7212/api/auth/logout", null);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Déconnexion réussie !", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                    AdminManager.Logout(); // Réinitialise l'état admin
                }
                else
                {
                    MessageBox.Show("Erreur lors de la déconnexion.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
