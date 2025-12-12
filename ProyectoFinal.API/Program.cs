using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ProyectoFinal.Datos.Repositorios;
using ProyectoFinal.Dominio.Interfaces;
using ProyectoFinal.Dominio.Modelos;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// 1. SWAGGER
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Pon: Bearer TU_TOKEN",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});

// 2. AUTENTICACIÓN (Con la misma clave que AuthController)
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false, // No comprobar emisor
            ValidateAudience = false, // No comprobar audiencia
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

// 3. PERSISTENCIA
var tipoPersistencia = builder.Configuration["ConfiguracionProyecto:TipoPersistencia"];
var cadenaConexion = builder.Configuration["ConfiguracionProyecto:CadenaConexionMySQL"];

if (tipoPersistencia == "MySQL")
{
    // --- PINTAMOS EL AVISO EN VERDE ---
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("\n#############################################");
    Console.WriteLine("--> MODO ACTIVO: BASE DE DATOS MYSQL <--");
    Console.WriteLine("#############################################\n");
    Console.ResetColor(); // Volvemos al color normal

    builder.Services.AddScoped<IRepositorio<Coche>>(provider => new RepositorioMySQL(cadenaConexion));
}
else
{
    // --- PINTAMOS EL AVISO EN AMARILLO ---
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("\n#############################################");
    Console.WriteLine("--> MODO ACTIVO: MEMORIA RAM <--");
    Console.WriteLine("#############################################\n");
    Console.ResetColor();

    builder.Services.AddSingleton<IRepositorio<Coche>, RepositorioMemoria>();
}

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// 4. ORDEN OBLIGATORIO
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// 5. CARGA INICIAL
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var repositorio = services.GetRequiredService<IRepositorio<Coche>>();
        repositorio.CargarDesdeCSV("Sport car price.csv").Wait();
    }
    catch { }
}

app.Run();