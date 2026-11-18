# SecureSpend

Expense tracker: ASP.NET Core 8 API, React + Tailwind, PostgreSQL, Docker, AWS.

## Run locally
```bash
cp .env.example .env      # then edit the values
docker compose up --build
```
App: http://localhost:3000  ·  API docs (dev only): run the API with ASPNETCORE_ENVIRONMENT=Development

## Structure
```
api/        ASP.NET Core API (Models, Data, Dtos, Endpoints, Services)
api.Tests/  xUnit integration tests (run: dotnet test api.Tests)
web/        React + Tailwind (components, pages)
infra/      Terraform (coming next)
```

## Admin account
Set ADMIN_EMAIL and ADMIN_PASSWORD in .env. The account is created on startup. Admins see totals and user counts only.

## Security decisions (grows with the project)
- Passwords hashed with bcrypt; JWT access tokens expire after 2 hours
- Role-based access: admin endpoints return 403 for normal users; admins never see individual expenses
- Every expense query is scoped to the logged-in user
- Rate limiting on login and register; security headers on API and web
- Containers run as non-root; secrets come from environment, never from the image
