using System;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using AnnuaireApp.Helpers;
using AnnuaireLibrary.DTO;

namespace AnnuaireApp.ViewModels
{
    public class AddServiceViewModel : BaseViewModel
    {
        private readonly HttpClient _httpClient;
        public ServiceDTO NewService { get; set; } = new();

        public ICommand AddServiceCommand { get; }
        public ICommand CloseWindowCommand { get; }

        public static event Action? ServiceAdded;

        public AddServiceViewModel()
        {
            _httpClient = new HttpClient();
            AddServiceCommand = new RelayCommand(async _ => await ExecuteAddService());
            CloseWindowCommand = new RelayCommand(ExecuteCloseWindow);
        }

        private bool ValidateService()
        {
            var context = new ValidationContext(NewService, null, null);
            var results = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(NewService, context, results, true);

            if (!isValid)
            {
                string errors = string.Join("\n", results.Select(r => r.ErrorMessage));
                MessageBox.Show($"Erreur de validation :\n{errors}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);

            }

            return isValid;
        }

        private async Task ExecuteAddService()
        {
            if (!ValidateService()) return;
            try
            {
                // Vérification des champs avant l'envoi
                if (string.IsNullOrWhiteSpace(NewService.Nom))
                {
                    MessageBox.Show("Veuillez entrer un nom valide pour la ville.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                
                var response = await _httpClient.PostAsJsonAsync("https://localhost:7212/api/service", NewService);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Service ajouté avec succès !", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                    ExecuteCloseWindow(null);
                    ServiceAdded?.Invoke();
                }
                else
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Erreur lors de l'ajout du service : {errorMessage}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
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
                if (window is Views.AddServiceView)
                {
                    window.Close();
                    break;
                }
            }
        }
    }
}
