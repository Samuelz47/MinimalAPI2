using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MinimalAPI2.Domain.DTOs;
using MinimalAPI2.Domain.Interface;
using MinimalAPI2.Domain.Services;
using MinimalAPI2.Infrastructure.Db;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IAdministratorService, AdministratorService>();

builder.Services.AddDbContext<MinimalAPIDbContext>(options =>
{
    options.UseMySql(
        builder.Configuration.GetConnectionString("mysql"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("mysql")));
});
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapPost("/login", ([FromBody] LoginDTO loginDTO, IAdministratorService administratorService) =>                                                            //Minimal API usamos o Endpoint dentro do program.cs
{
    if (administratorService.Login(loginDTO) != null)                                                                                                          //Usando o m�todo criado em AdmService para verificar o login
    {
        return Results.Ok("Login com sucesso!");
    }
    else
    {
        return Results.Unauthorized();
    }
});

app.Run();
