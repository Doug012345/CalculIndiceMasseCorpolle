//// Program.cs
//using CalculIndiceMasseCorpolle.Models;
//using CalculIndiceMasseCorpolle.Services;
//using Microsoft.AspNetCore.Builder;
//using Microsoft.EntityFrameworkCore;

//var builder = WebApplication.CreateBuilder(args);

//// Configuration basée sur l'environnement
//builder.Configuration
//    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
//    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
//    .AddEnvironmentVariables();

//builder.Services.AddControllers();
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

//// Configuration de la base de données avec gestion d'erreurs
//builder.Services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
//{
//    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

//    if (string.IsNullOrEmpty(connectionString))
//    {
//        throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
//    }

//    // Remplacer les variables d'environnement dans la chaîne de connexion
//    connectionString = connectionString
//        .Replace("${SQL_SERVER}", Environment.GetEnvironmentVariable("SQL_SERVER") ?? "")
//        .Replace("${SQL_DATABASE}", Environment.GetEnvironmentVariable("SQL_DATABASE") ?? "")
//        .Replace("${SQL_USER}", Environment.GetEnvironmentVariable("SQL_USER") ?? "")
//        .Replace("${SQL_PASSWORD}", Environment.GetEnvironmentVariable("SQL_PASSWORD") ?? "");

//    options.UseSqlServer(connectionString, sqlOptions =>
//    {
//        sqlOptions.EnableRetryOnFailure(
//            maxRetryCount: 5,
//            maxRetryDelay: TimeSpan.FromSeconds(30),
//            errorNumbersToAdd: null);

//        if (builder.Environment.IsProduction())
//        {
//            sqlOptions.CommandTimeout(60);
//        }
//    });

//    if (builder.Environment.IsDevelopment())
//    {
//        options.EnableSensitiveDataLogging();
//        options.EnableDetailedErrors();
//    }
//});

//// Injection des services
//builder.Services.AddScoped<IIMCService, IMCService>();

//// Configuration CORS
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowAll", policy =>
//    {
//        policy.AllowAnyOrigin()
//              .AllowAnyMethod()
//              .AllowAnyHeader();
//    });
//});

//var app = builder.Build();

//// Initialisation de la base de données
//try
//{
//    using (var scope = app.Services.CreateScope())
//    {
//        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

//        if (app.Environment.IsDevelopment())
//        {
//            // Migration automatique en développement
//            await context.Database.MigrateAsync();
//            Console.WriteLine("Database migrations applied successfully.");
//        }
//        else
//        {
//            // Juste tester la connexion en production
//            var canConnect = await context.Database.CanConnectAsync();
//            if (canConnect)
//            {
//                Console.WriteLine("Database connection successful.");
//            }
//            else
//            {
//                Console.WriteLine("Warning: Cannot connect to database.");
//            }
//        }
//    }
//}
//catch (Exception ex)
//{
//    Console.WriteLine($"Database initialization error: {ex.Message}");
//    if (app.Environment.IsDevelopment())
//    {
//        Console.WriteLine($"Stack trace: {ex.StackTrace}");
//    }
//}

//// Configuration du pipeline HTTP
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI(options =>
//    {
//        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Indice Corporel API v1");
//        options.RoutePrefix = string.Empty; // Pour avoir Swagger à la racine
//    });

//    app.UseDeveloperExceptionPage();
//}
//else
//{
//    app.UseExceptionHandler("/error");
//    app.UseHsts();
//}

//app.UseCors("AllowAll");
//app.UseHttpsRedirection();
//app.UseAuthorization();
//app.MapControllers();

//// Endpoint pour la santé de l'application
//app.MapGet("/health", () => Results.Ok(new { status = "Healthy", timestamp = DateTime.UtcNow }));

//app.Run();

// Program.cs
using CalculIndiceMasseCorpolle.Models;
using CalculIndiceMasseCorpolle.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configuration
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

// Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "API Calcul IMC",
        Version = "v1",
        Description = "API pour calculer l'Indice de Masse Corporelle"
    });
});

// Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Custom Services
builder.Services.AddScoped<IIMCService, IMCService>();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Calcul IMC v1");
        c.RoutePrefix = string.Empty; // Pour avoir Swagger à la racine
    });
}

app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Endpoint de santé
app.MapGet("/health", () => Results.Ok(new
{
    status = "Healthy",
    service = "API Calcul IMC",
    timestamp = DateTime.UtcNow
}));

// Redirection de la racine vers Swagger
app.MapGet("/", () => Results.Redirect("/swagger"));

Console.WriteLine("✅ API Calcul IMC démarrée!");
Console.WriteLine("📍 Swagger UI: http://localhost:5000");
Console.WriteLine("🎯 Contrôleur IMC: /api/imc");

app.Run();