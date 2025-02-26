namespace AnnuaireLibrary.DTO
{
    public class EmployeDTO
    {
        public int Id { get; set; }
        public string? Nom { get; set; }
        public string? Prenom { get; set; }
        public string? TelephoneFixe { get; set; }
        public string? TelephonePortable { get; set; }
        public string? Email { get; set; }

        public int ServiceId { get; set; }
        public int SiteId { get; set; }

        // Champs uniquement pour affichage (non sérialisés en entrée)
        [System.Text.Json.Serialization.JsonIgnore]  // Ignore en POST
        public string? Service { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]  // Ignore en POST
        public string? Site { get; set; }
    }
}
