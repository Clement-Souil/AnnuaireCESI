using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using AnnuaireApp.Helpers;
using AnnuaireLibrary.DTO;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.VisualBasic;

namespace AnnuaireApp.ViewModels
{
    public class EditEmployeViewModel : BaseViewModel
    {
        private readonly HttpClient _httpClient;
        public EmployeDTO Employe { get; set; }

        public ObservableCollection<ServiceDTO> Services { get; set; } = new();
        public ObservableCollection<SiteDTO> Sites { get; set; } = new();

        private ServiceDTO _selectedService;
        public ServiceDTO SelectedService
        {
            get => _selectedService;
            set
            {
                _selectedService = value;
                Employe.ServiceId = value?.Id ?? 0;
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
                Employe.SiteId = value?.Id ?? 0;
                OnPropertyChanged(nameof(SelectedSite));
            }
        }

        public ICommand EditEmployeCommand { get; }
        public ICommand CloseWindowCommand { get; }

        public EditEmployeViewModel(EmployeDTO employe)
        {
            _httpClient = new HttpClient();
            Employe = employe;

            EditEmployeCommand = new RelayCommand(async _ => await ExecuteEditEmploye());
            CloseWindowCommand = new RelayCommand(ExecuteCloseWindow);

            LoadServicesAndSites();
        }

        private async void LoadServicesAndSites()
        {
            try
            {
                var servicesResult = await _httpClient.GetFromJsonAsync<ObservableCollection<ServiceDTO>>("https://localhost:7212/api/service");
                var sitesResult = await _httpClient.GetFromJsonAsync<ObservableCollection<SiteDTO>>("https://localhost:7212/api/site");

                if (servicesResult != null)
                    Services = servicesResult;
                if (sitesResult != null)
                    Sites = sitesResult;

                OnPropertyChanged(nameof(Services));
                OnPropertyChanged(nameof(Sites));

                // Associer le service et le site sélectionnés
                SelectedService = Services.FirstOrDefault(s => s.Id == Employe.ServiceId);
                SelectedSite = Sites.FirstOrDefault(s => s.Id == Employe.SiteId);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur de chargement des services/sites : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool ModifyEmploye()
        {
            var context = new ValidationContext(Employe, null, null);
            var results = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(Employe, context, results, true);

            if (!isValid)
            {
                string errors = string.Join("\n", results.Select(r => r.ErrorMessage));
                MessageBox.Show($"Erreur de validation :\n{errors}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);

            }

            return isValid;
        }


        private async Task ExecuteEditEmploye()
        {
            if (!ModifyEmploye()) return; // Annule la modification si validation échoue

            try
            {
                var response = await _httpClient.PutAsJsonAsync($"https://localhost:7212/api/employe/{Employe.Id}", Employe);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Employé modifié avec succès !", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                    ExecuteCloseWindow(null);
                }
                else
                {
                    MessageBox.Show("Erreur lors de la modification de l'employé.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
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
                if (window is Views.EditEmployeView)
                {
                    window.Close();
                    break;
                }
            }
        }
    }
}
