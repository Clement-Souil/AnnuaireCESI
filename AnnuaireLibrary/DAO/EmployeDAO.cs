using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AnnuaireLibrary.DAO
{
    public class EmployeDAO
    {
        [Key]
        public int Id { get; set; } = 0;

        [Required]
        public string Nom { get; set; } = string.Empty;

        [Required]
        public string Prenom { get; set; } = string.Empty;

        public string TelephoneFixe { get; set; } = string.Empty;
        public string TelephonePortable { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public int ServiceId { get; set; } = 0;
        [ForeignKey("ServiceId")]
        public ServiceDAO Service { get; set; }

        public int SiteId { get; set; } = 0;
        [ForeignKey("SiteId")]
        public SiteDAO Site { get; set; }
    }
}
