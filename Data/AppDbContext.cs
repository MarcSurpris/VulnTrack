using Microsoft.EntityFrameworkCore;
using VulnTrack.Models;

namespace VulnTrack.Data;

// AppDbContext is EF Core's representation of the database.
// Each DbSet<T> is a table. Querying _db.Vulnerabilities runs SQL under the hood.
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Vulnerability> Vulnerabilities => Set<Vulnerability>();
    public DbSet<User> Users => Set<User>();
}
