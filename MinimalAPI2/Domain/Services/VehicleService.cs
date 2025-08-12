using MinimalAPI2.Domain.Entities;
using MinimalAPI2.Domain.Interface;
using MinimalAPI2.Infrastructure.Db;

namespace MinimalAPI2.Domain.Services;

public class VehicleService : IVehicleService
{
    private readonly MinimalAPIDbContext _dbContext;
    public VehicleService(MinimalAPIDbContext db)
    {
        _dbContext = db;
    }
    public void Delete(Vehicles vehicle)
    {
        _dbContext.Veiculos.Remove(vehicle);
        _dbContext.SaveChanges();
    }

    public Vehicles? GetById(int id)
    {
        var veiculo = _dbContext.Veiculos.Where(a => a.Id == id).FirstOrDefault();
        return veiculo;
    }

    public List<Vehicles> GetVehicles(int pagina = 1, string? nome = null, string? marca = null)
    {
        var query = _dbContext.Veiculos.AsQueryable();
        if (!string.IsNullOrEmpty(nome))                                                                             
        {
            query = query.Where(v => v.Nome.ToLower().Contains(nome.ToLower()));
        }

        int itensPorPagina = 10;
        query = query.Skip((pagina - 1) * itensPorPagina).Take(itensPorPagina);

        return query.ToList();
    }

    public void Include(Vehicles vehicle)
    {
        _dbContext.Veiculos.Add(vehicle);
        _dbContext.SaveChanges();
    }

    public void Update(Vehicles vehicle)
    {
        _dbContext.Veiculos.Update(vehicle);
        _dbContext.SaveChanges();
    }
}
