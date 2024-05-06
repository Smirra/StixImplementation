using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Vulnerabilities.Api.Data;
using Vulnerabilities.Api.MappingProfiles;
using Vulnerabilities.Api.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authorization;
using Vulnerabilities.Api;
using Vulnerabilities.Api.Converters;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            Constants.SecurityScheme,
            Array.Empty<string>()
        }
    });
    options.AddSecurityDefinition(Constants.SecurityScheme.Reference.Id, Constants.SecurityScheme);
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
        options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.SnakeCaseLower;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.Converters.Add(new DateTimeConverter());
    });

builder.Services.AddAutoMapper(typeof(VulnProfile));

builder.Services.AddDbContext<VulnDbContext>(options =>
{
    options.UseSqlite("Data Source=vulnerabilities.db");
});
builder.Services.AddScoped<IVulnRepo, VulnRepo>();

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.User.RequireUniqueEmail = true;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
})
    .AddRoles<IdentityRole>()
    .AddRoleManager<RoleManager<IdentityRole>>()
    .AddEntityFrameworkStores<VulnDbContext>();

var secret = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]
                                ?? throw new InvalidOperationException("A Jwt secret needs to be set."));
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(secret),
        };
    });

builder.Services.AddAuthorizationBuilder()
                .SetDefaultPolicy(new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser()
                    .Build());

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<VulnDbContext>();
    context.Database.Migrate();
    if (!context.Vulnerabilities.Any())
    {
        SeedDb.SeedVulnerabilities(context);
    }

    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    if (!roleManager.Roles.Any())
    {
        SeedDb.SeedRoles(roleManager);
    }

    var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
    if (!userManager.Users.Any())
    {
        SeedDb.SeedUsers(userManager);
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
    app.UseHttpsRedirection();
    app.UseExceptionHandler("/error");
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();


app.Run();

public partial class Program { }