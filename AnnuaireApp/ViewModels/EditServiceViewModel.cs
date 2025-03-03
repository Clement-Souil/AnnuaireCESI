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
    public class EditServiceViewModel : BaseViewModel
    {
        private readonly HttpClient _httpClient;
        public ServiceDTO SelectedService { get; set; }

        public ICommand EditServiceCommand { get; }
        public ICommand CloseWindowCommand { get; }

        public static event Action? ServiceUpdated;

        public EditServiceViewModel(ServiceDTO service)
        {
            _httpClient = new HttpClient();
            SelectedService = service;

            EditServiceCommand = new RelayCommand(async _ => await ExecuteEditService());
            CloseWindowCommand = new RelayCommand(ExecuteCloseWindow);

            OnPropertyChanged(nameof(SelectedService));
        }

        private bool ModifyService()
        {
            var context = new ValidationContext(SelectedService, null, null);
            var results = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(SelectedService, context, results, true);

            if (!isValid)
            {
                string errors = string.Join("\n", results.Select(r => r.ErrorMessage));
                MessageBox.Show($"Erreur de validation :\n{errors}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);

            }

            return isValid;
        }

        private async Task ExecuteEditService()
        {
            if (!ModifyService()) return;
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"https://localhost:7212/api/service/{SelectedService.Id}", SelectedService);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Service modifié avec succès !", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                    ExecuteCloseWindow(null);
                    ServiceUpdated?.Invoke();
                }
                else
                {
                    MessageBox.Show("Erreur lors de la modification du service.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
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
                if (window is Views.EditServiceView)
                {
                    window.Close();
                    break;
                }
            }
        }
    }
}
