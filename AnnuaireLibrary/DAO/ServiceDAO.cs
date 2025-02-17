using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AnnuaireLibrary.DAO
{
    public class ServiceDAO
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Nom { get; set; }

        public List<EmployeDAO> Employes { get; set; } = new List<EmployeDAO>();
    }
}
