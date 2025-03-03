using AnnuaireLibrary.DTO;

namespace AnnuaireApp.ViewModels
{
    public class EmployeDetailViewModel : BaseViewModel
    {
        public EmployeDTO Employe { get; }

        public EmployeDetailViewModel(EmployeDTO employe)
        {
            Employe = employe;
        }
    }
}
