using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using AnnuaireLibrary.Data; 

namespace AnnuaireLibrary.Factories
{
    public class AnnuaireContextFactory : IDesignTimeDbContextFactory<AnnuaireContext>
    {
        public AnnuaireContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AnnuaireContext>();

            var connectionString = "server=localhost;port=3306;userid=root;password=;database=AnnuaireDB;";
            optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

            return new AnnuaireContext(optionsBuilder.Options);
        }
    }
}

