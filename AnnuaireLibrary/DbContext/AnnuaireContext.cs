﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using AnnuaireLibrary.DAO;
using AnnuaireLibrary.Models;

namespace AnnuaireLibrary.Data
{
    public class AnnuaireContext : IdentityDbContext<UserSecure>
    {
        public DbSet<EmployeDAO> Employes { get; set; }
        public DbSet<ServiceDAO> Services { get; set; }
        public DbSet<SiteDAO> Sites { get; set; }

        public AnnuaireContext(DbContextOptions<AnnuaireContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var connectionString = "server=localhost;port=3306;userid=root;password=;database=AnnuaireDB;";
                optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            }
        }
    }
}