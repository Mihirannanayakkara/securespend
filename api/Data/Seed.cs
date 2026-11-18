using Microsoft.EntityFrameworkCore;
using SecureSpend.Api.Models;
namespace SecureSpend.Api.Data;
public static class Seed {
    // Creates the admin account from Admin:Email / Admin:Password (environment variables), if set.
    public static async Task AdminAsync(AppDb db, IConfiguration cfg) {
        var email = cfg["Admin:Email"]?.Trim().ToLowerInvariant(); var pw = cfg["Admin:Password"];
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(pw) || await db.Users.AnyAsync(u => u.Email == email)) return;
        db.Users.Add(new User { Email = email, PasswordHash = BCrypt.Net.BCrypt.HashPassword(pw), Role = "Admin" });
        await db.SaveChangesAsync();
    }
}
