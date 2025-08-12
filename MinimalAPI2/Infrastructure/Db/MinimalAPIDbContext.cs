using Microsoft.EntityFrameworkCore;
using MinimalAPI2.Domain.Entities;
using System.Data.Common;

namespace MinimalAPI2.Infrastructure.Db;

public class MinimalAPIDbContext : DbContext
{
    private readonly IConfiguration _configurationAppSettings;                                              //Puxa a configuração do appsettings.json onde declaramos a connectionString
    public MinimalAPIDbContext(IConfiguration configurationAppSettings)
    {
        _configurationAppSettings = configurationAppSettings;
    }
    public DbSet<Administrator> Administradores {  get; set; }                                              //Seta as tabelas
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Administrator>().HasData(new Administrator
        {
            Id = 1,
            Email = "administrator@teste.com",
            Senha = "123456",
            Perfil = "Adm"
        });
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)                                                                   //Caso o builder ainda não tenha sido configurado
        {
            var connectionString = _configurationAppSettings.GetConnectionString("mysql")?.ToString();

            if (!string.IsNullOrEmpty(connectionString))
            {
                optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            }
        }
    }
}
