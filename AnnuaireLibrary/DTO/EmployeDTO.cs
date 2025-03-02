﻿using System.ComponentModel.DataAnnotations;

namespace AnnuaireLibrary.DTO
{
    public class EmployeDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Le nom est obligatoire.")]
        [RegularExpression(@"^[A-Za-zÀ-ÖØ-öø-ÿ\- ]+$", ErrorMessage = "Le nom ne peut contenir que des lettres.")]
        public string? Nom { get; set; }

        [Required(ErrorMessage = "Le prénom est obligatoire.")]
        [RegularExpression(@"^[A-Za-zÀ-ÖØ-öø-ÿ\- ]+$", ErrorMessage = "Le prénom ne peut contenir que des lettres.")]
        public string? Prenom { get; set; }

        [RegularExpression(@"^(\d{10})?$", ErrorMessage = "Le téléphone fixe doit contenir 10 chiffres.")]
        public string? TelephoneFixe { get; set; }

        [RegularExpression(@"^(\d{10})?$", ErrorMessage = "Le téléphone fixe doit contenir 10 chiffres.")]
        public string? TelephonePortable { get; set; }

        [Required(ErrorMessage = "L'email est obligatoire.")]
        [EmailAddress(ErrorMessage = "L'email n'est pas valide.")]
        public string? Email { get; set; }

        public int ServiceId { get; set; }

        public int SiteId { get; set; }

        //// Champs uniquement pour affichage (non sérialisés en entrée)
        [System.Text.Json.Serialization.JsonIgnore]  // Ignore en POST
        [Required(ErrorMessage = "Le service est obligatoire.")]
        public string? Service { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]  // Ignore en POST
        [Required(ErrorMessage = "Le site est obligatoire.")]
        public string? Site { get; set; }
    }
}

