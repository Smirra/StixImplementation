using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.SwaggerGen;
using Vulnerabilities.Api.Data;
using Vulnerabilities.Api.Helpers;
using Vulnerabilities.Api.Models;
using Vulnerabilities.Api.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<VulnContext>(options =>
{
    options.UseSqlite("Data Source=vulnerabilities.db");
});
builder.Services.AddScoped<IVulnRepo, VulnRepo>();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
    options.SerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.SnakeCaseLower;
    options.SerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    options.SerializerOptions.Converters.Add(new DateTimeConverter());
});

// Force snake_case for swashbuckle.
builder.Services.AddTransient<ISerializerDataContractResolver>(_ => new JsonSerializerDataContractResolver(
    new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower, }
));

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
}

app.UseHttpsRedirection();

app.MapGet("/vulnerabilities", async (IVulnRepo repo) =>
{
    var vulnerability = await repo.GetVulnerabilities();

    return Results.Ok(vulnerability);
});

app.MapGet("/vulnerabilities/{id}", async (string id, IVulnRepo repo) =>
{
    var vulnerability = await repo.GetVulnerability(id);
    if (vulnerability == null)
    {
        return Results.NotFound();
    }

    return Results.Ok(vulnerability);
});

app.MapPost("/vulnerabilities", async (VulnerabilityDTO vulnDTO, IVulnRepo repo) =>
{
    var vulnTemp = new Vulnerability
    {
        Type = vulnDTO.Type,
        SpecVersion = vulnDTO.SpecVersion,
        Created = vulnDTO.Created,
        Modified = vulnDTO.Modified,
        Id = $"vulnerability--{Guid.NewGuid()}",
        Name = vulnDTO.Name,
        Description = vulnDTO.Description
    };

    var vulnerability = await repo.CreateVulnerability(vulnTemp);
    if (vulnerability == null)
    {
        return Results.BadRequest();
    }

    return Results.Created($"/vulnerabilities/{vulnerability.Id}", vulnerability);
});

app.MapPut("/vulnerabilities/{id}", async (
    string id,
    VulnerabilityDTO vulnDTO,
    IVulnRepo repo) =>
{
    if (!await repo.VulnerabilityExists(id))
    {
        return Results.NotFound();
    }

    var vulnerability = new Vulnerability
    {
        Type = vulnDTO.Type,
        SpecVersion = vulnDTO.SpecVersion,
        Modified = DateTime.UtcNow,
        Id = id,
        Name = vulnDTO.Name,
        Description = vulnDTO.Description
    };

    var isUpdated = await repo.UpdateVulnerability(id, vulnerability);
    if (isUpdated == null)
    {
        return Results.BadRequest();
    }

    return Results.Ok(vulnerability);
});

app.MapDelete("/vulnerabilities/{id}", async (string id, IVulnRepo repo) =>
{
    var isDeleted = await repo.DeleteVulnerability(id);
    if (!isDeleted)
    {
        return Results.NotFound();
    }

    return Results.NoContent();
});

app.Run();