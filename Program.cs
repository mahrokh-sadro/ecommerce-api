using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using WebApplication1.Models;
using WebApplication1.Services;
using WebApplication1.Interfaces;
using WebApplication1.Views;
using AppContext = WebApplication1.Models.AppContext;
using Microsoft.AspNetCore.Identity;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<AppContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    )
);



// Enable CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200") // Allow Angular app
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials()
                  ;
        });
});
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var connString = builder.Configuration.GetConnectionString("Redis");
    if (string.IsNullOrEmpty(connString))
    {
        throw new Exception("Redis connection string is empty.");
    }
    return ConnectionMultiplexer.Connect(connString);
});

builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddAuthorization();
//Add Identity services
builder.Services.AddIdentityApiEndpoints<AppUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AppContext>();


var app = builder.Build();

app.UseHttpsRedirection();
app.UseCors("AllowAngularApp");

// Add authentication & authorization middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapGroup("api").MapIdentityApi<AppUser>(); // api/login

app.Run();
