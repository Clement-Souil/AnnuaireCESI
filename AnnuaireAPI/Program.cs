using AnnuaireLibrary.Data;
using AnnuaireLibrary.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Ajouter le DbContext avec Identity
builder.Services.AddDbContext<AnnuaireContext>(options =>
    options.UseMySql("server=localhost;port=3306;userid=root;password=;database=AnnuaireDB;",
                     ServerVersion.AutoDetect("server=localhost;port=3306;userid=root;password=;database=AnnuaireDB;")));

// Ajouter ASP.NET Identity
builder.Services.AddIdentity<UserSecure, IdentityRole>()
    .AddEntityFrameworkStores<AnnuaireContext>()
    .AddDefaultTokenProviders()
    .AddApiEndpoints();

// Configurer l'authentification JWT
var key = Encoding.UTF8.GetBytes("MaSuperCleSecretePourJWT123!"); 
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// Ajouter Swagger et activer l'authentification
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

var app = builder.Build();

// Activer l'authentification et l'autorisation
app.UseAuthentication();
app.UseAuthorization();

app.MapIdentityApi<UserSecure>();

app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.Run();
