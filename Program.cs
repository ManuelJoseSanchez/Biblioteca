using BibliotecaAPI.Datos;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);

//area de servicio
builder.Services.AddControllers();
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// area de middlewares
app.MapControllers();

app.Run();
