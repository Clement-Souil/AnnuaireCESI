using AnnuaireApp.Helpers;

namespace AnnuaireApp.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private string _titre;
        public string Titre
        {
            get { return _titre; }
            set
            {
                _titre = value;
                OnPropertyChanged(nameof(Titre));
            }
        }

        public RelayCommand ChangerTitreCommand { get; }

        public MainViewModel()
        {
            Titre = "Bienvenue dans l'Annuaire !";
            ChangerTitreCommand = new RelayCommand(_ => ChangerTitre());
        }

        private void ChangerTitre()
        {
            Titre = "Titre mis à jour !";
        }
    }
}
