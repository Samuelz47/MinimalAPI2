using MinimalAPI2.Domain.DTOs;
using MinimalAPI2.Domain.Entities;
using MinimalAPI2.Domain.Interface;
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
}
