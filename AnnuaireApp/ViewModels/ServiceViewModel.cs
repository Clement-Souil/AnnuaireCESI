using System;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using AnnuaireApp.Helpers;
using AnnuaireLibrary.DTO;

namespace AnnuaireApp.ViewModels
{
    public class ServiceViewModel : BaseViewModel
    {
        private ObservableCollection<ServiceDTO> _services = new();
        public ObservableCollection<ServiceDTO> Services
        {
            get => _services;
            set
            {
                _services = value;
                OnPropertyChanged(nameof(Services));
            }
        }

        private readonly HttpClient _httpClient;

        // Commandes CRUD
        public ICommand AddServiceCommand { get; }
        public ICommand EditServiceCommand { get; }
        public ICommand DeleteServiceCommand { get; }

        public ServiceViewModel()
        {
            _httpClient = new HttpClient();
            LoadServices();

            // Écoute les modifications des services
            EditServiceViewModel.ServiceUpdated += LoadServices;

            // Initialisation des Command
            AddServiceCommand = new RelayCommand(ExecuteAddService);
            EditServiceCommand = new RelayCommand(ExecuteEditService);
            DeleteServiceCommand = new RelayCommand(ExecuteDeleteService);
        }

        private ServiceDTO _selectedService;
        public ServiceDTO SelectedService
        {
            get => _selectedService;
            set
            {
                _selectedService = value;
                OnPropertyChanged(nameof(SelectedService));
            }
        }

        private async void LoadServices()
        {
            try
            {
                var result = await _httpClient.GetFromJsonAsync<ObservableCollection<ServiceDTO>>("https://localhost:7212/api/service");
                if (result != null)
                {
                    Services.Clear();
                    foreach (var service in result)
                    {
                        Services.Add(service);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors du chargement des services : {ex.Message}");
            }
        }

        private void ExecuteAddService(object? parameter)
        {
            var addServiceView = new Views.AddServiceView
            {
                DataContext = new AddServiceViewModel()
            };
            addServiceView.ShowDialog();
            LoadServices(); 
        }


        private async void ExecuteEditService(object? parameter)
        {
            if (SelectedService == null)
            {
                MessageBox.Show("Veuillez sélectionner un service à modifier.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Vérifier si le service est utilisé par des employés
                var response = await _httpClient.GetAsync($"https://localhost:7212/api/employe/exists/{SelectedService.Id}");
                if (response.IsSuccessStatusCode)
                {
                    bool isUsed = await response.Content.ReadFromJsonAsync<bool>();
                    if (isUsed)
                    {
                        MessageBox.Show($"Impossible de modifier ce service car des employés y sont affectés.", "Modification refusée", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la vérification des employés : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Si tout est bon, ouvrir la fenêtre d'édition
            var editServiceView = new Views.EditServiceView
            {
                DataContext = new EditServiceViewModel(SelectedService)
            };
            editServiceView.ShowDialog();
            LoadServices(); // Mise à jour après modification
        }

        private async void ExecuteDeleteService(object? parameter)
        {
            if (SelectedService == null)
            {
                MessageBox.Show("Veuillez sélectionner un service à supprimer.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Vérifie si des employés sont rattachés au service
                var response = await _httpClient.GetAsync($"https://localhost:7212/api/employe/exists/{SelectedService.Id}");

                if (response.IsSuccessStatusCode)
                {
                    bool isUsed = await response.Content.ReadFromJsonAsync<bool>();
                    if (isUsed)
                    {
                        MessageBox.Show("Impossible de supprimer ce service car des employés y sont affectés.", "Suppression refusée", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }

                var confirm = MessageBox.Show($"Êtes-vous sûr de vouloir supprimer {SelectedService.Nom} ?",
                                              "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (confirm == MessageBoxResult.Yes)
                {
                    response = await _httpClient.DeleteAsync($"https://localhost:7212/api/service/{SelectedService.Id}");

                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Service supprimé avec succès !", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadServices(); 
                    }
                    else
                    {
                        MessageBox.Show("Erreur lors de la suppression du service.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
