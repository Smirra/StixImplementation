using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Vulnerabilities.Api.Models;

namespace Vulnerabilities.Api.Data;
public class VulnDbContext(DbContextOptions<VulnDbContext> options) : IdentityDbContext(options)
{
    public DbSet<Vulnerability> Vulnerabilities { get; set; }
}