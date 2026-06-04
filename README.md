# VulnTrack — Vulnerability Management & Remediation Tracking

A .NET 8 Web API for ingesting, scoring, and tracking security vulnerabilities through remediation.

## Stack
- ASP.NET Core 8 Web API
- Entity Framework Core + Pomelo (MySQL)
- JWT bearer authentication with role-based access control (Analyst / Admin / Auditor)
- BCrypt password hashing
- Swagger / OpenAPI

## Prerequisites
- .NET 8 SDK — https://dotnet.microsoft.com/download
- MySQL running locally (you have MySQL Workbench)

## Setup

1. In MySQL Workbench, create the database:
   ```sql
   CREATE DATABASE vulntrack;
   ```

2. Edit `appsettings.json` — set your MySQL password and a long random JWT key:
   - `ConnectionStrings:Default` → your MySQL user/password
   - `Jwt:Key` → any random string 32+ characters

3. Restore packages and run:
   ```bash
   dotnet restore
   dotnet run
   ```

4. Open the Swagger UI (the URL prints in the console, e.g. `https://localhost:5001/swagger`).

## Demo flow (for the interview)

1. **Register** an Analyst — `POST /api/auth/register`
   ```json
   { "username": "analyst1", "password": "password123", "role": "Analyst" }
   ```
2. **Login** — `POST /api/auth/login` → copy the returned `token`.
3. Click **Authorize** in Swagger, paste the token.
4. **Create a vulnerability** — `POST /api/vulnerabilities`
   ```json
   { "title": "SQL injection in login form", "description": "Unsanitized input", "cvssScore": 9.1, "assignedTo": "team-appsec" }
   ```
   Note it auto-assigns severity "Critical" and a 7-day SLA.
5. **List** — `GET /api/vulnerabilities` → sorted by score, highest first.
6. Try **DELETE** as an Analyst → 403 Forbidden (only Admins can delete). This demonstrates RBAC.

## What to highlight when presenting
- Separation of concerns: controllers stay thin, logic lives in services (`CvssService`).
- Security: passwords hashed with BCrypt, never stored plaintext; DTOs prevent over-posting; `[Authorize(Roles=...)]` enforces least privilege per endpoint.
- CVSS v3.1 severity bands map score → severity → SLA deadline automatically.
