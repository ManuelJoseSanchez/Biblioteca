using System.Text.Json.Serialization;
using BibliotecaAPI;
using BibliotecaAPI.Datos;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);

//area de servicio
builder.Services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddTransient<IRepositorioValores,RepositorioValores>();
var app = builder.Build();

// area de middlewares
app.MapControllers();

app.Run();
