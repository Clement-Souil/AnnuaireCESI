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

        private SiteDTO _selectedSite;
        public SiteDTO SelectedSite
        {
            get => _selectedSite;
            set
            {
                _selectedSite = value;
                OnPropertyChanged(nameof(SelectedSite));
            }
        }

        public SiteViewModel()
        {
            _httpClient = new HttpClient();
            LoadSites();

            // Écoute les modifications des sites
            EditSiteViewModel.SiteUpdated += LoadSites;

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
            var addSiteView = new Views.AddSiteView
            {
                DataContext = new AddSiteViewModel()
            };
            addSiteView.ShowDialog();
            LoadSites(); 
        }


        private async void ExecuteEditSite(object? parameter)
        {
            if (SelectedSite == null)
            {
                MessageBox.Show("Veuillez sélectionner un site à modifier.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Vérifier si le site est utilisé par des employés
                var response = await _httpClient.GetAsync($"https://localhost:7212/api/employe/exists/{SelectedSite.Id}");
                if (response.IsSuccessStatusCode)
                {
                    bool isUsed = await response.Content.ReadFromJsonAsync<bool>();
                    if (isUsed)
                    {
                        MessageBox.Show($"Impossible de modifier ce site car des employés y sont affectés.", "Modification refusée", MessageBoxButton.OK, MessageBoxImage.Warning);
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
            var editSiteView = new Views.EditSiteView
            {
                DataContext = new EditSiteViewModel(SelectedSite)
            };
            editSiteView.ShowDialog();
            LoadSites(); // Mise à jour après modification
        }



        private async void ExecuteDeleteSite(object? parameter) 
        {
            if (SelectedSite == null)
            {
                MessageBox.Show("Veuillez sélectionner un site à supprimer.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show($"Êtes-vous sûr de vouloir supprimer {SelectedSite.Ville} ?",
                                         "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var response = await _httpClient.DeleteAsync($"https://localhost:7212/api/site/{SelectedSite.Id}");

                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Site supprimé avec succès !", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadSites(); //  Mise à jour après suppression
                    }
                    else
                    {
                        string errorMessage = await response.Content.ReadAsStringAsync();
                        MessageBox.Show($"Erreur lors de la suppression du site : {errorMessage}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erreur : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }



    }
}
