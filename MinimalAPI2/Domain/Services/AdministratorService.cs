using MinimalAPI2.Domain.DTOs;
using MinimalAPI2.Domain.Entities;
using MinimalAPI2.Domain.Interface;
using MinimalAPI2.Domain.ModelViews;
using MinimalAPI2.Infrastructure.Db;

namespace MinimalAPI2.Domain.Services;

public class AdministratorService : IAdministratorService                   //Classes de serviço fazem o "trabalho"
{
    private readonly MinimalAPIDbContext _dbContext;                        //Precisamos do construtor chamando o dbContext que nada mais é que nosso banco de dados, nesse caso uma injeção de dependencia
    public AdministratorService(MinimalAPIDbContext db)
    {
        _dbContext = db;
    }
    public Administrator? Login(LoginDTO loginDTO)
    {
        var adm = _dbContext.Administradores.Where(a => a.Email == loginDTO.Email && a.Senha == loginDTO.Senha).FirstOrDefault();                           //Nesse caso ela busca pra ver se o email e a senha existem na nossa base de dados
        return adm;
    }
    public Administrator Include(Administrator administrator)
    {
        _dbContext.Administradores.Add(administrator);
        _dbContext.SaveChanges();
        return administrator;
    }
    public List<Administrator> GetAdm(int? pagina)
    {
        var query = _dbContext.Administradores.AsQueryable();

        int itensPorPagina = 10;

        if (pagina != null)
        {
            query = query.Skip(((int)pagina - 1) * itensPorPagina).Take(itensPorPagina);
        }

        return query.ToList();
    }
    public Administrator? GetById(int id)
    {
        var administrator = _dbContext.Administradores.Where(a => a.Id == id).FirstOrDefault();
        return administrator;
    }
}
