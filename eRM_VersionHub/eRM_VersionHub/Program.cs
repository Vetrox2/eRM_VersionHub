using Dapper;
using eRM_VersionHub.Data;
using eRM_VersionHub.Models;
using eRM_VersionHub.Repositories.Database;
using eRM_VersionHub.Repositories.Interfaces;
using eRM_VersionHub.Services;
using eRM_VersionHub.Services.Database;
using eRM_VersionHub.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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

builder.Services.AddScoped<IAppDataScanner, AppDataScanner>();

var app = builder.Build();
app.UseCors(builder =>
{
    builder.WithOrigins("http://localhost:4200").AllowAnyHeader().AllowAnyMethod();
});

var db = new DbInitializer(app);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

PackagesGenerator.Generate(); //To delete

app.Run();
