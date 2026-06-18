# EventBookingAPI

EventBookingAPI is an ASP.NET Core Web API for managing events and bookings. The project includes JWT authentication, role-based access for admins, SQL Server persistence with Entity Framework Core, and a small built-in frontend for testing the main user flows.

## Features

- User registration and login with JWT tokens
- User and Admin roles
- Admin event management
- Event listing with pagination
- Booking creation for published events
- Booking cancellation
- Swagger UI in development
- Built-in frontend in `wwwroot`

## Tech Stack

- ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- JWT Bearer Authentication
- FluentValidation
- AutoMapper
- Vanilla HTML, CSS and JavaScript frontend

## Getting Started

### 1. Clone the repository

```bash
git clone <your-repository-url>
cd EventBookingAPI
```

### 2. Configure the app

Copy the example settings file:

```bash
copy appsettings.Example.json appsettings.Development.json
```

Then update `appsettings.Development.json` with your SQL Server connection string and JWT key.

### 3. Restore dependencies

```bash
dotnet restore
```

### 4. Apply database migrations

```bash
dotnet ef database update
```

### 5. Run the project

```bash
dotnet run
```

Default local URL:

```text
http://localhost:5285
```

Swagger is available in development:

```text
http://localhost:5285/swagger
```

The demo frontend is available at:

```text
http://localhost:5285
```

## API Overview

### Auth

| Method | Endpoint | Description |
| --- | --- | --- |
| POST | `/api/Auth/register` | Register a new user |
| POST | `/api/Auth/login` | Login and receive JWT token |

### Events

| Method | Endpoint | Description |
| --- | --- | --- |
| GET | `/api/Event?pageNumber=1&pageSize=10` | Get paged events |
| GET | `/api/Event/{id}` | Get event by id |
| POST | `/api/Event` | Create event, Admin only |
| PUT | `/api/Event/{id}` | Update event, Admin only |
| PATCH | `/api/Event/{id}/publish` | Publish event, Admin only |
| PATCH | `/api/Event/{id}/cancel` | Cancel event, Admin only |
| DELETE | `/api/Event/{id}` | Delete event, Admin only |

### Bookings

| Method | Endpoint | Description |
| --- | --- | --- |
| POST | `/api/Booking` | Create booking |
| GET | `/api/Booking?page=1&pageSize=10` | Get current user's bookings |
| GET | `/api/Booking/{id}` | Get booking by id |
| PATCH | `/api/Booking/{id}/cancel` | Cancel booking |

## Demo Flow

1. Open `http://localhost:5285`.
2. Register an Admin account.
3. Create a published event.
4. Register or login as a User.
5. Book seats for the event.
6. Cancel a booking from the Bookings panel.

## Notes

- `bin/` and `obj/` are ignored and should not be committed.
- `appsettings.Development.json` is ignored because it can contain local connection strings and secrets.
- Use `appsettings.Example.json` as a safe configuration template.
