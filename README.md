# FindTheBug - Diagnostics Lab Management System

A modern, multi-tenant diagnostics laboratory management system built with ASP.NET Core following Clean Architecture principles.

## 🏗️ Architecture

This project implements **Clean Architecture** with clear separation of concerns:

- **Domain Layer** (`FindTheBug.Domain`) - Core business entities and interfaces
- **Application Layer** (`FindTheBug.Application`) - Business logic, CQRS handlers, DTOs
- **Infrastructure Layer** (`FindTheBug.Infrastructure`) - Data access, external services
- **Presentation Layer** (`FindTheBug.WebAPI`) - REST API endpoints, middleware

## ✨ Key Features

### Authentication & Security
- **JWT Authentication** - Secure token-based authentication with 15-minute access tokens
- **Refresh Token Mechanism** - Long-lived refresh tokens (7 days) with automatic rotation
- **Password Reset** - Secure email-based password reset with one-time tokens
- **Password Change** - Authenticated users can change passwords with automatic token revocation
- **Account Lockout** - Automatic lockout after 5 failed login attempts (15-minute duration)
- **BCrypt Password Hashing** - Industry-standard password hashing with work factor 12
- **Token Revocation** - Logout functionality with refresh token revocation
- **Security Audit Trail** - IP address tracking for logins and token operations

### Multi-Tenancy
- **Subdomain-based tenant resolution** - Automatic tenant detection from request subdomain
- **Tenant-isolated databases** - Each tenant has its own PostgreSQL database
- **Dynamic connection strings** - Runtime database selection based on tenant context
- **Tenant-scoped data access** - Automatic filtering of all queries by tenant ID

### CQRS with MediatR
- **Command/Query Separation** - Clear distinction between reads and writes
- **Generic base interfaces** - `ICommand<TResponse>` and `IQuery<TResponse>` reduce boilerplate
- **Handler pattern** - `ICommandHandler` and `IQueryHandler` for consistent implementation
- **Decoupled architecture** - Controllers delegate to MediatR handlers

### Error Handling
- **ErrorOr pattern** - Functional error handling without exceptions
- **Automatic response conversion** - `ErrorOrActionFilter` converts errors to Problem Details (RFC 7807)
- **Consistent API responses** - `ResultWrapperMiddleware` wraps all responses in `Result<T>` format
- **Typed errors** - NotFound, Validation, Conflict, Unauthorized, etc.

### Monitoring & Observability
- **Prometheus metrics** - HTTP request metrics, custom business metrics
- **Health checks** - Database connectivity, application health
- **Structured logging** - Serilog with console sink and correlation IDs
- **Request/Response logging** - Automatic logging of all HTTP requests

### Modern C# Features
- **File-scoped namespaces** - Cleaner code organization
- **Primary constructors** - Reduced boilerplate in classes
- **Record types** - Immutable DTOs and commands/queries
- **Pattern matching** - Modern null checking with `is null`/`is not null`
- **Collection expressions** - Simplified collection initialization

## 🏥 Domain Model

### Core Entities

- **User** - User accounts with authentication and roles
- **Patient** - Patient demographics and contact information
- **DiagnosticTest** - Test catalog with pricing and descriptions
- **TestParameter** - Individual parameters/fields for each test
- **TestEntry** - Patient test registration and sample tracking
- **TestResult** - Test results with verification workflow
- **Invoice** - Billing and payment tracking
- **InvoiceItem** - Line items for invoices
- **RefreshToken** - JWT refresh tokens with expiration tracking
- **PasswordResetToken** - Secure password reset tokens

## 🚀 Getting Started

### Prerequisites

- .NET 8.0 SDK or later
- PostgreSQL 14+
- (Optional) Docker for containerized deployment

### Configuration

Update `appsettings.json` with your PostgreSQL connection string:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=FindTheBug;Username=postgres;Password=yourpassword"
  }
}
```

### Running the Application

```bash
# Restore dependencies
dotnet restore

# Run database migrations
dotnet ef database update --project src/FindTheBug.Infrastructure --startup-project src/FindTheBug.WebAPI

# Run the application
dotnet run --project src/FindTheBug.WebAPI
```

The API will be available at `https://localhost:7001` (or configured port).

### API Documentation

Swagger/OpenAPI documentation is available at:
- `https://localhost:7001/swagger`

## 📡 API Endpoints

### Patients
- `GET /api/patients` - Get all patients (with optional search)
- `GET /api/patients/{id}` - Get patient by ID
- `POST /api/patients` - Create new patient
- `PUT /api/patients/{id}` - Update patient
- `DELETE /api/patients/{id}` - Delete patient

### Diagnostic Tests
- `GET /api/diagnostictests` - Get all tests
- `GET /api/diagnostictests/{id}` - Get test by ID
- `POST /api/diagnostictests` - Create new test
- `PUT /api/diagnostictests/{id}` - Update test
- `DELETE /api/diagnostictests/{id}` - Delete test

### Test Parameters
- `GET /api/testparameters?diagnosticTestId={id}` - Get parameters for a test
- `POST /api/testparameters` - Create new parameter
- `PUT /api/testparameters/{id}` - Update parameter
- `DELETE /api/testparameters/{id}` - Delete parameter

### Test Entries
- `GET /api/testentries` - Get all test entries
- `GET /api/testentries/{id}` - Get entry by ID
- `POST /api/testentries` - Register patient for test
- `PUT /api/testentries/{id}/status` - Update entry status

### Test Results
- `GET /api/testresults/entry/{testEntryId}` - Get results for a test entry
- `POST /api/testresults` - Record test result
- `PUT /api/testresults/{id}` - Update test result
- `POST /api/testresults/{testEntryId}/verify` - Verify test results

### Invoices
- `GET /api/invoices` - Get all invoices
- `GET /api/invoices/{id}` - Get invoice by ID
- `POST /api/invoices` - Create new invoice
- `PUT /api/invoices/{id}/status` - Update invoice status

### Authentication
- `POST /api/token` - Login with email and password
- `POST /api/token/refresh` - Refresh access token
- `POST /api/token/change-password` - Change password (requires auth)
- `POST /api/token/request-reset` - Request password reset email
- `POST /api/token/reset-password` - Reset password with token
- `POST /api/token/revoke` - Revoke refresh token (logout)

### Monitoring
- `GET /metrics` - Prometheus metrics endpoint
- `GET /health` - Health check endpoint

## 🔧 Technology Stack

### Backend
- **ASP.NET Core 8.0** - Web framework
- **Entity Framework Core** - ORM
- **PostgreSQL** - Database
- **MediatR** - CQRS implementation
- **ErrorOr** - Functional error handling
- **Serilog** - Structured logging
- **Prometheus** - Metrics and monitoring

### Packages
- `ErrorOr` - Functional error handling
- `MediatR` - Mediator pattern for CQRS
- `Npgsql.EntityFrameworkCore.PostgreSQL` - PostgreSQL provider
- `Serilog.AspNetCore` - Logging
- `prometheus-net.AspNetCore` - Metrics
- `Swashbuckle.AspNetCore` - OpenAPI/Swagger
- `BCrypt.Net-Next` - Password hashing
- `Microsoft.AspNetCore.Authentication.JwtBearer` - JWT authentication
- `System.IdentityModel.Tokens.Jwt` - JWT token handling

## 📚 Documentation

Additional documentation is available in the `/docs` folder:

- [ErrorOr + Result Integration](docs/ErrorOr-Result-Integration.md) - Error handling flow
- [Generic Commands & Queries](docs/Generic-Commands-Queries.md) - CQRS pattern guide

## 🏛️ Design Patterns

- **Clean Architecture** - Dependency inversion, separation of concerns
- **CQRS** - Command Query Responsibility Segregation
- **Repository Pattern** - Data access abstraction
- **Unit of Work** - Transaction management
- **Mediator Pattern** - Decoupled request handling
- **Functional Error Handling** - ErrorOr pattern instead of exceptions

## 🔐 Security Features

- **Tenant isolation** - Automatic data segregation
- **Input validation** - Request validation at API layer
- **Error sanitization** - No sensitive data in error responses
- **Correlation IDs** - Request tracing for security auditing

## 📊 Response Format

All API responses follow a consistent format:

### Success Response
```json
{
  "isSuccess": true,
  "data": { /* entity data */ },
  "errorMessage": null,
  "errors": []
}
```

### Error Response
```json
{
  "isSuccess": false,
  "data": null,
  "errorMessage": "Patient with ID xxx not found",
  "errors": [
    {
      "code": "Patient.NotFound",
      "description": "Patient with ID xxx not found"
    }
  ]
}
```

## 🧪 Testing

```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test /p:CollectCoverage=true
```

## 📈 Monitoring

### Prometheus Metrics

The application exposes metrics at `/metrics`:

- HTTP request duration
- HTTP request count by status code
- Active requests
- Custom business metrics (patients created, tests performed, etc.)

### Health Checks

Health check endpoint at `/health` returns:
- Database connectivity status
- Application health status

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📝 License

This project is licensed under the MIT License.

## 👥 Authors

- Your Name - Initial work

## 🙏 Acknowledgments

- Clean Architecture by Robert C. Martin
- ErrorOr library by Amichai Mantinband
- MediatR by Jimmy Bogard
