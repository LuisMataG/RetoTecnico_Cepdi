using Microsoft.EntityFrameworkCore;
using WebApi.Models;
using WebApi.Services.Interfaces;
using WebApi.Services.Impementations;

var builder = WebApplication.CreateBuilder(args);

// Configuración para leer la cadena de conexión desde la variable de entorno
var connectionString = Environment.GetEnvironmentVariable("MY_CONNECTION_STRING");

// Configurar el DbContext con la cadena de conexión
builder.Services.AddDbContext<CepdiPruebaContext>(options =>
    options.UseSqlServer(connectionString));

// Registrar el servicio IUsuarioService con su implementación UsuarioService
builder.Services.AddScoped<IUsuarioService, UsuarioService>();

// Agregar servicios para CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", builder =>
    {
        builder.AllowAnyOrigin()    // Permite solicitudes desde cualquier origen
               .AllowAnyMethod()    // Permite cualquier método HTTP (GET, POST, PUT, DELETE, etc.)
               .AllowAnyHeader();   // Permite cualquier encabezado
    });
});

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Habilitar CORS con la política definida
app.UseCors("AllowAllOrigins");

app.UseAuthorization();

app.MapControllers();

app.Run();
