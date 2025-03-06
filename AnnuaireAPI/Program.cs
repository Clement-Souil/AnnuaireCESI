using AnnuaireLibrary.Data;
using AnnuaireLibrary.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;



var builder = WebApplication.CreateBuilder(args);

//  Connexion à la base de données
builder.Services.AddDbContext<AnnuaireContext>(options =>
    options.UseMySql(
        "server=localhost;port=3306;userid=root;password=;database=AnnuaireDB;",
        ServerVersion.AutoDetect("server=localhost;port=3306;userid=root;password=;database=AnnuaireDB;"))
);

//  Configuration d'ASP.NET Identity (sans JWT)
builder.Services.AddIdentity<UserSecure, IdentityRole>()
    .AddEntityFrameworkStores<AnnuaireContext>()
    .AddDefaultTokenProviders();

//  Ajout des services MVC
builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Annuaire API",
        Version = "v1"
    });
});


var app = builder.Build();

//  Configuration du pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Annuaire API V1");
        options.RoutePrefix = "swagger"; 
    });
}

app.UseHttpsRedirection(); // Force HTTPS
app.UseStaticFiles(); // Active les fichiers statiques
app.UseRouting(); // Active le routage
app.UseAuthentication(); // Active l'authentification ASP.NET Identity
app.UseAuthorization(); // Active l'autorisation

app.MapControllers(); // Mappe les routes vers les contrôleurs



app.Run();
