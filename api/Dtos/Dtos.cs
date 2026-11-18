namespace SecureSpend.Api.Dtos;
public record AuthDto(string Email, string Password);
public record ExpenseDto(string Title, decimal Amount, string Category, DateTime Date);
