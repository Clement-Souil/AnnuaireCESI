using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Json;
using AnnuaireLibrary.DTO;

namespace AnnuaireApp.ViewModels
{
    public class SiteViewModel : BaseViewModel
    {
        public ObservableCollection<SiteDTO> Sites { get; set; } = new();
        private readonly HttpClient _httpClient;

        public SiteViewModel()
        {
            _httpClient = new HttpClient();
            LoadSites();
        }

        private async void LoadSites()
        {
            var result = await _httpClient.GetFromJsonAsync<ObservableCollection<SiteDTO>>("https://localhost:7212/api/site");
            if (result != null)
            {
                Sites = result;
                OnPropertyChanged(nameof(Sites));
            }
        }
    }
}
