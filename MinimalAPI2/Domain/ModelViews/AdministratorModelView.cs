using MinimalAPI2.Domain.Enuns;

namespace MinimalAPI2.Domain.ModelViews;

public record AdministratorModelView
{
    public int Id { get; set; }
    public string Email { get; set; } = default!;
    public string Perfil { get; set; } = default!;
}
