using MinimalAPI2.Domain.DTOs;
using MinimalAPI2.Domain.Entities;

namespace MinimalAPI2.Domain.Interface;

public interface IAdministratorService
{
    Administrator Login(LoginDTO loginDTO);

    Administrator Include(Administrator administrator);

    List<Administrator> GetAdm(int? pagina);

    Administrator? GetById(int id);
}
