using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AnnuaireLibrary.DAO
{
    public class ServiceDAO
    {
        [Key]
        public int Id { get; set; } = 0;

        [Required]
        public string Nom { get; set; } = string.Empty;

        public List<EmployeDAO> Employes { get; set; } = new List<EmployeDAO>();
    }
}
