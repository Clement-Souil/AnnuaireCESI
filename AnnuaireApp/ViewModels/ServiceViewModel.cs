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

            // Initialisation des Command
            AddServiceCommand = new RelayCommand(ExecuteAddService);
            EditServiceCommand = new RelayCommand(ExecuteEditService);
            DeleteServiceCommand = new RelayCommand(ExecuteDeleteService);
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
            MessageBox.Show("Ajout d'un service !", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ExecuteEditService(object? parameter)
        {
            MessageBox.Show("Modification d'un service !", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ExecuteDeleteService(object? parameter)
        {
            MessageBox.Show("Suppression d'un service !", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
