using Microsoft.EntityFrameworkCore;
using SecureSpend.Api.Data;
namespace SecureSpend.Api.Endpoints;
// Admins see aggregate numbers only, never another user's individual expenses.
public static class AdminEndpoints {
    public static void MapAdminEndpoints(this IEndpointRouteBuilder app) {
        var g = app.MapGroup("/api/admin").RequireAuthorization(p => p.RequireRole("Admin"));

        g.MapGet("/stats", async (AppDb db) => new {
            users = await db.Users.CountAsync(),
            expenses = await db.Expenses.CountAsync(),
            total = await db.Expenses.SumAsync(e => e.Amount)
        });

        g.MapGet("/users", async (AppDb db) => await db.Users.OrderBy(u => u.Id)
            .Select(u => new { u.Id, u.Email, u.Role, expenseCount = db.Expenses.Count(e => e.UserId == u.Id) }).ToListAsync());
    }
}
