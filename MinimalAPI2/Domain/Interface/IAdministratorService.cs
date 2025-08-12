using MinimalAPI2.Domain.DTOs;
using MinimalAPI2.Domain.Entities;

namespace MinimalAPI2.Domain.Interface;

public interface IAdministratorService
{
    Administrator Login(LoginDTO loginDTO);
}
