using ApiElda.Contexts;
using ApiElda.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configurar Kestrel para que escuche en todas las IPs (si es necesario)
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(5153); // Asegúrate de que este puerto sea el que quieres usar
});


// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configurar CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// Configurar servicios necesarios para Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add controllers (Esto es necesario para que los controladores funcionen)
builder.Services.AddControllers();

var app = builder.Build();

// Usar CORS
app.UseCors("AllowAllOrigins");

// Configurar la tubería de la solicitud HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); // Habilitar Swagger
    app.UseSwaggerUI(); // Habilitar la interfaz de usuario de Swagger
}


// Usar redirección HTTPS (si es necesario)
app.UseHttpsRedirection();

// Habilitar la autorización
app.UseAuthorization();

// Mapear los controladores
app.MapControllers();

// Ejecutar la aplicación
app.Run();
