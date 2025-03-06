using Firebase.Auth;
using Firebase.Auth.Providers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using PNE_core.Services;
using PNE_core.Enums;
using PNE_core.Services.Interfaces;
using PNE_DataAccess;
using NetTopologySuite.Geometries;
using NetTopologySuite.Geometries.Implementation;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.DataProtection;
using System.Net;

var builder = WebApplication.CreateBuilder(args);


if (File.Exists("../.env") && builder.Environment.IsDevelopment())
{
    DotNetEnv.Env.Load("../.env");
}

builder.Configuration.AddEnvironmentVariables(prefix:"PNE_");
var config = builder.Configuration;
string connectionString = config.GetConnectionString("PNE")!;

if (builder.Environment.IsProduction() && Directory.Exists("/run/secrets"))
{
    builder.WebHost.UseUrls(new[] { "http://*:8080", "https://*:8081" });

    try
    {
        config["DB_NAME"] = File.ReadAllText(config["DB_NAME"]!);
        config["DB_USER"] = File.ReadAllText(config["DB_USER"]!);
        config["DB_PASS"] = File.ReadAllText(config["DB_PASS"]!);
        config["FIREBASE_APIKEY"] = File.ReadAllText(config["FIREBASE_APIKEY"]!);
        config["FIREBASE_PROJECTNAME"] = File.ReadAllText(config["FIREBASE_PROJECTNAME"]!);
        config["GOOGLE_ID"] = File.ReadAllText(config["GOOGLE_ID"]!);
        config["GOOGLE_SECRET"] = File.ReadAllText(config["GOOGLE_SECRET"]!);

        connectionString = $"Host={config["DB_HOST"]};Port={config["DB_PORT"]};Database={config["DB_NAME"]};Username={config["DB_USER"]};Password={config["DB_PASS"]}";
    }
    catch (Exception ex) 
    {
        throw new WebException(ex.Message, WebExceptionStatus.UnknownError);
    }
}

string firebaseProjectName = $"{config["FIREBASE_PROJECTNAME"]}";

// Add services to the container.
builder.Services.AddControllersWithViews();

var npgsqlDataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
npgsqlDataSourceBuilder.MapEnum<Niveau>();
npgsqlDataSourceBuilder.MapEnum<TypeLavage>();
npgsqlDataSourceBuilder.MapEnum<TypePneId>();
npgsqlDataSourceBuilder.MapEnum<StationPersonnelStatus>();
npgsqlDataSourceBuilder.UseNetTopologySuite(
    new DotSpatialAffineCoordinateSequenceFactory(Ordinates.XYM));
var npgsqlSetup = npgsqlDataSourceBuilder.Build();

builder.Services.AddDbContext<IPneDbContext, PneContext>(opt =>
    opt.UseNpgsql(
        npgsqlSetup,
        o => {
            o.MigrationsAssembly(typeof(PneContext).Assembly.FullName);
            o.MigrationsHistoryTable("__EFMigrationsHistory", $"{config["DB_NAME"]}");
            o.UseNetTopologySuite(new DotSpatialAffineCoordinateSequenceFactory(Ordinates.XYM));
        }
    )
);

builder.Services.AddSingleton(new FirebaseAuthClient(new FirebaseAuthConfig
{
    ApiKey = $"{config["FIREBASE_APIKEY"]}",
    AuthDomain = $"{firebaseProjectName}.firebaseapp.com",
    Providers =
    [
        new EmailProvider(),
        new GoogleProvider()
    ]
}));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = $"https://securetoken.google.com/{firebaseProjectName}";
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidIssuer = $"https://securetoken.google.com/{firebaseProjectName}",
            ValidateAudience = false,
            ValidAudience = firebaseProjectName,
            ValidateLifetime = true
        };
    });

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddScoped<IFirebaseAuthService, FirebaseAuthService>();
builder.Services.AddScoped<IEmbarcationService, EmbarcationService>();
builder.Services.AddScoped<INoteDossierService, NoteDossierService>();
builder.Services.AddScoped<IUtilisateurService, UtilisateursService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IStationLavageService, StationLavageService>();
builder.Services.AddScoped<IPlanEauService, PlanEauService>();
builder.Services.AddScoped<ICertificationService, CertificationService>();
builder.Services.AddScoped<IMiseAEauService, MiseAEauService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();
app.Use(async (context, next) =>
{
    var token = context.Session.GetString("token");
    if (!string.IsNullOrEmpty(token))
    {
        context.Request.Headers.Append("Authorization", "Bearer " + token);
    }
    await next();
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
