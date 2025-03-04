using System.Net.Http;
using System.Net.Http.Json;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using AnnuaireApp.Helpers;

namespace AnnuaireApp.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly HttpClient _httpClient;
        private string _email;
        private string _password;

        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged(nameof(Email));
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }

        public ICommand LoginCommand { get; }
        public ICommand CloseCommand { get; }

        public LoginViewModel()
        {
            _httpClient = new HttpClient();
            LoginCommand = new RelayCommand(async _ => await ExecuteLogin());
            CloseCommand = new RelayCommand(_ => ExecuteClose(null));
        }

        private async Task ExecuteLogin()
        {
            var loginRequest = new { Email, Password };

            try
            {
                var response = await _httpClient.PostAsJsonAsync("https://localhost:7212/api/auth/login", loginRequest);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<LoginResponse>();

                    if (result != null && result.Roles.Contains("Admin"))
                    {
                        AdminManager.IsAdmin = true;
                        MessageBox.Show("Connexion réussie en tant qu'Admin !");
                    }
                    else
                    {
                        MessageBox.Show("Connexion réussie, mais vous n'êtes pas Admin.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                    }

                    ExecuteClose(null);
                }
                else
                {
                    MessageBox.Show("Échec de la connexion. Vérifiez vos identifiants.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExecuteClose(object? parameter)
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window is Views.LoginView)
                {
                    window.Close();
                    break;
                }
            }
        }

    }

    public class LoginResponse
    {
        public string Message { get; set; }
        public string Email { get; set; }
        public string[] Roles { get; set; }
    }

    public static class AdminManager
    {
        public static bool IsAdmin { get; set; } = false;

        internal static void Logout()
        {
            IsAdmin = false;
        }
    }
}
