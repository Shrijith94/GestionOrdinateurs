using GestionOrdinateurs.API.src.service;
using GestionOrdinateurs.API.src.abonnee;
using GestionOrdinateurs.API.src.data;
using GestionOrdinateurs.Shared.src.enums;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
var builder = WebApplication.CreateBuilder(args);
//test
// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSingleton<OrdinateurLogger>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorClient", policy =>
    {
        policy.WithOrigins("http://localhost:5262", "https://localhost:7088")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
// ajouter swagger JWT
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "GestionOrdinateurs",
        Version = "v1"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Entrez : Bearer {token}"
    });
    c.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
    });
});



// Configuration JWT
var jwtKey = builder.Configuration["Jwt:Key"] ?? "";
var key = Encoding.ASCII.GetBytes(jwtKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
    options.Events = new JwtBearerEvents
        {
            OnTokenValidated = context =>
            {
                Console.WriteLine($"Utilisateur utilisant un Token Valide --- {context.Principal?.Identity?.Name ?? "Anonymous"}");
                return Task.CompletedTask;
            },
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine("Token Invalide");
                return Task.CompletedTask;
            }
        };
});

// Enregistre le service en Scoped (instance par requête HTTP)
// et gère les abonnement du logger (mon abonnée) à ses évenements dès sa création
builder.Services.AddScoped<IOrdinateurService>(sp =>
{
    var service = new OrdinateurService(sp.GetRequiredService<ParcInformatiqueContext>(),sp.GetRequiredService<JournalService>());
    var logger = sp.GetRequiredService<OrdinateurLogger>();

    service.OrdinateurAjoute += logger.AfficherMessageAjout;
    service.OrdinateurSupprime += logger.AfficherMessageSuppression;
    service.OrdinateurMisAJour += logger.AfficherMessageMiseAJour;

    return service;
});
builder.Services.AddDbContext<ParcInformatiqueContext>();
builder.Services.AddScoped<ReparationService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<StockService>();
builder.Services.AddScoped<JournalService>();

var app = builder.Build();

//Initialisation de la base de données
using(var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ParcInformatiqueContext>();
    
    db.Database.Migrate();

    if(!db.Users.Any())
    {
        db.Users.AddRange(
            new User { Login="user1", Role=RoleEnum.Admin },
            new User { Login="user2", Role=RoleEnum.Gestionnaire },
            new User { Login="user3", Role=RoleEnum.Gestionnaire }
        );

        db.SaveChanges();
    }
}





// Middleware
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("AllowBlazorClient");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseHttpsRedirection();

app.Run();