using AnnuaireLibrary.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Ajout du DbContext avec MySQL
builder.Services.AddDbContext<AnnuaireContext>(options =>
    options.UseMySql("server=localhost;port=3306;userid=root;password=;database=AnnuaireDB;",
                     ServerVersion.AutoDetect("server=localhost;port=3306;userid=root;password=;database=AnnuaireDB;")));

// Ajouter les services MVC et JSON
builder.Services.AddControllers().AddNewtonsoftJson();

// Autoriser les appels API depuis n'importe quelle source 
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Annuaire API", Version = "v1" });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Annuaire API v1");
    });
}

// Activer CORS ( sert à autoriser les appels depuis le frontend )
app.UseCors("AllowAll");

app.UseAuthorization();
app.MapControllers();
app.Run();
