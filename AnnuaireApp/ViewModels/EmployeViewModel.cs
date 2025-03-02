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
    public class EmployeViewModel : BaseViewModel
    {
        // Liste observable des employés
        private ObservableCollection<EmployeDTO> _employes = new();
        public ObservableCollection<EmployeDTO> Employes
        {
            get => _employes;
            set
            {
                _employes = value;
                OnPropertyChanged(nameof(Employes));
            }
        }

        private readonly HttpClient _httpClient;

        // Commandes pour les boutons
        public ICommand AddEmployeCommand { get; }
        public ICommand EditEmployeCommand { get; }
        public ICommand DeleteEmployeCommand { get; }

        // Listes des services et sites
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

        private EmployeDTO _selectedEmploye;
        public EmployeDTO SelectedEmploye
        {
            get => _selectedEmploye;
            set
            {
                _selectedEmploye = value;
                OnPropertyChanged(nameof(SelectedEmploye));
            }
        }

        // Constructeur
        public EmployeViewModel()
        {
            _httpClient = new HttpClient();
            LoadData(); //  On lance le chargement de manière propre

            // Initialisation des commandes 
            AddEmployeCommand = new RelayCommand(ExecuteAddEmploye);
            EditEmployeCommand = new RelayCommand(_ =>
            {
                if (SelectedEmploye != null)
                {
                    var editEmployeView = new Views.EditEmployeView
                    {
                        DataContext = new EditEmployeViewModel(SelectedEmploye)
                    };
                    editEmployeView.ShowDialog();
                    LoadEmployes();
                }
                else
                {
                    MessageBox.Show("Veuillez sélectionner un employé avant de modifier.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            });

            DeleteEmployeCommand = new RelayCommand(ExecuteDeleteEmploye);
        }

        //  Nouvelle méthode pour garantir que Services et Sites sont bien chargés avant les employés
        private async void LoadData()
        {
            await LoadServicesAndSites();
            await LoadEmployes();
        }

        private async Task LoadServicesAndSites()
        {
            try
            {
                var servicesResult = await _httpClient.GetFromJsonAsync<ObservableCollection<ServiceDTO>>("https://localhost:7212/api/service");
                var sitesResult = await _httpClient.GetFromJsonAsync<ObservableCollection<SiteDTO>>("https://localhost:7212/api/site");

                if (servicesResult != null)
                {
                    Services.Clear();
                    foreach (var service in servicesResult)
                        Services.Add(service);
                }

                if (sitesResult != null)
                {
                    Sites.Clear();
                    foreach (var site in sitesResult)
                        Sites.Add(site);
                }

                //  Vérification après chargement
                if (Services.Count == 0 || Sites.Count == 0)
                {
                    MessageBox.Show(" Problème : Services ou Sites non chargés !", "Erreur de chargement", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur de chargement des services/sites : {ex.Message}",
                                "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public async Task LoadEmployes()
        {
            try
            {
                //  Vérification avant de charger les employés
                if (Services.Count == 0 || Sites.Count == 0)
                {
                    MessageBox.Show(" Impossible de charger les employés : Services et Sites non chargés !",
                                    "Erreur de dépendance", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var result = await _httpClient.GetFromJsonAsync<ObservableCollection<EmployeDTO>>("https://localhost:7212/api/employe");
                if (result != null)
                {
                    Employes.Clear();
                    foreach (var employe in result)
                    {
                        var service = Services.FirstOrDefault(s => s.Id == employe.ServiceId);
                        var site = Sites.FirstOrDefault(s => s.Id == employe.SiteId);

                        employe.Service = service?.Nom ?? "Service introuvable";
                        employe.Site = site?.Ville ?? "Site introuvable";

                        Employes.Add(employe);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur de chargement des employés : {ex.Message}",
                                "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Méthodes des boutons
        private void ExecuteAddEmploye(object? parameter)
        {
            var addEmployeView = new Views.AddEmployeView
            {
                DataContext = new AddEmployeViewModel()
            };
            addEmployeView.ShowDialog();
            LoadEmployes(); // Rafraîchir la liste après l'ajout
        }


        private async void ExecuteDeleteEmploye(object? parameter)
        {
            if (SelectedEmploye == null)
            {
                MessageBox.Show("Veuillez sélectionner un employé à supprimer.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show($"Êtes-vous sûr de vouloir supprimer {SelectedEmploye.Nom} {SelectedEmploye.Prenom} ?",
                                         "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var response = await _httpClient.DeleteAsync($"https://localhost:7212/api/employe/{SelectedEmploye.Id}");

                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Employé supprimé avec succès !", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadEmployes(); // 🔄 Met à jour la liste après suppression
                    }
                    else
                    {
                        string errorMessage = await response.Content.ReadAsStringAsync();
                        MessageBox.Show($"Erreur lors de la suppression de l'employé : {errorMessage}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
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
