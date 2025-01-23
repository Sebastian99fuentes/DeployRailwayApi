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

// Configuraci�n del puerto para ejecutar la aplicaci�n. Toma un valor desde una variable de entorno o usa 8080 como predeterminado.
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://*:{port}");

// Agrega servicios b�sicos al contenedor.
builder.Services.AddHealthChecks(); // Agrega un endpoint para verificar el estado de la aplicaci�n.
builder.Services.AddControllers(); // Habilita el uso de controladores para manejar solicitudes HTTP.
builder.Services.AddEndpointsApiExplorer(); // Habilita la exploraci�n de endpoints para documentaci�n autom�tica.

// Configuraci�n opcional para habilitar CORS (permitir solicitudes desde un frontend espec�fico).
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowFrontend", policy =>
//    {
//        policy.WithOrigins("https://deploy-vercer-front-qxswtvoch-sebastians-projects-cdbb03df.vercel.app")  // Cambia por la URL real de tu frontend
//              .AllowAnyMethod()
//              .AllowCredentials() // Permitir cookies y autenticaci�n basada en credenciales
//              .AllowAnyHeader();
//    });
//});

// Configuraci�n de Swagger para documentar la API.
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" }); // Define el documento principal.
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme // Configuraci�n para usar JWT en Swagger.
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token", // Mensaje para los usuarios.
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement // Requiere autenticaci�n para ciertas operaciones.
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

// Configuraci�n para manejar ciclos de referencia al serializar datos JSON.
builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
});

// Configuraci�n de Entity Framework para usar una base de datos PostgreSQL.
builder.Services.AddDbContext<ApplicationDBContext>(options => {
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Configuraci�n de Identity para manejo de usuarios y roles.
builder.Services.AddIdentity<AppUser, IdentityRole>(options => {
    options.Password.RequireDigit = true; // La contrase�a debe tener al menos un n�mero.
    options.Password.RequireLowercase = true; // La contrase�a debe tener al menos una letra min�scula.
    options.Password.RequireUppercase = true; // La contrase�a debe tener al menos una letra may�scula.
    options.Password.RequiredLength = 10; // La contrase�a debe tener al menos 10 caracteres.
})
.AddEntityFrameworkStores<ApplicationDBContext>(); // Usa el contexto de base de datos para almacenar datos de usuario.

// Configuraci�n de autenticaci�n basada en JWT.
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

// Inyecci�n de dependencias para servicios y repositorios personalizados.
builder.Services.AddScoped<ItokenService, TokenService>();
builder.Services.AddScoped<IAreaRepository, AreaRepository>();
builder.Services.AddScoped<IImplementosRepository, ImplementoRepository>();
builder.Services.AddScoped<IReservaAreaRepository, ReservaAreaRepository>();
builder.Services.AddScoped<IReservasImplementosRepository, ReservasImplementosRepository>();

var app = builder.Build();

// Configuraci�n del pipeline de la aplicaci�n.
// Habilita Swagger en cualquier entorno.
app.UseSwagger();
app.UseSwaggerUI();

// Habilita pol�ticas de CORS.
app.UseCors(x => x
    .AllowAnyOrigin() // Permite solicitudes desde cualquier origen.
    .AllowAnyMethod() // Permite cualquier m�todo HTTP (GET, POST, etc.).
    .AllowAnyHeader()); // Permite cualquier encabezado.


app.UseHttpsRedirection();

// Middleware para autenticaci�n y autorizaci�n.
app.UseAuthentication();
app.UseAuthorization();

// Mapeo de controladores a rutas.
app.MapControllers();

// Endpoint para verificar la salud de la aplicaci�n.
app.UseHealthChecks("/health");

// Inicia la aplicaci�n.
app.Run();