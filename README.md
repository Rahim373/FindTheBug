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
- **Module-Based Authorization** - Granular CRUD permissions (View, Create, Edit, Delete) per module
- **Permission-Aware UI** - Sidebar menu and navigation automatically adapt to user's module permissions
- **Permission Service** - Frontend service for checking module permissions in components and templates
- **Permission Directive** - Structural directive for conditional UI elements based on permissions
- **Multi-Role Users** - Assign multiple roles to users with aggregated permissions
- **Grouped Navigation** - User Management menu groups Users and Roles together
- **System Modules** - Dashboard, Doctors, Patients, Laboratory, Dispensary, Billing, User Management
- **Role Management UI** - Table-based interface for configuring module permissions with View, Create, Edit, Delete columns
- **System Role Protection** - Built-in roles (Admin, User, SuperUser) cannot be deleted or modified
- **Backend Authorization** - All API endpoints protected with module-level authorization attributes
- **Policy-Based Security** - ASP.NET Core authorization policies enforce permissions at the API level

### Authentication & Security

- **JWT Authentication** - Secure token-based authentication with 15-minute access tokens
- **Module-Based Authorization** - Granular permission checks on all API endpoints per module
- **Policy-Based Authorization** - ASP.NET Core authorization policies for each module permission
- **Permission Aggregation** - Users can have multiple roles with combined permissions
- **Frontend Permission Service** - Reactive permission checking for UI elements and navigation
- **Permission-Aware Navigation** - Sidebar menu items automatically hide based on user permissions
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
- **Module** - System modules (Dashboard, Doctors, Patients, Laboratory, Dispensary, Billing, UserManagement)
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

# Run API
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

- `GET /api/users` - Get all users with pagination and search (requires UserManagement.View permission)
- `GET /api/users/{id}` - Get user by ID (requires UserManagement.View permission)
- `POST /api/users` - Create new user with role assignment (requires UserManagement.Create permission)
- `PUT /api/users/{id}` - Update user and roles (requires UserManagement.Edit permission)
- `DELETE /api/users/{id}` - Delete user (requires UserManagement.Delete permission)

### Roles (RBAC) 🆕

- `GET /api/roles` - Get all roles with pagination and search (requires UserManagement.View permission)
- `GET /api/roles/active` - Get active roles (for dropdowns) (requires UserManagement.View permission)
- `GET /api/roles/{id}` - Get role by ID (requires UserManagement.View permission)
- `POST /api/roles` - Create new role (requires UserManagement.Create permission)
- `PUT /api/roles/{id}` - Update role (system roles protected) (requires UserManagement.Edit permission)
- `DELETE /api/roles/{id}` - Delete role (system roles protected) (requires UserManagement.Delete permission)

### Doctors 🆕

- `GET /api/doctors` - Get all doctors with pagination (requires Doctors.View permission)
- `GET /api/doctors/{id}` - Get doctor by ID (requires Doctors.View permission)
- `POST /api/doctors` - Create new doctor (requires Doctors.Create permission)
- `PUT /api/doctors/{id}` - Update doctor (requires Doctors.Edit permission)
- `DELETE /api/doctors/{id}` - Delete doctor (requires Doctors.Delete permission)

### Patients 🆕

- `GET /api/patients` - Get all patients (with optional search) (requires Patients.View permission)
- `GET /api/patients/{id}` - Get patient by ID (requires Patients.View permission)
- `POST /api/patients` - Create new patient (requires Patients.Create permission)
- `PUT /api/patients/{id}` - Update patient (requires Patients.Edit permission)
- `DELETE /api/patients/{id}` - Delete patient (requires Patients.Delete permission)

### Drugs 🆕

- `GET /api/drugs` - Get all drugs (requires Dispensary.View permission)
- `GET /api/drugs/{id}` - Get drug by ID (requires Dispensary.View permission)
- `POST /api/drugs` - Create new drug (requires Dispensary.Create permission)
- `PUT /api/drugs/{id}` - Update drug (requires Dispensary.Edit permission)
- `DELETE /api/drugs/{id}` - Delete drug (requires Dispensary.Delete permission)

### Products 🆕

- `GET /api/products` - Get all products (requires Dispensary.View permission)
- `GET /api/products/{id}` - Get product by ID (requires Dispensary.View permission)
- `POST /api/products` - Create new product (requires Dispensary.Create permission)
- `PUT /api/products/{id}` - Update product (requires Dispensary.Edit permission)
- `DELETE /api/products/{id}` - Delete product (requires Dispensary.Delete permission)

### Diagnostic Tests

- `POST /api/diagnostictests` - Create new diagnostic test (requires Laboratory.Create permission)

### Test Parameters

- `GET /api/testparameters` - Get all test parameters (optional filter by diagnostic test ID) (requires Laboratory.View permission)
- `POST /api/testparameters` - Create new test parameter (requires Laboratory.Create permission)
- `PUT /api/testparameters/{id}` - Update test parameter (requires Laboratory.Edit permission)
- `DELETE /api/testparameters/{id}` - Delete test parameter (requires Laboratory.Delete permission)

### Test Entries

- `POST /api/testentries` - Create new test entry (requires Laboratory.Create permission)

### Test Results

- `GET /api/testresults/entry/{entryId}` - Get test results for a test entry (requires Laboratory.View permission)
- `POST /api/testresults` - Create test result (requires Laboratory.Create permission)
- `PUT /api/testresults/{id}` - Update test result (requires Laboratory.Edit permission)
- `POST /api/testresults/{testEntryId}/verify` - Verify test results for a test entry (requires Laboratory.Edit permission)

### Invoices

- `POST /api/invoices` - Create new invoice (requires Billing.Create permission)
- `GET /api/invoices/{id}/pdf` - Generate PDF for an invoice (requires Billing.View permission)

### Modules

- `GET /api/modules` - Get all modules (requires UserManagement.View permission)

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
- **Permission-Aware Navigation** - Sidebar menu automatically hides/shows based on module permissions
- **Permission Service** - Check module permissions in components and templates
- **Permission Directive** - Structural directive for conditional UI elements
- **Multi-Select Dropdowns** - Assign multiple roles to users
- **Role Permission Grid** - Table-based interface for configuring module permissions
- **Responsive Design** - Mobile-friendly interface
- **Dark Theme** - Ng-Zorro dark theme support

### CSS Architecture 🎨

- **No Inline Styles** - All styles separated into dedicated CSS files
- **Global Utility Classes** - Reusable utility classes in `styles.css` for common patterns
  - `.cursor-pointer` - Pointer cursor for interactive elements
  - `.margin-left-8`, `.margin-left-12` - Margin utilities
  - `.width-100` - Full width utility
  - `.text-secondary`, `.font-size-12`, `.font-size-14` - Typography utilities
  - `.form-container`, `.form-container h2` - Standard form styling
  - `.page-header`, `.page-container` - Page layout utilities
  - `.search-bar` - Search input styling
- **Component-Specific CSS** - Each component has its own `.component.css` file for isolated styles
- **Clean Architecture** - Proper separation between HTML templates, CSS stylesheets, and TypeScript logic
- **Maintainable Codebase** - Easier to update, debug, and scale styling
- **Better IDE Support** - Full CSS syntax highlighting and IntelliSense
- **Consistent Patterns** - Standardized styling approach across all components

## 📚 Documentation

Additional documentation is available in `/docs` folder:

- [Module-Based Authorization Guide](docs/Module-Based-Authorization-Guide.md) - Complete guide to permission system
- [Application Layer Structure](docs/Application-Layer-Structure.md)
- [ErrorOr + Result Integration](docs/ErrorOr-Result-Integration.md)
- [Generic Commands & Queries](docs/Generic-Commands-Queries.md)
- [JWT Authentication Guide](docs/JWT-Authentication-Guide.md)
- [Architecture Testing Guide](docs/Architecture-Testing-Guide.md)
- [Docker Registry Documentation](docs/docker-registry.md)
- [Angular App Docker Registry Documentation](docs/docker-registry-app.md)

## 🔐 Security Features

- **JWT Authentication** - Secure token-based authentication
- **Module-Based Authorization** - Granular permission system with View, Create, Edit, Delete per module
- **Policy-Based Authorization** - ASP.NET Core authorization policies enforce permissions at API level
- **Permission-Aware UI** - Frontend automatically adapts to user's permissions
- **Permission Aggregation** - Users can have multiple roles with combined permissions
- **Refresh Token Rotation** - Single-use refresh tokens
- **Account Lockout** - Brute-force protection
- **BCrypt Password Hashing** - Work factor 12
- **Password Reset Security** - One-time tokens with 1-hour expiration
- **RBAC** - Granular permission system
- **Tenant Isolation** - Automatic data segregation
- **Audit Trail** - IP address tracking
- **Correlation IDs** - Request tracing

## 🤝 Contributing

1. Fork repository
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