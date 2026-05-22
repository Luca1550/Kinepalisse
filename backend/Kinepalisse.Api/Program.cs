using Dapper;
using Kinepalisse.Api.Auth;
using Kinepalisse.Api.Data;
using Kinepalisse.Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// La factory de connexions (sans état mutable → singleton OK)
builder.Services.AddSingleton<DbConnectionFactory>();

// AddScoped = .NET crée une instance par requête HTTP
builder.Services.AddScoped<FilmService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<SalleService>();
builder.Services.AddScoped<TarifService>();
builder.Services.AddScoped<SeanceService>();
builder.Services.AddScoped<ReservationService>();

var jwtConfig = builder.Configuration.GetSection("Jwt");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer           = true,
            ValidateAudience         = true,
            ValidateLifetime         = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer    = jwtConfig["Issuer"],
            ValidAudience  = jwtConfig["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtConfig["Key"]!))
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    // Ajoute le bouton "Authorize" dans Swagger UI pour tester les routes protégées
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Colle ici ton token JWT (sans écrire 'Bearer')"
    });
    // doc = le document OpenAPI en cours de génération, nécessaire pour créer la référence
    opt.AddSecurityRequirement(doc => new OpenApiSecurityRequirement
    {
        { new OpenApiSecuritySchemeReference("Bearer", doc), new List<string>() }
    });
});

builder.Services.AddCors(o =>
    o.AddPolicy("Front", p => p
        .WithOrigins("http://localhost:4200")
        .AllowAnyHeader()
        .AllowAnyMethod()));

var app = builder.Build();

// Dapper : mappe automatiquement les colonnes snake_case vers les propriétés PascalCase
DefaultTypeMap.MatchNamesWithUnderscores = true;

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("Front");
// Ordre important : UseAuthentication (qui es-tu ?) AVANT UseAuthorization (as-tu le droit ?)
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
