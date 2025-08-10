using Microsoft.EntityFrameworkCore;
using MinimalAPI2.Domain.DTOs;
using MinimalAPI2.Infrastructure.Db;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<MinimalAPIDbContext>(options =>
{
    options.UseMySql(
        builder.Configuration.GetConnectionString("mysql"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("mysql")));
});
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapPost("/login", (LoginDTO loginDTO) =>                                                            //Minimal API usamos o Endpoint dentro do program.cs
{
    if (loginDTO.Email == "adm@teste.com" && loginDTO.Senha == "123456")                               //Teste de pedreiro pra ver se o login é igual o do adm
    {
        return Results.Ok("Login com sucesso!");
    }
    else
    {
        return Results.Unauthorized();
    }
});

app.Run();
