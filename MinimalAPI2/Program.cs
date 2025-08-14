using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MinimalAPI2.Domain.DTOs;
using MinimalAPI2.Domain.Entities;
using MinimalAPI2.Domain.Enuns;
using MinimalAPI2.Domain.Interface;
using MinimalAPI2.Domain.ModelViews;
using MinimalAPI2.Domain.Services;
using MinimalAPI2.Infrastructure.Db;
using System.Runtime.Intrinsics.Arm;
#region Builder
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IAdministratorService, AdministratorService>();
builder.Services.AddScoped<IVehicleService, VehicleService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<MinimalAPIDbContext>(options =>
{
    options.UseMySql(
        builder.Configuration.GetConnectionString("mysql"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("mysql")));
});
var app = builder.Build();
#endregion

#region Home
app.MapGet("/", () => Results.Json(new Home())).WithTags("Home");
#endregion

#region Administrator
app.MapPost("/login", ([FromBody] LoginDTO loginDTO, IAdministratorService administratorService) =>                                                            //Minimal API usamos o Endpoint dentro do program.cs
{
    if (administratorService.Login(loginDTO) != null)                                                                                                          //Usando o método criado em AdmService para verificar o login
    {
        return Results.Ok("Login com sucesso!");
    }
    else
    {
        return Results.Unauthorized();
    }
}).WithTags("Administrator");

app.MapPost("/administrator", ([FromBody] AdministratorDTO administratorDTO, IAdministratorService administratorService) =>                                                          
{
    var validation = new ValidationErrors
    {
        Messages = new List<string>()
    };
    if (string.IsNullOrEmpty(administratorDTO.Email))
    {
        validation.Messages.Add("O Email não pode ser vazio");
    }
    if (string.IsNullOrEmpty(administratorDTO.Senha))
    {
        validation.Messages.Add("A senha não pode ficar em branco");
    }
    if (administratorDTO.Perfil == null)
    {
        validation.Messages.Add("O perfil não pode ficar em branco");
    }
    if (validation.Messages.Count > 0)
    {
        return Results.BadRequest(validation.Messages);
    }

    var administrator = new Administrator
    {
        Email = administratorDTO.Email,
        Senha = administratorDTO.Senha,
        Perfil = administratorDTO.Perfil.ToString() ?? Perfil.Editor.ToString()
    };
    administratorService.Include(administrator);

    return Results.Created($"/administrator/{administrator.Id}", new AdministratorModelView
    {
        Id = administrator.Id,
        Email = administrator.Email,
        Perfil = administrator.Perfil
    });
}).WithTags("Administrator");

app.MapGet("/administrator", ([FromQuery] int? pagina, IAdministratorService administratorService) =>
{
    var adms = new List<AdministratorModelView>();
    var administrators = administratorService.GetAdm(pagina);
    foreach (var adm in administrators)
    {
        adms.Add(new AdministratorModelView
        {
            Id = adm.Id,
            Email = adm.Email,
            Perfil = adm.Perfil
        });
    }
    return Results.Ok(adms);
}).WithTags("Administrator");

app.MapGet("/administrator/{id}", ([FromRoute] int id, IAdministratorService administratorService) =>
{
    var administrator = administratorService.GetById(id);

    if (administrator == null)
    {
        return Results.NotFound();
    }
    else
    {
        return Results.Ok(new AdministratorModelView
        {
            Id = administrator.Id,
            Email = administrator.Email,
            Perfil = administrator.Perfil
        });
    }
}).WithTags("Administrator");
#endregion

#region Vehicle
app.MapPost("/vehicle", ([FromBody] VehicleDTO vehicleDTO, IVehicleService vehicleService) =>                                                           
{
    var validation = validationDTO(vehicleDTO);
    if (validation.Messages.Count > 0)
    {
        return Results.BadRequest(validation.Messages);
    }

    var vehicle = new Vehicles
    {
        Nome = vehicleDTO.Nome,
        Marca = vehicleDTO.Marca,
        Ano = vehicleDTO.Ano,
    };
    vehicleService.Include(vehicle);

    return Results.Created($"/veiculo/{vehicle.Id}", vehicle);
}).WithTags("Vehicle");

app.MapGet("/vehicle", ([FromQuery] int? pagina, IVehicleService vehicleService) =>
{
    var vehicles = vehicleService.GetVehicles(pagina);

    return Results.Ok(vehicles);
}).WithTags("Vehicle");

app.MapGet("/vehicle/{id}", ([FromRoute] int id, IVehicleService vehicleService) =>
{
    var vehicle = vehicleService.GetById(id);

    if(vehicle == null)
    {
        return Results.NotFound();
    }
    else
    { 
        return Results.Ok(vehicle);
    }
}).WithTags("Vehicle");

app.MapPut("/vehicle/{id}", ([FromRoute] int id, VehicleDTO vehicleDTO, IVehicleService vehicleService) =>
{
    var vehicle = vehicleService.GetById(id);

    if (vehicle == null)
    {
        return Results.NotFound();
    }

    var validation = validationDTO(vehicleDTO);
    if (validation.Messages.Count > 0)
    {
        return Results.BadRequest(validation.Messages);
    }

    vehicle.Nome = vehicleDTO.Nome;
    vehicle.Marca = vehicleDTO.Marca;
    vehicle.Ano = vehicleDTO.Ano;

    vehicleService.Update(vehicle);

    return Results.Ok(vehicle);
}).WithTags("Vehicle");

app.MapDelete("/vehicle/{id}", ([FromRoute] int id, IVehicleService vehicleService) =>
{
    var vehicle = vehicleService.GetById(id);

    if (vehicle == null)
    {
        return Results.NotFound();
    }
    else
    {
        vehicleService.Delete(vehicle);
        return Results.NoContent();
    }
}).WithTags("Vehicle");
#endregion

#region App
app.UseSwagger();
app.UseSwaggerUI();

app.Run();
#endregion

ValidationErrors validationDTO(VehicleDTO vehicleDTO)
{
    var validation = new ValidationErrors
    {
        Messages = new List<string>()
    };

    if (string.IsNullOrEmpty(vehicleDTO.Nome))
    {
        validation.Messages.Add("O Nome não pode ser vazio");
    }
    if (string.IsNullOrEmpty(vehicleDTO.Marca))
    {
        validation.Messages.Add("A marca não pode ficar em branco");
    }
    if (vehicleDTO.Ano > 2026 || vehicleDTO.Ano < 1885)
    {
        validation.Messages.Add("O ano é inválido");
    }

    return validation;
}