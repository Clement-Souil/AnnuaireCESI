using System.ComponentModel.DataAnnotations;

namespace AnnuaireLibrary.DTO
{
    public class ServiceDTO
    {
        public int Id { get; set; }

        [RegularExpression(@"^[A-Za-zÀ-ÖØ-öø-ÿ\- ]+$", ErrorMessage = "Le service ne peut contenir que des lettres.")]
        public string? Nom { get; set; }
    }
}
