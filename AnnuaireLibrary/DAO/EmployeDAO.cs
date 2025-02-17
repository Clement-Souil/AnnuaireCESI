using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AnnuaireLibrary.DAO
{
    public class EmployeDAO
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Nom { get; set; }

        [Required]
        public string Prenom { get; set; }

        public string TelephoneFixe { get; set; }
        public string TelephonePortable { get; set; }
        public string Email { get; set; }

        public int ServiceId { get; set; }
        [ForeignKey("ServiceId")]
        public ServiceDAO Service { get; set; }

        public int SiteId { get; set; }
        [ForeignKey("SiteId")]
        public SiteDAO Site { get; set; }
    }
}
