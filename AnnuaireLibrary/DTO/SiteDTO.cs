using System.ComponentModel.DataAnnotations;

namespace AnnuaireLibrary.DTO
{
    public class SiteDTO
    {
        public int Id { get; set; }

        [RegularExpression(@"^[A-Za-zÀ-ÖØ-öø-ÿ\- ]+$", ErrorMessage = "Le site ne peut contenir que des lettres.")]
        public string? Ville { get; set; }
    }
}
