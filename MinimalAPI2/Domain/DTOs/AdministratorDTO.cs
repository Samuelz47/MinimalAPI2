using MinimalAPI2.Domain.Enuns;

namespace MinimalAPI2.Domain.DTOs;

public class AdministratorDTO
{
    public string Email { get; set; } = default!;
    public string Senha { get; set; } = default!;
    public Perfil? Perfil { get; set; } = default!;
}
