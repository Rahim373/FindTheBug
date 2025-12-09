# FindTheBug - Diagnostics Lab Management System

[![Architecture Tests](https://github.com/rahim373/FindTheBug/actions/workflows/architecture-tests.yml/badge.svg)](https://github.com/rahim373/FindTheBug/actions/workflows/architecture-tests.yml)
[![Docker Build and Publish](https://github.com/rahim373/FindTheBug/actions/workflows/docker-publish.yml/badge.svg)](https://github.com/rahim373/FindTheBug/actions/workflows/docker-publish.yml)
[![Docker Build and Publish Angular App](https://github.com/rahim373/FindTheBug/actions/workflows/docker-publish-app.yml/badge.svg)](https://github.com/rahim373/FindTheBug/actions/workflows/docker-publish-app.yml)

A modern, multi-tenant diagnostics laboratory management system built with ASP.NET Core and Angular, following Clean Architecture principles.

## 🏗️ Architecture

This project implements **Clean Architecture** with clear separation of concerns:

- **Domain Layer** (`FindTheBug.Domain`) - Core business entities and interfaces
- **Application Layer** (`FindTheBug.Application`) - Business logic, CQRS handlers, DTOs
- **Infrastructure Layer** (`FindTheBug.Infrastructure`) - Data access, external services
- **Presentation Layer** (`FindTheBug.WebAPI`) - REST API endpoints, middleware
- **Frontend** (`FindTheBug.App`) - Angular SPA with Ng-Zorro UI

## ✨ Key Features

### Role-Based Access Control (RBAC) 🆕

- **Custom Roles** - Create and manage custom roles with specific permissions
- **Module-Based Permissions** - Granular CRUD permissions (View, Create, Edit, Delete) per module
- **Multi-Role Users** - Assign multiple roles to users via multi-select dropdown
- **Grouped Navigation** - User Management menu groups Users and Roles together
- **System Modules** - Dashboard, Users, Roles, Modules pre-configured
- **Role Management UI** - Full CRUD interface for managing roles
- **System Role Protection** - Built-in roles (Admin, User, SuperUser) cannot be deleted or modified

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
- **Typed errors** - NotFound, Validation, Conflict, Unauthorized, Forbidden, etc.

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

**RBAC Entities:**

- **User** - User accounts with authentication, multi-role assignment, phone, and NID number
- **Role** - Custom roles with system role protection
- **Module** - System modules (Dashboard, Users, Roles, Modules)
- **UserRole** - Many-to-many relationship between users and roles
- **RoleModulePermission** - CRUD permissions for roles on modules

**Business Entities:**

- **Patient** - Patient demographics and contact information
- **DiagnosticTest** - Test catalog with pricing and descriptions
- **TestParameter** - Individual parameters/fields for each test
- **TestEntry** - Patient test registration and sample tracking
- **TestResult** - Test results with verification workflow
- **Invoice** - Billing and payment tracking
- **InvoiceItem** - Line items for invoices

**Security Entities:**

- **RefreshToken** - JWT refresh tokens with expiration tracking
- **PasswordResetToken** - Secure password reset tokens

## 🚀 Getting Started

### Prerequisites

- .NET 10.0 SDK or later
- Node.js 18+ and Yarn
- PostgreSQL 14+
- (Optional) Docker for containerized deployment

### Running with Docker Compose (Recommended)

```bash
# Start all services (API, Angular App, PostgreSQL, Prometheus, Grafana)
docker-compose up -d

# View logs
docker-compose logs -f

# Stop all services
docker-compose down
```

**Services:**

- **Angular App** - http://localhost:4200
- **WebAPI** - http://localhost:9909
- **PostgreSQL** - Port 5432
- **Prometheus** - http://localhost:9090
- **Grafana** - http://localhost:3000 (admin/admin)

### Running Locally

**Backend:**

```bash
# Restore dependencies
dotnet restore

# Run database migrations
dotnet ef database update --project src/FindTheBug.Infrastructure --startup-project src/FindTheBug.WebAPI

# Run the API
dotnet run --project src/FindTheBug.WebAPI
```

**Frontend:**

```bash
cd src/FindTheBug.App

# Install dependencies
yarn install

# Run development server
yarn start
```

The Angular app will be available at `http://localhost:4200` and API at `https://localhost:7001`.

## 📡 API Endpoints

### Users

- `GET /api/users` - Get all users with pagination and search
- `GET /api/users/{id}` - Get user by ID
- `POST /api/users` - Create new user with role assignment
- `PUT /api/users/{id}` - Update user and roles
- `DELETE /api/users/{id}` - Delete user

### Roles (RBAC) 🆕

- `GET /api/roles` - Get all roles with pagination and search
- `GET /api/roles/active` - Get active roles (for dropdowns)
- `GET /api/roles/{id}` - Get role by ID
- `POST /api/roles` - Create new role
- `PUT /api/roles/{id}` - Update role (system roles protected)
- `DELETE /api/roles/{id}` - Delete role (system roles protected)

### Patients

- `GET /api/patients` - Get all patients (with optional search)
- `GET /api/patients/{id}` - Get patient by ID
- `POST /api/patients` - Create new patient
- `PUT /api/patients/{id}` - Update patient
- `DELETE /api/patients/{id}` - Delete patient

### Diagnostic Tests

- `POST /api/diagnostictests` - Create new diagnostic test

### Test Parameters

- `GET /api/testparameters` - Get all test parameters (optional filter by diagnostic test ID)
- `POST /api/testparameters` - Create new test parameter
- `PUT /api/testparameters/{id}` - Update test parameter
- `DELETE /api/testparameters/{id}` - Delete test parameter

### Test Entries

- `POST /api/testentries` - Create new test entry

### Test Results

- `GET /api/testresults/entry/{entryId}` - Get test results for a test entry
- `POST /api/testresults` - Create test result
- `PUT /api/testresults/{id}` - Update test result
- `POST /api/testresults/{testEntryId}/verify` - Verify test results for a test entry

### Invoices

- `POST /api/invoices` - Create new invoice
- `GET /api/invoices/{id}/pdf` - Generate PDF for an invoice

### Modules

- `GET /api/modules` - Get all modules

### Authentication

- `POST /api/token` - Login with email and password
- `POST /api/token/refresh` - Refresh access token
- `POST /api/token/change-password` - Change password
- `POST /api/token/request-reset` - Request password reset email
- `POST /api/token/reset-password` - Reset password with token
- `POST /api/token/revoke` - Revoke refresh token (logout)

### Monitoring

- `GET /metrics` - Prometheus metrics endpoint
- `GET /health` - Health check endpoint

## 🔧 Technology Stack

### Backend

- **ASP.NET Core 10.0** - Web framework
- **Entity Framework Core** - ORM
- **PostgreSQL** - Database
- **MediatR** - CQRS implementation
- **ErrorOr** - Functional error handling
- **Serilog** - Structured logging
- **Prometheus** - Metrics and monitoring

### Frontend

- **Angular 19** - SPA framework
- **Ng-Zorro** - UI component library
- **RxJS** - Reactive programming
- **TypeScript** - Type-safe JavaScript

## 🎨 Frontend Features

- **Standalone Components** - Modern Angular architecture
- **Reactive Forms** - Type-safe form handling
- **Lazy Loading** - Optimized bundle sizes
- **Grouped Navigation** - User Management menu with Users and Roles
- **Multi-Select Dropdowns** - Assign multiple roles to users
- **Responsive Design** - Mobile-friendly interface
- **Dark Theme** - Ng-Zorro dark theme support

## 📚 Documentation

Additional documentation is available in the `/docs` folder:

- [Application Layer Structure](docs/Application-Layer-Structure.md)
- [ErrorOr + Result Integration](docs/ErrorOr-Result-Integration.md)
- [Generic Commands & Queries](docs/Generic-Commands-Queries.md)
- [JWT Authentication Guide](docs/JWT-Authentication-Guide.md)
- [Architecture Testing Guide](docs/Architecture-Testing-Guide.md)
- [Docker Registry Documentation](docs/docker-registry.md)
- [Angular App Docker Registry Documentation](docs/docker-registry-app.md)

## 🔐 Security Features

- **JWT Authentication** - Secure token-based authentication
- **Refresh Token Rotation** - Single-use refresh tokens
- **Account Lockout** - Brute-force protection
- **BCrypt Password Hashing** - Work factor 12
- **Password Reset Security** - One-time tokens with 1-hour expiration
- **RBAC** - Granular permission system
- **Tenant Isolation** - Automatic data segregation
- **Audit Trail** - IP address tracking
- **Correlation IDs** - Request tracing

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📝 License

This project is licensed under the MIT License.

## 👥 Authors

- Abdur Rahim

## 🙏 Acknowledgments

- Clean Architecture by Robert C. Martin
- ErrorOr library by Amichai Mantinband
- MediatR by Jimmy Bogard
- Ng-Zorro UI Library
