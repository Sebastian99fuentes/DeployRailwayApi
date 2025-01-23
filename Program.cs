using ApiDeployReservas.Data;
using ApiDeployReservas.Data.Models;
using ApiDeployReservas.Repository;
using ApiDeployReservas.Repository.Interfaces;
using ApiDeployReservas.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Configuración del puerto para ejecutar la aplicación. Toma un valor desde una variable de entorno o usa 8080 como predeterminado.
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://*:{port}");

// Agrega servicios básicos al contenedor.
builder.Services.AddHealthChecks(); // Agrega un endpoint para verificar el estado de la aplicación.
builder.Services.AddControllers(); // Habilita el uso de controladores para manejar solicitudes HTTP.
builder.Services.AddEndpointsApiExplorer(); // Habilita la exploración de endpoints para documentación automática.

// Configuración opcional para habilitar CORS (permitir solicitudes desde un frontend específico).
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowFrontend", policy =>
//    {
//        policy.WithOrigins("https://deploy-vercer-front-qxswtvoch-sebastians-projects-cdbb03df.vercel.app")  // Cambia por la URL real de tu frontend
//              .AllowAnyMethod()
//              .AllowCredentials() // Permitir cookies y autenticación basada en credenciales
//              .AllowAnyHeader();
//    });
//});

// Configuración de Swagger para documentar la API.
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" }); // Define el documento principal.
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme // Configuración para usar JWT en Swagger.
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token", // Mensaje para los usuarios.
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement // Requiere autenticación para ciertas operaciones.
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});

// Configuración para manejar ciclos de referencia al serializar datos JSON.
builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
});

// Configuración de Entity Framework para usar una base de datos PostgreSQL.
builder.Services.AddDbContext<ApplicationDBContext>(options => {
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Configuración de Identity para manejo de usuarios y roles.
builder.Services.AddIdentity<AppUser, IdentityRole>(options => {
    options.Password.RequireDigit = true; // La contraseña debe tener al menos un número.
    options.Password.RequireLowercase = true; // La contraseña debe tener al menos una letra minúscula.
    options.Password.RequireUppercase = true; // La contraseña debe tener al menos una letra mayúscula.
    options.Password.RequiredLength = 10; // La contraseña debe tener al menos 10 caracteres.
})
.AddEntityFrameworkStores<ApplicationDBContext>(); // Usa el contexto de base de datos para almacenar datos de usuario.

// Configuración de autenticación basada en JWT.
builder.Services.AddAuthentication(Options => {
    Options.DefaultAuthenticateScheme =
    Options.DefaultChallengeScheme =
    Options.DefaultForbidScheme =
    Options.DefaultScheme =
    Options.DefaultSignInScheme =
    Options.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["JWT:Issuer"], // Valida el emisor del token.
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JWT:Audience"], // Valida la audiencia del token.
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes(builder.Configuration["JWT:SigningKey"]) // Llave para firmar el token.
            )
    };
});

// Inyección de dependencias para servicios y repositorios personalizados.
builder.Services.AddScoped<ItokenService, TokenService>();
builder.Services.AddScoped<IAreaRepository, AreaRepository>();
builder.Services.AddScoped<IImplementosRepository, ImplementoRepository>();
builder.Services.AddScoped<IReservaAreaRepository, ReservaAreaRepository>();
builder.Services.AddScoped<IReservasImplementosRepository, ReservasImplementosRepository>();

var app = builder.Build();

// Configuración del pipeline de la aplicación.
// Habilita Swagger en cualquier entorno.
app.UseSwagger();
app.UseSwaggerUI();

// Habilita políticas de CORS.
app.UseCors(x => x
    .AllowAnyOrigin() // Permite solicitudes desde cualquier origen.
    .AllowAnyMethod() // Permite cualquier método HTTP (GET, POST, etc.).
    .AllowAnyHeader()); // Permite cualquier encabezado.


app.UseHttpsRedirection();

// Middleware para autenticación y autorización.
app.UseAuthentication();
app.UseAuthorization();

// Mapeo de controladores a rutas.
app.MapControllers();

// Endpoint para verificar la salud de la aplicación.
app.UseHealthChecks("/health");

// Inicia la aplicación.
app.Run();