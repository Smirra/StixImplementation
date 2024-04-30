using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Vulnerabilities.Api.Data;
using Vulnerabilities.Api.Helpers;
using Vulnerabilities.Api.Models;
using Vulnerabilities.Api.Repositories;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
        options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.SnakeCaseLower;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.Converters.Add(new DateTimeConverter());
    });

builder.Services.AddAutoMapper(typeof(VulnProfile));

builder.Services.AddDbContext<VulnContext>(options =>
{
    options.UseSqlite("Data Source=vulnerabilities.db");
});
builder.Services.AddScoped<IVulnRepo, VulnRepo>();


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<VulnContext>();
    context.Database.Migrate();
    if (!context.Vulnerabilities.Any())
    {
        SeedDb.SeedVulnerabilities(context);
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseExceptionHandler("/error-development");
}
else
{
    app.UseExceptionHandler("/error");
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();