using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using AnnuaireApp.Helpers;
using AnnuaireLibrary.DTO;

namespace AnnuaireApp.ViewModels
{
    public class AddEmployeViewModel : BaseViewModel
    {
        private readonly HttpClient _httpClient;
        public EmployeDTO NewEmploye { get; set; } = new();

        // Listes déroulantes pour les services et sites
        public ObservableCollection<ServiceDTO> Services { get; set; } = new();
        public ObservableCollection<SiteDTO> Sites { get; set; } = new();

        // Sélections utilisateur
        private ServiceDTO _selectedService;
        public ServiceDTO SelectedService
        {
            get => _selectedService;
            set
            {
                _selectedService = value;
                NewEmploye.ServiceId = value?.Id ?? 0; // Mise à jour de l'ID en base
                OnPropertyChanged(nameof(SelectedService));
            }
        }

        private SiteDTO _selectedSite;
        public SiteDTO SelectedSite
        {
            get => _selectedSite;
            set
            {
                _selectedSite = value;
                NewEmploye.SiteId = value?.Id ?? 0; // Mise à jour de l'ID en base
                OnPropertyChanged(nameof(SelectedSite));
            }
        }

        // Commandes
        public ICommand AddEmployeCommand { get; }
        public ICommand CloseWindowCommand { get; }

        public AddEmployeViewModel()
        {
            _httpClient = new HttpClient();
            LoadServicesAndSites();

            AddEmployeCommand = new RelayCommand(ExecuteAddEmploye);
            CloseWindowCommand = new RelayCommand(ExecuteCloseWindow);
        }

        // Charger les services et sites depuis l'API
        private async void LoadServicesAndSites()
        {
            try
            {
                var servicesResult = await _httpClient.GetFromJsonAsync<ObservableCollection<ServiceDTO>>("https://localhost:7212/api/service");
                var sitesResult = await _httpClient.GetFromJsonAsync<ObservableCollection<SiteDTO>>("https://localhost:7212/api/site");

                if (servicesResult != null) Services = servicesResult;
                if (sitesResult != null) Sites = sitesResult;

                OnPropertyChanged(nameof(Services));
                OnPropertyChanged(nameof(Sites));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur de chargement des listes : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool ValidateEmploye()
        {
            var context = new ValidationContext(NewEmploye, null, null);
            var results = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(NewEmploye, context, results, true);

            if (!isValid)
            {
                string errors = string.Join("\n", results.Select(r => r.ErrorMessage));
                MessageBox.Show($"Erreur de validation :\n{errors}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);

            }

            return isValid;
        }

        private async void ExecuteAddEmploye(object? parameter)
        {
            if (!ValidateEmploye()) return; // Annule l'envoi si validation échoue

            try
            {

                var response = await _httpClient.PostAsJsonAsync("https://localhost:7212/api/employe", NewEmploye);
                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Employé ajouté avec succès !", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                    ExecuteCloseWindow(null);

                    if (Application.Current.Windows.OfType<Views.MainWindow>().FirstOrDefault()?.DataContext is EmployeViewModel employeVM)
                    {
                        MessageBox.Show("Rafraîchissement après ajout");
                        await employeVM.LoadEmployes();
                    }

                }
                else
                {
                    MessageBox.Show("Erreur lors de l'ajout de l'employé.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExecuteCloseWindow(object? parameter)
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window is Views.AddEmployeView)
                {
                    window.Close();
                    break;
                }
            }
        }
    }
}
