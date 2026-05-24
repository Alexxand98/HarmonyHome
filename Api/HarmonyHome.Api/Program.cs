using HarmonyHome.Api.Data;
using HarmonyHome.Api.Models.Entity;
using HarmonyHome.Api.Repository;
using HarmonyHome.Api.Repository.IRepository;
using HarmonyHome.Api.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

using System.Text;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Swagger / OpenAPI visual
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Introduce el token JWT. Ejemplo: Bearer {token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });
});

// Entity Framework Core + SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// JWT
var key = builder.Configuration.GetValue<string>("ApiSettings:Secret");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key!)),
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration.GetValue<string>("ApiSettings:Issuer"),
        ValidateAudience = true,
        ValidAudience = builder.Configuration.GetValue<string>("ApiSettings:Audience"),
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>{options.AddPolicy("AllowAngular", policy =>{policy.WithOrigins("http://localhost:4200").AllowAnyHeader().AllowAnyMethod();});});

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IProductoRepository, ProductoRepository>();
builder.Services.AddScoped<IUbicacionRepository, UbicacionRepository>();
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<IStockRepository, StockRepository>();
builder.Services.AddScoped<IVentaRepository, VentaRepository>();
builder.Services.AddScoped<IPedidoClienteRepository, PedidoClienteRepository>();
builder.Services.AddScoped<IOrdenRecogidaRepository, OrdenRecogidaRepository>();
builder.Services.AddScoped<IOrdenReposicionRepository, OrdenReposicionRepository>();
builder.Services.AddScoped<IDemarcaRepository, DemarcaRepository>();
builder.Services.AddScoped<IMovimientoStockRepository, MovimientoStockRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAngular");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await SeedData.InicializarRolesYUsuariosAsync(services);
}

app.Run();