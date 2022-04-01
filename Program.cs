using Microsoft.Extensions.Options;
using signature_lookup;
using System.ComponentModel.DataAnnotations;

var builder = WebApplication.CreateBuilder(args);

builder.Host.ConfigureAppConfiguration((hostingContext, config) =>
{
    var secretPath = Environment.GetEnvironmentVariable("EXTRA_CONFIG") is string v && v.Length > 0 ? v : "/secrets/secrets";
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


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();


app.Run();
