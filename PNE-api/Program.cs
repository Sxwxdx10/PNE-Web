using Microsoft.EntityFrameworkCore;
using Npgsql;
using PNE_core.Enums;
using PNE_core.Services.Interfaces;
using PNE_DataAccess;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

if (File.Exists("../.env") && builder.Environment.IsDevelopment())
{
    DotNetEnv.Env.Load("../.env");
}

builder.Configuration.AddEnvironmentVariables(prefix: "PNE_");
var config = builder.Configuration;
string connectionString = config.GetConnectionString("PNE")!;

if (builder.Environment.IsProduction() && Directory.Exists("/run/secrets"))
{
    builder.WebHost.UseUrls(new[] { "http://*:5000", "https://*:5001" });

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

builder.Services.AddControllers();

var npgsqlDataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
npgsqlDataSourceBuilder.MapEnum<Niveau>();
npgsqlDataSourceBuilder.MapEnum<TypeLavage>();
npgsqlDataSourceBuilder.MapEnum<TypePneId>();
npgsqlDataSourceBuilder.UseNetTopologySuite();
var npgsqlSetup = npgsqlDataSourceBuilder.Build();

builder.Services.AddDbContext<IPneDbContext, PneContext>(opt =>
    opt.UseNpgsql(
        npgsqlSetup,
        o => {
            o.MigrationsAssembly(typeof(PneContext).Assembly.FullName);
            o.MigrationsHistoryTable("__EFMigrationsHistory", $"{config["DB_NAME"]}");
            o.UseNetTopologySuite();
        }
    )
);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
