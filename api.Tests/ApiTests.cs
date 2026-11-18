using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SecureSpend.Api.Data;
using Xunit;

public class ApiFactory : WebApplicationFactory<Program> {
    public ApiFactory() {
        Environment.SetEnvironmentVariable("Jwt__Key", "test-key-test-key-test-key-test-key-1234");
        Environment.SetEnvironmentVariable("ConnectionStrings__Default", "Host=unused");
        Environment.SetEnvironmentVariable("Rate__AuthPerMinute", "1000");
    }
    protected override void ConfigureWebHost(IWebHostBuilder b) => b.ConfigureServices(s => {
        var d = s.Single(x => x.ServiceType == typeof(DbContextOptions<AppDb>));
        s.Remove(d);
        s.AddDbContext<AppDb>(o => o.UseInMemoryDatabase("tests"));
    });
}

public class ApiTests(ApiFactory f) : IClassFixture<ApiFactory> {
    static string Email() => $"{Guid.NewGuid():N}@test.dev";

    async Task<HttpClient> LoggedIn(string email) {
        var c = f.CreateClient();
        var r = await c.PostAsJsonAsync("/api/auth/register", new { email, password = "password123" });
        r.EnsureSuccessStatusCode();
        var token = (await r.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("token").GetString();
        c.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return c;
    }

    [Fact] public async Task Register_rejects_short_password() {
        var r = await f.CreateClient().PostAsJsonAsync("/api/auth/register", new { email = Email(), password = "short" });
        Assert.Equal(HttpStatusCode.BadRequest, r.StatusCode);
    }

    [Fact] public async Task Register_rejects_duplicate_email() {
        var email = Email(); await LoggedIn(email);
        var r = await f.CreateClient().PostAsJsonAsync("/api/auth/register", new { email, password = "password123" });
        Assert.Equal(HttpStatusCode.Conflict, r.StatusCode);
    }

    [Fact] public async Task Login_with_wrong_password_is_unauthorized() {
        var email = Email(); await LoggedIn(email);
        var r = await f.CreateClient().PostAsJsonAsync("/api/auth/login", new { email, password = "wrong-password" });
        Assert.Equal(HttpStatusCode.Unauthorized, r.StatusCode);
    }

    [Fact] public async Task Expenses_require_login() {
        var r = await f.CreateClient().GetAsync("/api/expenses");
        Assert.Equal(HttpStatusCode.Unauthorized, r.StatusCode);
    }

    [Fact] public async Task User_cannot_read_or_delete_another_users_expense() {
        var a = await LoggedIn(Email()); var b = await LoggedIn(Email());
        var created = await a.PostAsJsonAsync("/api/expenses", new { title = "Lunch", amount = 12.5, category = "Food", date = DateTime.UtcNow });
        var id = (await created.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("id").GetInt32();
        var list = await b.GetFromJsonAsync<JsonElement>("/api/expenses");
        Assert.Equal(0, list.GetArrayLength());
        Assert.Equal(HttpStatusCode.NotFound, (await b.DeleteAsync($"/api/expenses/{id}")).StatusCode);
    }

    [Fact] public async Task Normal_user_cannot_use_admin_endpoints() {
        var c = await LoggedIn(Email());
        Assert.Equal(HttpStatusCode.Forbidden, (await c.GetAsync("/api/admin/stats")).StatusCode);
    }
}
