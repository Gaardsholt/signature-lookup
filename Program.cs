using Microsoft.Extensions.Options;
using Prometheus;
using signature_lookup;

var builder = WebApplication.CreateBuilder(args);

builder.Host.ConfigureAppConfiguration((hostingContext, config) =>
{
    var secretPath = Environment.GetEnvironmentVariable("EXTRA_CONFIG");
    if (String.IsNullOrEmpty(secretPath))
        return;

    if (File.Exists(secretPath))
        config.AddJsonFile(secretPath, optional: false, reloadOnChange: false);
});

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<Credentials>(builder.Configuration.GetSection(nameof(Credentials)));

// Make sure to validate settings on startup
builder.Services.AddTransient<IStartupFilter, SettingValidationStartupFilter>();
builder.Services.AddSingleton<IValidatable>(resolver => resolver.GetRequiredService<IOptions<Credentials>>().Value);

builder.Services.AddHealthChecks();


var app = builder.Build();

app.MapHealthChecks("/healthz");
app.MapMetrics();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();


app.Run();
