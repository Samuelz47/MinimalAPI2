using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MinimalAPI2.Domain.DTOs;
using MinimalAPI2.Domain.Entities;
using MinimalAPI2.Domain.Enuns;
using MinimalAPI2.Domain.Interface;
using MinimalAPI2.Domain.ModelViews;
using MinimalAPI2.Domain.Services;
using MinimalAPI2.Infrastructure.Db;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.Intrinsics.Arm;
using System.Security.Claims;
using System.Text;

#region Builder
var builder = WebApplication.CreateBuilder(args);
var key = builder.Configuration.GetSection("Jwt").ToString();

builder.Services.AddAuthentication(option =>
{
    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;                      //Configuração do JWT Authentication
    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(option =>
{
    option.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateLifetime = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
        ValidateIssuer = false,
        ValidateAudience = false,
    };
});

builder.Services.AddAuthorization();

builder.Services.AddScoped<IAdministratorService, AdministratorService>();
builder.Services.AddScoped<IVehicleService, VehicleService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insira o token JWT aqui"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new List<string>()
        }
    });
});

builder.Services.AddDbContext<MinimalAPIDbContext>(options =>
{
    options.UseMySql(
        builder.Configuration.GetConnectionString("mysql"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("mysql")));
});
var app = builder.Build();
#endregion

#region Home
app.MapGet("/", () => Results.Json(new Home())).AllowAnonymous().WithTags("Home");
#endregion

#region Administrator
string GenerateToken(Administrator administrator)
{
    if (string.IsNullOrEmpty(key))
    {
        return string.Empty;
    }
    else
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));                                                                                //Criptografia
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>()
        {
            new Claim("Email", administrator.Email),
            new Claim("Perfil", administrator.Perfil)
        };
        var token = new JwtSecurityToken
        (
            claims: claims,
            expires: DateTime.Now.AddDays(1),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

app.MapPost("/login", ([FromBody] LoginDTO loginDTO, IAdministratorService administratorService) =>                                                            //Minimal API usamos o Endpoint dentro do program.cs
{
    var administrator = administratorService.Login(loginDTO);
    if (administrator != null)                                                                                                          //Usando o método criado em AdmService para verificar o login
    {
        string token = GenerateToken(administrator);
        return Results.Ok(new AdmLogged
        {
            Email = administrator.Email,
            Perfil = administrator.Perfil,
            Token = token
        });
    }
    else
    {
        return Results.Unauthorized();
    }
}).AllowAnonymous().WithTags("Administrator");

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
}).RequireAuthorization().WithTags("Administrator");

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
}).RequireAuthorization().WithTags("Administrator");

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
}).RequireAuthorization().WithTags("Administrator");
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
}).RequireAuthorization().WithTags("Vehicle");

app.MapGet("/vehicle", ([FromQuery] int? pagina, IVehicleService vehicleService) =>
{
    var vehicles = vehicleService.GetVehicles(pagina);

    return Results.Ok(vehicles);
}).RequireAuthorization().WithTags("Vehicle");

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
}).RequireAuthorization().WithTags("Vehicle");

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
}).RequireAuthorization().WithTags("Vehicle");

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
}).RequireAuthorization().WithTags("Vehicle");
#endregion

#region App
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

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