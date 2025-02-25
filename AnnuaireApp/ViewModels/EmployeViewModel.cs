using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AnnuaireLibrary.DTO;

namespace AnnuaireApp.ViewModels
{
    public class EmployeViewModel : BaseViewModel
    {
        // Liste observable des employés
        public ObservableCollection<EmployeDTO> Employes { get; set; } = new();

        private readonly HttpClient _httpClient;

        // Constructeur
        public EmployeViewModel()
        {
            _httpClient = new HttpClient();
            LoadEmployes();
        }

        // Méthode pour récupérer les employés via l'API
        private async void LoadEmployes()
        {
            var result = await _httpClient.GetFromJsonAsync<ObservableCollection<EmployeDTO>>("https://localhost:7212/api/employe");
            if (result != null)
            {
                Employes = result;
                OnPropertyChanged(nameof(Employes));
            }
        }
    }
}
