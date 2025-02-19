using AnnuaireLibrary.DAO;
using AnnuaireLibrary.Data;
using Microsoft.EntityFrameworkCore;

namespace Seeder
{
    public class SeedService
    {
        private readonly AnnuaireContext _context;

        // Injection de dépendance du DbContext
        public SeedService(AnnuaireContext context)
        {
            _context = context;
        }

        // Méthode pour peupler la base de données
        public void SeedDatabase()
        {
            // S'assurer que la base est bien créée
            _context.Database.Migrate();

            // Vérifier et peupler les Sites
            if (!_context.Sites.Any())
            {
                _context.Sites.AddRange(new List<SiteDAO>
                {
                    new SiteDAO { Ville = "Paris" },
                    new SiteDAO { Ville = "Lyon" },
                    new SiteDAO { Ville = "Marseille" },
                    new SiteDAO { Ville = "Nantes" },
                    new SiteDAO { Ville = "Lille" }
                });
                _context.SaveChanges();
            }

            // Vérifier et peupler les Services
            if (!_context.Services.Any())
            {
                _context.Services.AddRange(new List<ServiceDAO>
                {
                    new ServiceDAO { Nom = "Informatique" },
                    new ServiceDAO { Nom = "Ressources Humaines" },
                    new ServiceDAO { Nom = "Marketing" },
                    new ServiceDAO { Nom = "Commercial" },
                    new ServiceDAO { Nom = "Comptabilité" }
                });
                _context.SaveChanges();
            }

            // Vérifier et peupler les Employés
            if (!_context.Employes.Any())
            {
                var noms = new[] { "Dupont", "Martin", "Bernard", "Petit", "Durand", "Leroy", "Moreau", "Simon", "Laurent", "Lefevre" };
                var prenoms = new[] { "Jean", "Marie", "Pierre", "Sophie", "Paul", "Julie", "Michel", "Claire", "Jacques", "Laura" };

                var random = new Random();

                var sites = _context.Sites.ToList();
                var services = _context.Services.ToList();

                // Génération de 100 employés aléatoires
                for (int i = 0; i < 100; i++)
                {
                    var nom = noms[random.Next(noms.Length)];
                    var prenom = prenoms[random.Next(prenoms.Length)];
                    var email = $"{prenom.ToLower()}.{nom.ToLower()}@entreprise.com";

                    var employe = new EmployeDAO
                    {
                        Nom = nom,
                        Prenom = prenom,
                        TelephoneFixe = $"01{random.Next(10000000, 99999999)}",
                        TelephonePortable = $"06{random.Next(10000000, 99999999)}",
                        Email = email,
                        ServiceId = services[random.Next(services.Count)].Id,
                        SiteId = sites[random.Next(sites.Count)].Id
                    };

                    _context.Employes.Add(employe);
                }

                _context.SaveChanges();
            }
        }
    }
}
