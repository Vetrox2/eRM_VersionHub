using Dapper;
using eRM_VersionHub.Data;
using eRM_VersionHub.Models;
using eRM_VersionHub.Repositories;
using eRM_VersionHub.Repositories.Interfaces;
using eRM_VersionHub.Services;
using eRM_VersionHub.Services.Database;
using eRM_VersionHub.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text.Json;

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

DefaultTypeMap.MatchNamesWithUnderscores = true;

builder.Services.Configure<AppSettings>(builder.Configuration);
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<IAppStructureCache, AppStructureCache>();
builder.Services.AddSingleton<IDbRepository, DbRepository>();
builder.Services.AddSingleton<IUserRepository, UserRepository>();
builder.Services.AddSingleton<IFavoriteRepository, FavoriteRepository>();
builder.Services.AddSingleton<IPermissionRepository, PermissionRepository>();
builder.Services.AddSingleton<ISqlConnectionFactory, SqlConnectionFactory>();

builder.Services.AddScoped<IDbService, DbService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddScoped<IFavoriteService, FavoriteService>();
builder.Services.AddScoped<IPublicationService, PublicationService>();
builder.Services.AddScoped<IAppDataScanner, AppDataScanner>();

builder.Services.AddHostedService<FileChangeWatcher>();

// Configure Swagger
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
    options.AddSecurityDefinition("Keycloak", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows
        {
            Implicit = new OpenApiOAuthFlow
            {
                AuthorizationUrl = new Uri("http://localhost:8080/realms/eRM-realm/protocol/openid-connect/auth"),
            }
        }
    });

    OpenApiSecurityScheme keycloakSecurityScheme = new()
    {
        Reference = new OpenApiReference
        {
            Id = "Keycloak",
            Type = ReferenceType.SecurityScheme,
        },
        In = ParameterLocation.Header,
        Name = "Bearer",
        Scheme = "Bearer",
    };

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { keycloakSecurityScheme, Array.Empty<string>() },
    });
});

// Add JWT Bearer Authentication
var keycloakSettings = builder.Configuration.GetSection("Keycloak").Get<KeycloakSettings>();

builder.Services
        .AddAuthentication()
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.Authority = keycloakSettings.Authority;
            options.MetadataAddress = keycloakSettings.MetadataAddress;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = keycloakSettings.Authority,
                ValidateAudience = true,
                ValidAudience = keycloakSettings.ClientId,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                RoleClaimType = ClaimTypes.Role,
                NameClaimType = "preferred_username"
            };
            options.Events = new JwtBearerEvents
            {
                OnTokenValidated = context =>
                {
                    var userClaims = context.Principal.Identity as ClaimsIdentity;

                    // Extract the realm_access claim and parse roles
                    var realmAccessClaim = context.Principal.FindFirst("realm_access")?.Value;
                    if (realmAccessClaim != null)
                    {
                        var realmAccess = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(realmAccessClaim);
                        if (realmAccess.TryGetValue("roles", out var rolesElement))
                        {
                            var roles = rolesElement.EnumerateArray().Select(role => role.GetString()).ToList();
                            foreach (var role in roles)
                            {
                                userClaims.AddClaim(new Claim(ClaimTypes.Role, role));
                            }
                        }
                    }

                    return Task.CompletedTask;
                }
            };
        });

builder.Services.AddAuthorization(o =>
{
    o.DefaultPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .RequireClaim("email_verified", "true")
        .Build();
});

// Build app
var app = builder.Build();

// Configure middleware
app.UseHttpLogging();

app.UseCors(builder =>
{
    builder
        .SetIsOriginAllowed(origin =>
        {
            var host = new Uri(origin).Host;
            return host == "localhost" || host == "127.0.0.1";
        })
        .AllowAnyHeader()
        .AllowAnyMethod();
});

// Turn on swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
        options.OAuthClientId("swaggerUI");
        options.OAuthUsePkce();
        options.OAuthScopeSeparator(" ");
        options.OAuthAdditionalQueryStringParams(new Dictionary<string, string> { { "prompt", "login" } });
    });

    var logger = app.Services.GetRequiredService<ILogger<DbInitializer>>();
    var db = new DbInitializer(app, logger);

    AppSettings? settings = app.Services.CreateScope().ServiceProvider.GetService<IOptions<AppSettings>>().Value;
    if (settings != null)
        PackagesGenerator.Generate(settings.MyAppSettings.InternalPackagesPath, Path.Combine(Directory.GetParent(settings.MyAppSettings.AppsPath).Name, "packages.txt")); // To delete

}

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

// Enable authentication and authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }