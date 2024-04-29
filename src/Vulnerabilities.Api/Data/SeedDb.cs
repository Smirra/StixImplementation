using Bogus;
using Vulnerabilities.Api.Models;

namespace Vulnerabilities.Api.Data;
public static class SeedDb
{
    public static void SeedVulnerabilities(VulnContext context)
    {
        var vulnerabilityFaker = new Faker<Vulnerability>()
            .RuleFor(v => v.Type, f => "vulnerability")
            .RuleFor(v => v.SpecVersion, f => "2.1")
            .RuleFor(v => v.Id, f => $"vulnerability--{f.Random.Uuid()}")
            .RuleFor(v => v.Created, f => f.Date.Past())
            .RuleFor(v => v.Modified, f => f.Date.Past())
            .RuleFor(v => v.Name, f => $"CVE-{f.Date.Between(new DateTime(2010, 1, 1), DateTime.Today).Year}-{f.Random.Number(10000, 9999999)}");

        var vulnerabilities = vulnerabilityFaker.Generate(250);

        context.Vulnerabilities.AddRange(vulnerabilities);
        context.SaveChanges();
    }
}