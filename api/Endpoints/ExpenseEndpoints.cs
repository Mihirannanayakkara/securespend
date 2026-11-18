using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using SecureSpend.Api.Data;
using SecureSpend.Api.Dtos;
using SecureSpend.Api.Models;
namespace SecureSpend.Api.Endpoints;
public static class ExpenseEndpoints {
    static int Uid(ClaimsPrincipal p) => int.Parse(p.FindFirstValue(ClaimTypes.NameIdentifier)!);
    static bool Invalid(ExpenseDto d) => d.Amount <= 0 || string.IsNullOrWhiteSpace(d.Title) || string.IsNullOrWhiteSpace(d.Category);

    public static void MapExpenseEndpoints(this IEndpointRouteBuilder app) {
        var g = app.MapGroup("/api/expenses").RequireAuthorization();

        g.MapGet("/", async (ClaimsPrincipal p, AppDb db) =>
            await db.Expenses.Where(e => e.UserId == Uid(p)).OrderByDescending(e => e.Date).ToListAsync());

        g.MapPost("/", async (ExpenseDto d, ClaimsPrincipal p, AppDb db) => {
            if (Invalid(d)) return Results.BadRequest(new { error = "Title, amount and category are required." });
            var e = new Expense { UserId = Uid(p), Title = d.Title.Trim(), Amount = d.Amount, Category = d.Category.Trim(), Date = d.Date.ToUniversalTime() };
            db.Expenses.Add(e); await db.SaveChangesAsync();
            return Results.Created($"/api/expenses/{e.Id}", e);
        });

        g.MapPut("/{id:int}", async (int id, ExpenseDto d, ClaimsPrincipal p, AppDb db) => {
            if (Invalid(d)) return Results.BadRequest(new { error = "Title, amount and category are required." });
            var e = await db.Expenses.FirstOrDefaultAsync(x => x.Id == id && x.UserId == Uid(p)); // owner check
            if (e is null) return Results.NotFound();
            e.Title = d.Title.Trim(); e.Amount = d.Amount; e.Category = d.Category.Trim(); e.Date = d.Date.ToUniversalTime();
            await db.SaveChangesAsync(); return Results.Ok(e);
        });

        g.MapDelete("/{id:int}", async (int id, ClaimsPrincipal p, AppDb db) => {
            var e = await db.Expenses.FirstOrDefaultAsync(x => x.Id == id && x.UserId == Uid(p));
            if (e is null) return Results.NotFound();
            db.Expenses.Remove(e); await db.SaveChangesAsync(); return Results.NoContent();
        });
    }
}
