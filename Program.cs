using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using WebApplication1.Models;
using AppContext = WebApplication1.Models.AppContext;
using WebApplication1.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDbContext<AppContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    )
);
//builder.Services.AddScoped<IProductService, ProductService>();

// Enable CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200") // Allow Angular app
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSingleton<ConnectionMultiplexer>(config =>
{
    var connString = builder.Configuration.GetConnectionString("Redis");
    if (connString != null) throw new Exception("Redis connectioon string is empty.") ;
    var configuration = ConfigurationOptions.Parse(connString, true);
    return ConnectionMultiplexer.Connect(configuration);
});



var app = builder.Build();

app.UseHttpsRedirection();

// Use CORS before controllers
app.UseCors("AllowAngularApp");
app.MapControllers();

app.Run();
