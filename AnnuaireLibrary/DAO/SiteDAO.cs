using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AnnuaireLibrary.DAO
{
    public class SiteDAO
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Ville { get; set; }

        public List<EmployeDAO> Employes { get; set; } = new List<EmployeDAO>();
    }
}
