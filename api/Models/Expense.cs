namespace SecureSpend.Api.Models;
public class Expense {
    public int Id { get; set; } public int UserId { get; set; }
    public string Title { get; set; } = ""; public decimal Amount { get; set; }
    public string Category { get; set; } = ""; public DateTime Date { get; set; }
}
