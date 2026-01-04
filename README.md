# Offline Ticketing System API

Backend Web API built with **ASP.NET Core 8**, **Entity Framework Core**, and **MySQL**.

---

##  How to Run the Project
1. Install .NET 8 SDK and MySQL.
2. Configure connection string in `appsettings.json`:   
   "ConnectionStrings": {
     "DefaultConnection": "Server=localhost;Port=3306;Database=ticketing_db;User=root;Password=your_password;"
   }

3. Apply migrations:
dotnet ef migrations add InitialCreate
dotnet ef database update

4. Run the project:
dotnet run

in browser find:
Swagger UI → https://localhost:5001/swagger


## Seed Data
On first run, if the database is empty, the DbSeeder creates:

Admin: admin@example.com / Admin123!

Employee: employee@example.com / Employee123!

Use these accounts to log in and obtain JWT tokens.
Include the token in request headers:
Authorization: Bearer <token>


## Assumptions & Decisions
. Roles: Admin and Employee.

. Passwords stored securely with BCrypt.

. Database: MySQL (ticketing_db) via Pomelo EF Core provider.


