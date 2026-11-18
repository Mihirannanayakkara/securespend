using Microsoft.EntityFrameworkCore;
using SecureSpend.Api.Models;
namespace SecureSpend.Api.Data;
public class AppDb(DbContextOptions<AppDb> o) : DbContext(o) {
    public DbSet<User> Users => Set<User>();
    public DbSet<Expense> Expenses => Set<Expense>();
    protected override void OnModelCreating(ModelBuilder b) {
        b.Entity<User>().HasIndex(u => u.Email).IsUnique();
        b.Entity<Expense>().Property(e => e.Amount).HasPrecision(12, 2);
    }
}
