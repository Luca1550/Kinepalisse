using Dapper;
using Kinepalisse.Api.Data;
using Kinepalisse.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<DbConnectionFactory>();
builder.Services.AddScoped<FilmService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
app.MapControllers();
app.Run();
