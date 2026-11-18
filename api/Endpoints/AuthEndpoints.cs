using Microsoft.EntityFrameworkCore;
using SecureSpend.Api.Data;
using SecureSpend.Api.Dtos;
using SecureSpend.Api.Models;
using SecureSpend.Api.Services;
namespace SecureSpend.Api.Endpoints;
public static class AuthEndpoints {
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app) {
        var g = app.MapGroup("/api/auth").RequireRateLimiting("auth");

        g.MapPost("/register", async (AuthDto d, AppDb db, TokenService tokens) => {
            if (string.IsNullOrWhiteSpace(d.Email) || !d.Email.Contains('@') || (d.Password ?? "").Length < 8)
                return Results.BadRequest(new { error = "Valid email and a password of 8+ characters are required." });
            var email = d.Email.Trim().ToLowerInvariant();
            if (await db.Users.AnyAsync(u => u.Email == email)) return Results.Conflict(new { error = "Email already registered." });
            var user = new User { Email = email, PasswordHash = BCrypt.Net.BCrypt.HashPassword(d.Password) };
            db.Users.Add(user); await db.SaveChangesAsync();
            return Results.Ok(new { token = tokens.Create(user), role = user.Role });
        });

        g.MapPost("/login", async (AuthDto d, AppDb db, TokenService tokens) => {
            var email = (d.Email ?? "").Trim().ToLowerInvariant();
            var user = await db.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user is null || !BCrypt.Net.BCrypt.Verify(d.Password ?? "", user.PasswordHash)) return Results.Unauthorized();
            return Results.Ok(new { token = tokens.Create(user), role = user.Role });
        });
    }
}
