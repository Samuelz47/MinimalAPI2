using MinimalAPI2.Domain.Entities;

namespace MinimalAPI2.Domain.Interface;

public interface IVehicleService
{
    List<Vehicles> GetVehicles(int? pagina = 1, string? nome = null, string? marca = null);
    Vehicles? GetById(int id);
    void Include(Vehicles vehicle);
    void Update(Vehicles vehicle);
    void Delete(Vehicles vehicle);
}
