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

        // Constructeur
        public EmployeViewModel()
        {
            _httpClient = new HttpClient();
            LoadEmployes();

            // Initialisation des commandes (elles ne font rien pour l’instant)
            AddEmployeCommand = new RelayCommand(ExecuteAddEmploye);
            EditEmployeCommand = new RelayCommand(ExecuteEditEmploye);
            DeleteEmployeCommand = new RelayCommand(ExecuteDeleteEmploye);
        }

        // Chargement des employés
        private async void LoadEmployes()
        {
            try
            {
                var result = await _httpClient.GetFromJsonAsync<ObservableCollection<EmployeDTO>>("https://localhost:7212/api/employe");
                if (result != null)
                {
                    Employes.Clear();
                    foreach (var employe in result)
                    {
                        Employes.Add(employe);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur de chargement des employés : {ex.Message}");
            }
        }

        // Méthodes des boutons (pour l’instant vides)
        private void ExecuteAddEmploye(object? parameter)
        {
            var addEmployeView = new Views.AddEmployeView
            {
                DataContext = new AddEmployeViewModel()
            };
            addEmployeView.ShowDialog();
            LoadEmployes(); // Rafraîchir la liste après l'ajout
        }

        private void ExecuteEditEmploye(object? parameter)
        {
            MessageBox.Show("Modification employé");
        }

        private void ExecuteDeleteEmploye(object? parameter)
        {
            MessageBox.Show("Suppression employé");
        }

    }
}
