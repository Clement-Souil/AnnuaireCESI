using AnnuaireLibrary.DAO;
using AnnuaireLibrary.Data;
using AnnuaireLibrary.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Seeder
{
    public class SeedService
    {
        private readonly AnnuaireContext _context;
        private readonly UserManager<UserSecure> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public SeedService(AnnuaireContext context, UserManager<UserSecure> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task SeedDatabase()
        {
            try
            {
                //  Appliquer les migrations
                Console.WriteLine("Migration de la base de données...");
                await _context.Database.MigrateAsync();
                Console.WriteLine("Migration terminée.");

                //  Peupler les Sites
                if (!await _context.Sites.AnyAsync())
                {
                    Console.WriteLine("Ajout des sites...");
                    await _context.Sites.AddRangeAsync(new List<SiteDAO>
                    {
                        new SiteDAO { Ville = "Paris" },
                        new SiteDAO { Ville = "Lyon" },
                        new SiteDAO { Ville = "Marseille" },
                        new SiteDAO { Ville = "Nantes" },
                        new SiteDAO { Ville = "Lille" }
                    });
                    await _context.SaveChangesAsync();
                    Console.WriteLine("Sites ajoutés.");
                }

                //  Peupler les Services
                if (!await _context.Services.AnyAsync())
                {
                    Console.WriteLine("Ajout des services...");
                    await _context.Services.AddRangeAsync(new List<ServiceDAO>
                    {
                        new ServiceDAO { Nom = "Informatique" },
                        new ServiceDAO { Nom = "Ressources Humaines" },
                        new ServiceDAO { Nom = "Marketing" },
                        new ServiceDAO { Nom = "Commercial" },
                        new ServiceDAO { Nom = "Comptabilité" }
                    });
                    await _context.SaveChangesAsync();
                    Console.WriteLine("Services ajoutés.");
                }

                // Peupler les Employés
                if (!await _context.Employes.AnyAsync())
                {
                    Console.WriteLine("Ajout des employés...");
                    var noms = new[] { "Dupont", "Martin", "Bernard", "Petit", "Durand", "Leroy", "Moreau", "Simon", "Laurent", "Lefevre" };
                    var prenoms = new[] { "Jean", "Marie", "Pierre", "Sophie", "Paul", "Julie", "Michel", "Claire", "Jacques", "Laura" };
                    var random = new Random();

                    var sites = await _context.Sites.ToListAsync();
                    var services = await _context.Services.ToListAsync();

                    var employes = new List<EmployeDAO>();
                    var emailsExistants = new HashSet<string>(await _context.Employes.Select(e => e.Email).ToListAsync()); // Récupère tous les emails existants

                    for (int i = 0; i < 100; i++)
                    {
                        string email;
                        int attempts = 0;
                        do
                        {
                            var nom = noms[random.Next(noms.Length)];
                            var prenom = prenoms[random.Next(prenoms.Length)];
                            email = $"{prenom.ToLower()}.{nom.ToLower()}@entreprise.com";
                            attempts++;
                        } while (emailsExistants.Contains(email) && attempts < 10); // Vérifie dans la HashSet

                        if (attempts < 10) // Si on a trouvé un email unique
                        {
                            emailsExistants.Add(email); // Ajoute l'email à la liste des existants pour éviter les doublons

                            employes.Add(new EmployeDAO
                            {
                                Nom = noms[random.Next(noms.Length)],
                                Prenom = prenoms[random.Next(prenoms.Length)],
                                TelephoneFixe = $"01{random.Next(10000000, 99999999)}",
                                TelephonePortable = $"06{random.Next(10000000, 99999999)}",
                                Email = email,
                                ServiceId = services[random.Next(services.Count)].Id,
                                SiteId = sites[random.Next(sites.Count)].Id
                            });
                        }
                    }

                    try
                    {
                        await _context.Employes.AddRangeAsync(employes);
                        await _context.SaveChangesAsync();
                        Console.WriteLine("Employés ajoutés.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"ERREUR LORS DE L'AJOUT DES EMPLOYÉS : {ex.Message}");
                    }
                }


                //  Création du rôle Admin
                if (!await _roleManager.RoleExistsAsync("Admin"))
                {
                    Console.WriteLine("Création du rôle Admin...");
                    await _roleManager.CreateAsync(new IdentityRole("Admin"));
                    Console.WriteLine("Rôle Admin créé.");
                }

                //  Création de l'utilisateur Admin
                if (await _userManager.FindByEmailAsync("admin@annuaire.com") == null)
                {
                    Console.WriteLine("Création de l'Admin...");
                    var admin = new UserSecure
                    {
                        UserName = "admin@annuaire.com",
                        Email = "admin@annuaire.com",
                        FullName = "Admin Annuaire"
                    };

                    var result = await _userManager.CreateAsync(admin, "Admin123!");
                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(admin, "Admin");
                        Console.WriteLine(" Admin ajouté avec succès !");
                    }
                    else
                    {
                        Console.WriteLine($" Erreur lors de la création de l'Admin : {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($" ERREUR LORS DU SEEDING : {ex.Message}");
            }
        }
    }
}
