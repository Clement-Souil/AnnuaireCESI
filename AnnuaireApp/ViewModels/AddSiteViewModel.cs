using System;
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
    public class AddSiteViewModel : BaseViewModel
    {
        private readonly HttpClient _httpClient;
        public SiteDTO NewSite { get; set; } = new();

        public ICommand AddSiteCommand { get; }
        public ICommand CloseWindowCommand { get; }

        public AddSiteViewModel()
        {
            _httpClient = new HttpClient();
            AddSiteCommand = new RelayCommand(async _ => await ExecuteAddSite());
            CloseWindowCommand = new RelayCommand(ExecuteCloseWindow);
        }

        private bool ValidateSite()
        {
            var context = new ValidationContext(NewSite, null, null);
            var results = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(NewSite, context, results, true);

            if (!isValid)
            {
                string errors = string.Join("\n", results.Select(r => r.ErrorMessage));
                MessageBox.Show($"Erreur de validation :\n{errors}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);

            }

            return isValid;
        }
        private async Task ExecuteAddSite()
        {
            if (!ValidateSite()) return;
            try
            {
                // Vérification des champs avant l'envoi
                if (string.IsNullOrWhiteSpace(NewSite.Ville))
                {
                    MessageBox.Show("Veuillez entrer un nom valide pour la ville.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Envoi de la requête POST à l’API
                var response = await _httpClient.PostAsJsonAsync("https://localhost:7212/api/site", NewSite);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Site ajouté avec succès !", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                    ExecuteCloseWindow(null);
                }
                else
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Erreur lors de l'ajout du site : {errorMessage}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
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
                if (window is Views.AddSiteView)
                {
                    window.Close();
                    break;
                }
            }
        }
    }
}
