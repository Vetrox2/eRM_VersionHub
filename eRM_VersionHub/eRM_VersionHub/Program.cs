using Dapper;
using eRM_VersionHub.Data;
using eRM_VersionHub.Models;
using eRM_VersionHub.Repositories;
using eRM_VersionHub.Repositories.Interfaces;
using eRM_VersionHub.Services;
using eRM_VersionHub.Services.Database;
using eRM_VersionHub.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Configure services
builder.Services.AddHttpLogging(o => { });

builder.Services.AddLogging(config =>
{
    config.AddConsole();
    config.AddDebug();
});

// Add services to the container.
builder.Services.AddControllers();

// Configure Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

DefaultTypeMap.MatchNamesWithUnderscores = true;

builder.Services.Configure<AppSettings>(builder.Configuration);
builder.Services.AddSingleton<IDbRepository, DbRepository>();
builder.Services.AddSingleton<IUserRepository, UserRepository>();
builder.Services.AddSingleton<IFavoriteRepository, FavoriteRepository>();
builder.Services.AddSingleton<IPermissionRepository, PermissionRepository>();

builder.Services.AddScoped<IDbService, DbService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddScoped<IFavoriteService, FavoriteService>();
builder.Services.AddScoped<IPublicationService, PublicationService>();

builder.Services.AddScoped<IAppDataScanner, AppDataScanner>();

// Configure Swagger
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "My API",
        Version = "v1"
    });

    // Configure OAuth2 for Swagger
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows
        {
            Implicit = new OpenApiOAuthFlow
            {
                AuthorizationUrl = new Uri("http://localhost:8080/realms/eRM-realm/protocol/openid-connect/auth"),
                Scopes = new Dictionary<string, string>
                {
                    { "api.read", "Read access to protected API" }
                }
            }
        }
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "oauth2"
                }
            },
            new[] { "api.read" }
        }
    });
});

// Add JWT Bearer Authentication
var keycloakSettings = builder.Configuration.GetSection("Keycloak").Get<KeycloakSettings>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.Authority = keycloakSettings.Authority;
    options.Audience = keycloakSettings.ClientId;
    options.RequireHttpsMetadata = false; // Set to true in production
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = keycloakSettings.Authority,
        ValidateAudience = true,
        ValidAudience = keycloakSettings.ClientId,
        ValidateLifetime = true,
        RoleClaimType = ClaimTypes.Role
    };

    options.Events = new JwtBearerEvents
    {
        OnTokenValidated = context =>
        {
            var userClaims = context.Principal.Identity as ClaimsIdentity;

            var clientRolesClaim = context.Principal.FindFirst("resource_access")?.Value;
            if (clientRolesClaim != null)
            {
                var resourceAccess = clientRolesClaim.Deserialize<Dictionary<string, Dictionary<string, string[]>>>();
                if (resourceAccess.ContainsKey(keycloakSettings.ClientId))
                {
                    var clientRoles = resourceAccess[keycloakSettings.ClientId]["roles"];
                    foreach (var role in clientRoles)
                    {
                        userClaims.AddClaim(new Claim(ClaimTypes.Role, role));
                    }
                }
            }

            return Task.CompletedTask;
        }
    };
});

var app = builder.Build();

// Configure middleware
app.UseHttpLogging();

app.UseCors(builder =>
{
    builder.WithOrigins("http://localhost:4200").AllowAnyHeader().AllowAnyMethod();
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        c.OAuthClientId("swaggerUI");
        c.OAuthAppName("My API");
        c.OAuthUsePkce(); // Recommended for OAuth2
    });
}

app.UseHttpsRedirection();

// Enable authentication and authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

//AppSettings? settings = app.Services.CreateScope().ServiceProvider.GetService<IOptions<AppSettings>>().Value;
// if (settings != null)
//     PackagesGenerator.Generate(settings.MyAppSettings.InternalPackagesPath, Path.Combine(Directory.GetParent(settings.MyAppSettings.AppsPath).Name, "packages.txt")); // To delete

app.Run();

public partial class Program { }
