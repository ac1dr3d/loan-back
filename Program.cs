using Serilog;
using FluentMigrator.Runner;
using LoanBack.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using LoanBack.Repositories;


var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
.WriteTo.Console()
.WriteTo.Seq("http://localhost:5341") // Default Seq URL
.Enrich.FromLogContext()
.CreateLogger();


builder.Host.UseSerilog();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

string fullConnFromConfig = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string is not configured.");
string dbName = "loan_app";

var (serverConn, dbConn) = DbConnectionHelper.BuildBoth(fullConnFromConfig, dbName);


// Ensure DB exists
DatabaseBootstrapper.EnsureDatabase(serverConn, dbName);


// Run FluentMigrator
builder.Services.AddFluentMigratorCore()
.ConfigureRunner(rb => rb
    .AddMySql5()
    .WithGlobalConnectionString(dbConn)
    .ScanIn(typeof(CreateUsersTable).Assembly).For.Migrations())
.AddLogging(lb => lb.AddFluentMigratorConsole());


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key is not configured.")))
        };
    });

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ILoanRepository, LoanRepository>();

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("DevCors", policy =>
    {
        policy.WithOrigins("http://localhost:1841") // Ext JS dev address
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});






var app = builder.Build();



using (var scope = app.Services.CreateScope())
{
    var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();

    // Manual migration trigger via CLI
    if (args.Contains("migrate"))
    {
        runner.MigrateUp();

        Console.WriteLine("Migrations complete.");
        return;
    }


    // Manual migration reset trigger via CLI
    if (args.Contains("migrate:reset"))
    {
        Console.WriteLine("Rolling back all migrations...");
        runner.MigrateDown(0); // Roll back to version 0

        Console.WriteLine("Reapplying all migrations...");
        runner.MigrateUp(); // Apply all

        Console.WriteLine("Database reset complete.");
        return;
    }

    // Automatic migration
    runner.MigrateUp();
}





// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors("DevCors");
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();


app.Run();


