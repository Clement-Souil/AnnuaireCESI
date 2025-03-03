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
    public class EditSiteViewModel : BaseViewModel
    {
        private readonly HttpClient _httpClient;
        public static event Action? SiteUpdated; // Événement pour notifier SiteViewModel

        public SiteDTO Site { get; set; }

        public ICommand EditSiteCommand { get; }
        public ICommand CloseWindowCommand { get; }

        public EditSiteViewModel(SiteDTO site)
        {
            _httpClient = new HttpClient();
            _selectedSite = new SiteDTO
            {
                Id = site.Id,  // On s’assure que l’ID est bien copié
                Ville = site.Ville
            };

            EditSiteCommand = new RelayCommand(async _ => await ExecuteEditSite());
            CloseWindowCommand = new RelayCommand(ExecuteCloseWindow);

            OnPropertyChanged(nameof(SelectedSite));
        }



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

        private bool ModifySite()
        {
            var context = new ValidationContext(_selectedSite, null, null);
            var results = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(_selectedSite, context, results, true);

            if (!isValid)
            {
                string errors = string.Join("\n", results.Select(r => r.ErrorMessage));
                MessageBox.Show($"Erreur de validation :\n{errors}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);

            }

            return isValid;
        }

        private async Task ExecuteEditSite()
        {
            if (!ModifySite()) return;

            try
            {
                var response = await _httpClient.PutAsJsonAsync($"https://localhost:7212/api/site/{_selectedSite.Id}", _selectedSite);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Site modifié avec succès !", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Déclencher l'événement pour informer SiteViewModel
                    SiteUpdated?.Invoke();

                    ExecuteCloseWindow(null); // Fermer la fenêtre après édition
                }

                else
                {
                    MessageBox.Show("Erreur lors de la modification du site.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
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
                if (window is Views.EditSiteView)
                {
                    window.Close();
                    break;
                }
            }
        }
    }
}
