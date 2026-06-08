using System.Text.Json.Serialization;
using Biblioteca.Middleware;
using BibliotecaAPI.Datos;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);

//area de servicio

builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddAutoMapper(cfg => { }, typeof(Program));

var app = builder.Build();

// area de middlewares

app.UseLogueaPeticion();
app.UseBloqueoPeticion();

app.MapControllers();

app.Run();
