using Bogus;
using Microsoft.AspNetCore.Identity;
using Vulnerabilities.Api.Models;

namespace Vulnerabilities.Api.Data;
public static class SeedDb
{
    public static void SeedVulnerabilities(VulnDbContext context)
    {
        var vulnerabilityFaker = new Faker<Vulnerability>()
            .RuleFor(v => v.Type, f => "vulnerability")
            .RuleFor(v => v.SpecVersion, f => "2.1")
            .RuleFor(v => v.Id, f => $"vulnerability--{f.Random.Uuid()}")
            .RuleFor(v => v.Created, f => f.Date.Past())
            .RuleFor(v => v.Modified, f => f.Date.Past())
            .RuleFor(v => v.Name, f => $"CVE-{f.Date.Between(new DateTime(2010, 1, 1), DateTime.Today).Year}-{f.Random.Number(10000, 9999999)}")
            .RuleFor(v => v.Description, f => f.Lorem.Sentence(30))
            .RuleFor(v => v.MagnussonSeverity, f => f.Random.Number(0, 100))
            .RuleFor(v => v.Status, f => f.PickRandom("open", "resolved", "dismissed"));

        var vulnerabilities = vulnerabilityFaker.Generate(250);

        context.Vulnerabilities.AddRange(vulnerabilities);
        context.SaveChanges();
    }

    public static async void SeedRoles(RoleManager<IdentityRole> roleManager)
    {
        await roleManager.CreateAsync(new IdentityRole(Constants.Roles.Reader));
        await roleManager.CreateAsync(new IdentityRole(Constants.Roles.Editor));
    }

    public static async void SeedUsers(UserManager<IdentityUser> userManager)
    {
        var userTemplate = new IdentityUser
        {
            UserName = "reader",
            Email = "reader@reader.read"
        };
        await userManager.CreateAsync(userTemplate, "password");
        var user = await userManager.FindByNameAsync(userTemplate.UserName);
        await userManager.AddToRoleAsync(user!, Constants.Roles.Reader);

        userTemplate = new IdentityUser
        {
            UserName = "editor",
            Email = "editor@editor.edit"
        };
        await userManager.CreateAsync(userTemplate, "password");
        user = await userManager.FindByNameAsync(userTemplate.UserName);
        await userManager.AddToRoleAsync(user!, Constants.Roles.Editor);
    }
}