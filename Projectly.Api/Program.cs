using Microsoft.AspNetCore.Authentication.JwtBearer; // Para JwtBearerDefaults
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models; // Para OpenApiInfo, OpenApiSecurityScheme, etc.
using Projectly.Api.Models;
using Projectly.Api.Services;
using Projectly.Shared.Dal;
using Projectly.Shared.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 1. Configurar JwtSettings
var jwtSettings = new JwtSettings();
builder.Configuration.GetSection("JwtSettings").Bind(jwtSettings);
builder.Services.AddSingleton(jwtSettings);

// 2. Añadir servicios al contenedor
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// 3. Configurar Swagger con soporte para JWT
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Projectly API",
        Version = "v1",
        Description = "API para gestión de proyectos con autenticación JWT"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Autenticación JWT usando Bearer token.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// 4. Configurar Entity Framework
builder.Services.AddDbContextFactory<ProjectlyDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// 5. Configurar autenticación JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
            ValidateIssuer = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtSettings.Audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// 6. Registrar tus servicios
builder.Services.AddProjectlyServices();
builder.Services.AddScoped<JwtService>();

var app = builder.Build();

// 7. Configurar el pipeline de HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Projectly API V1");
        c.RoutePrefix = "swagger";
    });
}

app.MapGet("/", () => "Projectly API is running!");

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();