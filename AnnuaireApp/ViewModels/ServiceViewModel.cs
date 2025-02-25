using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Json;
using AnnuaireLibrary.DTO;

namespace AnnuaireApp.ViewModels
{
    public class ServiceViewModel : BaseViewModel
    {
        public ObservableCollection<ServiceDTO> Services { get; set; } = new();
        private readonly HttpClient _httpClient;

        public ServiceViewModel()
        {
            _httpClient = new HttpClient();
            LoadServices();
        }

        private async void LoadServices()
        {
            var result = await _httpClient.GetFromJsonAsync<ObservableCollection<ServiceDTO>>("https://localhost:7212/api/service");
            if (result != null)
            {
                Services = result;
                OnPropertyChanged(nameof(Services));
            }
        }
    }
}
