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
    public class SiteViewModel : BaseViewModel
    {
        private ObservableCollection<SiteDTO> _sites = new();
        public ObservableCollection<SiteDTO> Sites
        {
            get => _sites;
            set
            {
                _sites = value;
                OnPropertyChanged(nameof(Sites));
            }
        }

        private readonly HttpClient _httpClient;

        // Commandes CRUD
        public ICommand AddSiteCommand { get; }
        public ICommand EditSiteCommand { get; }
        public ICommand DeleteSiteCommand { get; }

        public SiteViewModel()
        {
            _httpClient = new HttpClient();
            LoadSites();

            // Initialisation des Command
            AddSiteCommand = new RelayCommand(ExecuteAddSite);
            EditSiteCommand = new RelayCommand(ExecuteEditSite);
            DeleteSiteCommand = new RelayCommand(ExecuteDeleteSite);
        }

        private async void LoadSites()
        {
            try
            {
                var result = await _httpClient.GetFromJsonAsync<ObservableCollection<SiteDTO>>("https://localhost:7212/api/site");
                if (result != null)
                {
                    Sites.Clear();
                    foreach (var site in result)
                    {
                        Sites.Add(site);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors du chargement des sites : {ex.Message}");
            }
        }

        private void ExecuteAddSite(object? parameter)
        {
            MessageBox.Show("Ajout d'un site !", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ExecuteEditSite(object? parameter)
        {
            MessageBox.Show("Modification d'un site !", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ExecuteDeleteSite(object? parameter)
        {
            MessageBox.Show("Suppression d'un site !", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
