using System;
using System.Threading.Tasks;
using AnnuaireLibrary.Data;
using AnnuaireLibrary.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Identity;

namespace Seeder
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                //  Configuration de l'hôte pour le DbContext et Identity
                var host = Host.CreateDefaultBuilder(args)
                    .ConfigureServices((context, services) =>
                    {
                        //  Connexion à la base de données MySQL
                        var connectionString = "server=localhost;port=3306;userid=root;password=;database=AnnuaireDB;";
                        services.AddDbContext<AnnuaireContext>(options =>
                            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

                        //  Ajout d'Identity pour gérer les utilisateurs et rôles
                        services.AddIdentity<UserSecure, IdentityRole>()
                            .AddEntityFrameworkStores<AnnuaireContext>()
                            .AddDefaultTokenProviders();

                        //  Injection du SeedService
                        services.AddTransient<SeedService>();
                    })
                    .Build();

                //  Exécution du SeedService
                using (var scope = host.Services.CreateScope())
                {
                    var seedService = scope.ServiceProvider.GetRequiredService<SeedService>();
                    await seedService.SeedDatabase();
                }

                Console.WriteLine(" La base de données a été remplie avec succès !");
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Erreur lors du remplissage de la base : {ex.Message}");
                Environment.Exit(1); // Sort proprement en cas d'échec
            }
        }
    }
}
