using AnnuaireLibrary.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Seeder
{
    class Program
    {
        static void Main(string[] args)
        {
            // Configuration de l'hôte pour le DbContext
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    // Connexion à la base de données MySQL
                    services.AddDbContext<AnnuaireContext>(options =>
                        options.UseMySql("server=localhost;port=3306;userid=root;password=;database=AnnuaireDB;",
                            ServerVersion.AutoDetect("server=localhost;port=3306;userid=root;password=;database=AnnuaireDB;")));

                    // Injection du SeedService
                    services.AddTransient<SeedService>();
                })
                .Build();

            // Appel du SeedService pour remplir la base de données
            using (var scope = host.Services.CreateScope())
            {
                var seedService = scope.ServiceProvider.GetRequiredService<SeedService>();
                seedService.SeedDatabase();
            }

            Console.WriteLine("La base de données a été remplie avec succès !");
        }
    }
}
