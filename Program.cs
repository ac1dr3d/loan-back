using Serilog;
using FluentMigrator.Runner;
using LoanBack.Helpers;


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

string fullConnFromConfig = builder.Configuration.GetConnectionString("DefaultConnection") ?? "";
string dbName = "loan_app";

var (serverConn, dbConn) = DbConnectionHelper.BuildBoth(fullConnFromConfig, dbName);


// Ensure DB exists
DatabaseBootstrapper.EnsureDatabase(serverConn, dbName);


// Run FluentMigrator with full connection string
builder.Services.AddFluentMigratorCore()
.ConfigureRunner(rb => rb
    .AddMySql5()
    .WithGlobalConnectionString(dbConn)
    .ScanIn(typeof(CreateUsersTable).Assembly).For.Migrations())
.AddLogging(lb => lb.AddFluentMigratorConsole());


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
    runner.MigrateUp();
}





// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// app.UseAuthorization();

app.MapControllers();

app.Run();


