using WebApplication1.Extensions;
using WebApplication1.Infrastructure.Cli;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddUserSecrets<Program>();

builder.Services.AddApplicationServices(builder.Configuration);

builder.Services.AddDatabaseServices(builder.Configuration);

builder.Services.AddRepositoryServices();

builder.Services.AddCacheServices(builder.Configuration);

builder.Services.AddConfigurationService(builder.Configuration);

builder.Services.AddValidatorServices(builder.Configuration);

builder.Host.AddSerilogLogging();

builder.Services.AddAuthServices(builder.Configuration);

builder.Services.AddObservabilityServices(builder.Configuration);

builder.Services.AddHangfireServices(builder.Configuration);

builder.Services.AddApiServices();

var app = builder.Build();

if (await CliRunner.ExecuteAsync(app, builder.Configuration, args))
    return;

app.UseApplicationPipeline();

app.Run();

