E-Commerce Backend API
A secure and robust e-commerce backend API built with ASP.NET Core 9 Web API, featuring user and product management with JWT authentication and token refresh functionality.

🚀 Features
User Management - Registration, login, profile management

Product Management - Full CRUD operations for products

JWT Authentication - Secure token-based authentication

Token Refresh - Automatic token renewal mechanism

Role-based Authorization - Admin and customer roles

Entity Framework Core - Data access with SQL Server

Image Upload - Local storage for product images

Comprehensive Testing - Unit and integration tests

Swagger Documentation - Interactive API documentation

🛠️ Tech Stack
Framework: ASP.NET Core 9.0

Database: SQL Server with Entity Framework Core

Authentication: JWT Bearer Tokens

Testing: xUnit, Moq, Microsoft.AspNetCore.Mvc.Testing

Validation: FluentValidation

Mapping: AutoMapper

📋 Prerequisites
.NET 9.0 SDK

SQL Server

Visual Studio 2022 or VS Code

📦 Installation & Setup
1. Clone the Repository
bash
git clone <repository-url>
cd Ecommerce.API
2. Database Setup
Option A: Using Migrations (Recommended)
bash
# Navigate to the WebAPI project
cd Ecommerce.WebAPI

# Run database migrations
dotnet ef database update
Option B: Using SQL Script
Open SQL Server Management Studio

Run the provided DatabaseScript.sql file

Update connection string in appsettings.json

3. Configure Application Settings
Update appsettings.json with your database connection string:

json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=EcommerceDB;Trusted_Connection=true;TrustServerCertificate=true;"
  },
  "Jwt": {
    "Secret": "your-super-secret-key-here",
    "Issuer": "Ecommerce.API",
    "Audience": "Ecommerce.Client",
    "AccessTokenExpiration": 15, // minutes
    "RefreshTokenExpiration": 7 // days
  }
}
4. Run the Application
bash
# Restore packages
dotnet restore

# Run the application
dotnet run
