using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MinimalAPI2.Domain.Entities;
using MinimalAPI2.Domain.Services;
using MinimalAPI2.Infrastructure.Db;

namespace Test.Domain.Service;
[TestClass]
public sealed class AdministratorServiceTest
{
    private MinimalAPIDbContext _context;
    [TestInitialize]
    public void TestInitialize()
    {
        // Este método é executado antes de cada teste
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables();
        var configuration = builder.Build();

        _context = new MinimalAPIDbContext(configuration);

        // Limpar a tabela antes de cada teste
        _context.Database.ExecuteSqlRaw("TRUNCATE TABLE administradores");
    }
    [TestMethod]
    public void TestSaveAdministrator()
    {
        // Arange
        var adm = new Administrator();
        adm.Id = 1;
        adm.Email = "teste@teste.com";
        adm.Senha = "teste";
        adm.Perfil = "Adm";

        var administratorService = new AdministratorService(_context);

        // Act
        administratorService.Include(adm);

        // Assert
        Assert.AreEqual(1, administratorService.GetAdm(1).Count());
    }
    [TestMethod]
    public void TestGetById()
    {
        // Arange
        var adm = new Administrator();
        adm.Id = 1;
        adm.Email = "teste@teste.com";
        adm.Senha = "teste";
        adm.Perfil = "Adm";

        var administratorService = new AdministratorService(_context);

        // Act
        administratorService.Include(adm);
        var admin = administratorService.GetById(adm.Id);

        // Assert
        Assert.AreEqual(1, admin.Id);
    }
}
