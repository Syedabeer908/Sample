using FluentValidation;
using FluentValidation.AspNetCore;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using System.Diagnostics;
using Serilog;
using StackExchange.Redis;
using System.Security.Claims;
using System.Text;
using WebApplication1;
using WebApplication1.Common.Constants;
using WebApplication1.Entities;
using WebApplication1.Interfaces;
using WebApplication1.Jobs;
using WebApplication1.Middlewares;
using WebApplication1.Repository;
using WebApplication1.Seeders;
using WebApplication1.Services;
using WebApplication1.Settings;
using WebApplication1.Validator;


var builder = WebApplication.CreateBuilder(args);

// Add User Secret
builder.Configuration.AddUserSecrets<Program>();

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Adding DbContext with configuration using appsettings.json
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register Redis ConnectionMultiplexer (Singleton)
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    
    var configuration = builder.Configuration["Redis:ConnectionString"];
    var options = ConfigurationOptions.Parse(configuration);
    options.AbortOnConnectFail = false; 

    return ConnectionMultiplexer.Connect(options);
});


// Adding AuthConstants
builder.Services.Configure<AuthSettings>(
    builder.Configuration.GetSection("SecretKeys:Jwt")
);

// Adding RoleConstants
builder.Services.Configure<RoleSettings>(
    builder.Configuration.GetSection("StartUpRole")
);

// Adding FileSettings
builder.Services.Configure<FileSettings>(
    builder.Configuration.GetSection("FileStorage")
);

// Adding EmailSettings
builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings")
);

// Adding GoogleSettings
builder.Services.Configure<GoogleSettings>(
    builder.Configuration.GetSection("GoogleOAuth")
);

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole(builder.Configuration["StartUpRole:Admin"]));
});

builder.Services
    .AddFluentValidationAutoValidation()
    .AddFluentValidationClientsideAdapters();

builder.Services.AddValidatorsFromAssemblyContaining<UploadProfileImageRequestValidator>();

// Adding scoped (DI) 
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IProviderRepository, ProviderRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IRepository<Risk>, RiskRepository>();
builder.Services.AddScoped<IRepository<Control>, ControlRepository>();
builder.Services.AddScoped<IRepository<RiskControl>, RiskControlRepository>();
builder.Services.AddScoped<ISoftRepository, SoftRepository>();
builder.Services.AddScoped<IUserLoginHistoryRepository, UserLoginHistoryRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<UserDomainService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<AdminService>();
builder.Services.AddScoped<RoleService>();
builder.Services.AddScoped<RiskService>();
builder.Services.AddScoped<ControlService>();
builder.Services.AddScoped<RiskControlService>();
builder.Services.AddScoped<RedisService>();
builder.Services.AddScoped<ProfileService>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<FileService>();
builder.Services.AddScoped<ProviderService>();
builder.Services.AddScoped<GoogleService>();
builder.Services.AddScoped<AuthBackgroundJobs>();

builder.Services.AddScoped<RoleSettings>();
builder.Services.AddScoped<EmailSettings>();
builder.Services.AddScoped<FileSettings>();
builder.Services.AddScoped<AuthSettings>();
builder.Services.AddScoped<GoogleSettings>();

builder.Services.AddHangfire(config =>
    config.UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.AddHangfireServer();

var secretKey = Encoding.UTF8.GetBytes(builder.Configuration["SecretKeys:Jwt:Key"]);

// Add Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(secretKey),
        ClockSkew = TimeSpan.Zero,
        RoleClaimType = ClaimTypes.Role,
        NameClaimType = ClaimTypes.NameIdentifier
    };
});

// swagger configuration with JWT support
builder.Services.AddSwaggerGen(c =>
{
    // Add JWT Authentication to Swagger
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter JWT token as: Bearer {your token here}"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File(
        path: "Logs/log-.txt",
        rollingInterval: RollingInterval.Day, // new file every day
        retainedFileCountLimit: 7             // keep last 7 days
    )
    .CreateLogger();

// Replace default logging
builder.Host.UseSerilog();

// Adding Api Versioning
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});
builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

// swagerr configuration class
builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();

builder.Services.AddHealthChecks()
    // =========================
    // SQL SERVER HEALTH CHECK
    // =========================
    .AddSqlServer(
        connectionString: builder.Configuration.GetConnectionString("DefaultConnection"),
        healthQuery: "SELECT 1",
        name: "sql",
        failureStatus: HealthStatus.Unhealthy,
        tags: new[] { "db", "sql" })

    // =========================
    // REDIS HEALTH CHECK (custom for latency)
    // =========================
    .AddCheck("redis_latency", () =>
    {
        try
        {
            var redisConn = builder.Configuration["Redis:ConnectionString"];
            var sw = Stopwatch.StartNew();

            using var connection = ConnectionMultiplexer.Connect(redisConn);
            var db = connection.GetDatabase();

            // simple PING test with latency measurement
            var result = db.Ping();
            sw.Stop();

            var latencyMs = sw.ElapsedMilliseconds;

            if (latencyMs > 200)
            {
                return HealthCheckResult.Degraded($"Redis slow: {latencyMs}ms");
            }

            return HealthCheckResult.Healthy($"Redis OK: {latencyMs}ms");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Redis failed", ex);
        }
    },
    tags: new[] { "redis", "cache" });

var app = builder.Build();

//seeding data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    
    context.Database.Migrate();

    var roleSettings = scope.ServiceProvider.GetRequiredService<IOptions<RoleSettings>>();
    var seeder = new DbSeeder(context, roleSettings);

    await seeder.SeedAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        foreach (var desc in provider.ApiVersionDescriptions)
        {
            options.SwaggerEndpoint(
                $"/swagger/{desc.GroupName}/swagger.json",
                desc.GroupName.ToUpperInvariant()
            );
        }
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseMiddleware<UserValidationMiddleware>();

app.UseMiddleware<ExceptionMiddleware>();

app.UseAuthorization();

// Adding Hangfire Dashboard
app.UseHangfireDashboard();

// expose detailed health endpoints
app.MapHealthChecks("/health"); // full

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = r =>
        r.Tags.Contains("db") ||
        r.Tags.Contains("sql") ||
        r.Tags.Contains("redis") ||
        r.Tags.Contains("cache")
});

app.MapHealthChecks("/health/live");

app.MapControllers();

app.Run();

