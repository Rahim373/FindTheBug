# FindTheBug - Diagnostics Lab Management System

A modern, multi-tenant ASP.NET Core Web API for diagnostics lab management, built with Clean Architecture, CQRS pattern, and comprehensive monitoring.

## 🏗️ Architecture

This project follows **Clean Architecture** with **CQRS pattern** using MediatR, designed for scalability, maintainability, and testability.

```
FindTheBug/
├── src/
│   ├── FindTheBug.Domain/          # Core business entities and logic
│   ├── FindTheBug.Application/     # Commands, Queries, and Handlers (CQRS)
│   ├── FindTheBug.Infrastructure/  # Data access, repositories, and services
│   └── FindTheBug.WebAPI/          # API controllers and configuration
└── FindTheBug.sln
```

### Layer Dependencies

```
WebAPI → Infrastructure → Application → Domain
         ↓
    IMediator → Handlers → UnitOfWork → Repository → DbContext
```

- **Domain**: Enterprise business rules, entities, and domain exceptions (no dependencies)
- **Application**: Commands, queries, handlers, and interfaces (depends on Domain)
- **Infrastructure**: Data access, Unit of Work, repositories, and external services (depends on Application)
- **WebAPI**: API controllers using MediatR (depends on Application & Infrastructure)

## 🚀 Features

### Core Architecture
- ✅ **Clean Architecture** - Clear separation of concerns with dependency inversion
- ✅ **CQRS Pattern** - Commands and Queries with MediatR
- ✅ **Unit of Work Pattern** - Transaction management and repository coordination
- ✅ **Repository Pattern** - Generic repository for data access abstraction
- ✅ **Modern C# 12** - File-scoped namespaces, primary constructors, pattern matching

### Multi-Tenancy
- ✅ **Subdomain-based Tenant Resolution** - Automatic tenant detection from subdomain
- ✅ **Tenant Isolation** - Separate in-memory databases per tenant
- ✅ **Global Query Filters** - Automatic tenant data filtering
- ✅ **Tenant Management API** - CRUD operations for tenant configuration

### Diagnostics Lab Management
- ✅ **Diagnostic Tests** - Test catalog with pricing, categories, and parameters
- ✅ **Test Parameters** - Configurable test inputs with reference ranges
- ✅ **Patient Management** - Patient records with required mobile number
- ✅ **Test Entries** - Patient test registration with status workflow
- ✅ **Test Results** - Results storage with verification workflow
- ✅ **Invoicing** - Billing with line items, discounts, and payment tracking

### Monitoring & Observability
- ✅ **Prometheus Metrics** - HTTP metrics, custom business metrics, tenant-specific counters
- ✅ **Health Checks** - Application and database health monitoring
- ✅ **Metrics Endpoint** - `/metrics` for Prometheus scraping
- ✅ **Health Endpoint** - `/health` with detailed status

### API & Documentation
- ✅ **RESTful API** - Following REST principles with proper HTTP verbs
- ✅ **Swagger/OpenAPI** - Interactive API documentation with XML comments
- ✅ **Result Pattern** - Consistent response handling with success/failure states
- ✅ **Audit Tracking** - Automatic CreatedAt/UpdatedAt timestamps

## 📋 Prerequisites

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download) or later
- IDE: Visual Studio 2022, VS Code, or Rider
- (Optional) Docker for Prometheus monitoring

## 🛠️ Getting Started

### 1. Clone the Repository

```bash
git clone <repository-url>
cd FindTheBug
```

### 2. Restore Dependencies

```bash
dotnet restore
```

### 3. Build the Solution

```bash
dotnet build
```

### 4. Run the Application

```bash
dotnet run --project src/FindTheBug.WebAPI/FindTheBug.WebAPI.csproj
```

The API will be available at:
- HTTPS: `https://localhost:5001`
- HTTP: `http://localhost:5000`
- Swagger UI: `https://localhost:5001/swagger`
- Metrics: `https://localhost:5001/metrics`
- Health: `https://localhost:5001/health`

## 📚 API Endpoints

### Tenant Management

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/tenants` | Get all tenants |
| GET | `/api/tenants/{id}` | Get tenant by ID |
| GET | `/api/tenants/subdomain/{subdomain}` | Get tenant by subdomain |
| POST | `/api/tenants` | Create new tenant |
| PUT | `/api/tenants/{id}` | Update tenant |

### Patient Management

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/patients?search={query}` | Search patients by name or mobile |
| GET | `/api/patients/{id}` | Get patient by ID |
| POST | `/api/patients` | Register new patient (mobile required) |

### Diagnostic Tests

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/diagnostictests?category={category}` | Get tests by category |
| GET | `/api/diagnostictests/{id}` | Get test details |
| POST | `/api/diagnostictests` | Create new test |

### Test Entries

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/testentries?status={status}` | Get entries by status |
| GET | `/api/testentries/{id}` | Get entry details |
| POST | `/api/testentries` | Register patient for test |

### Test Results

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/testresults/entry/{testEntryId}` | Get results for test entry |
| POST | `/api/testresults` | Record test results |
| POST | `/api/testresults/{testEntryId}/verify` | Verify results |

### Invoices

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/invoices?status={status}` | Get invoices by status |
| GET | `/api/invoices/{id}` | Get invoice details |
| POST | `/api/invoices` | Create invoice from test entries |

### Monitoring

| Endpoint | Description |
|----------|-------------|
| `/metrics` | Prometheus metrics endpoint |
| `/health` | Health check endpoint |
| `/api/metrics/summary` | Metrics summary |

## 🔧 Multi-Tenancy Usage

### Creating a Tenant

```bash
curl -X POST https://localhost:5001/api/tenants \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Lab One",
    "subdomain": "lab1",
    "connectionString": "InMemory_Lab1",
    "isActive": true
  }'
```

### Accessing Tenant Data

Access the API using the tenant's subdomain:
- `https://lab1.localhost:5001/api/patients`
- `https://lab2.localhost:5001/api/diagnostictests`

Data is automatically isolated per tenant using global query filters.

## 📊 Prometheus Monitoring

### Available Metrics

**HTTP Metrics (Automatic):**
- `http_requests_received_total` - Total HTTP requests
- `http_request_duration_seconds` - Request duration histogram

**Custom Business Metrics:**
- `findthebug_tenant_requests_total{tenant_id}` - Requests per tenant
- `findthebug_active_tenants` - Number of active tenants
- `findthebug_entity_operations_total{entity_type,operation}` - CRUD operations
- `findthebug_operation_duration_seconds{operation}` - Operation performance

### Local Prometheus Setup

See `prometheus.yml.example` for configuration.

```bash
docker run -d \
  --name prometheus \
  -p 9090:9090 \
  -v ${PWD}/prometheus.yml:/etc/prometheus/prometheus.yml \
  prom/prometheus
```

## 🗂️ Project Structure

### Domain Layer

```
Domain/
├── Common/
│   ├── BaseEntity.cs
│   ├── BaseAuditableEntity.cs
│   └── ITenantEntity.cs
├── Entities/
│   ├── DiagnosticTest.cs
│   ├── TestParameter.cs
│   ├── Patient.cs
│   ├── TestEntry.cs
│   ├── TestResult.cs
│   ├── Invoice.cs
│   └── InvoiceItem.cs
└── Exceptions/
    └── DomainException.cs
```

### Application Layer (CQRS)

```
Application/
├── Common/
│   ├── Interfaces/
│   │   ├── IRepository.cs
│   │   ├── IUnitOfWork.cs
│   │   └── IApplicationDbContext.cs
│   └── Models/
│       └── Result.cs
├── Features/
│   ├── Patients/
│   │   ├── Commands/
│   │   │   └── CreatePatientCommand.cs
│   │   └── Queries/
│   │       ├── GetAllPatientsQuery.cs
│   │       └── GetPatientByIdQuery.cs
│   ├── DiagnosticTests/
│   ├── TestEntries/
│   └── Invoices/
└── DependencyInjection.cs
```

### Infrastructure Layer

```
Infrastructure/
├── Data/
│   ├── ApplicationDbContext.cs
│   └── MasterDbContext.cs
├── MultiTenancy/
│   ├── TenantContext.cs
│   ├── TenantService.cs
│   ├── TenantResolutionMiddleware.cs
│   └── TenantDbContextFactory.cs
├── Persistence/
│   └── UnitOfWork.cs
├── Repositories/
│   └── Repository.cs
├── Monitoring/
│   └── MetricsService.cs
└── DependencyInjection.cs
```

## 📦 NuGet Packages

### Application
- `MediatR` - CQRS pattern implementation

### Infrastructure
- `Microsoft.EntityFrameworkCore.InMemory` - In-memory database provider
- `prometheus-net` - Metrics collection
- `Microsoft.Extensions.Diagnostics.HealthChecks` - Health checks

### WebAPI
- `Swashbuckle.AspNetCore` - Swagger/OpenAPI
- `prometheus-net.AspNetCore` - HTTP metrics middleware
- `AspNetCore.HealthChecks.UI.Client` - Health check UI formatting

## 🎯 Design Patterns

- **Clean Architecture** - Dependency inversion and separation of concerns
- **CQRS** - Command Query Responsibility Segregation with MediatR
- **Repository Pattern** - Data access abstraction
- **Unit of Work** - Transaction management
- **Result Pattern** - Consistent response handling
- **Factory Pattern** - Tenant database context creation
- **Middleware Pattern** - Tenant resolution

## 🧪 Example Workflow

### 1. Create a Tenant
```bash
POST /api/tenants
{
  "name": "City Lab",
  "subdomain": "citylab"
}
```

### 2. Register a Patient
```bash
POST /api/patients
{
  "firstName": "John",
  "lastName": "Doe",
  "mobileNumber": "1234567890"
}
```

### 3. Create a Diagnostic Test
```bash
POST /api/diagnostictests
{
  "testCode": "CBC",
  "testName": "Complete Blood Count",
  "category": "Hematology",
  "price": 500
}
```

### 4. Register Patient for Test
```bash
POST /api/testentries
{
  "patientId": "guid",
  "diagnosticTestId": "guid",
  "priority": "Normal"
}
```

### 5. Record Results
```bash
POST /api/testresults
{
  "testEntryId": "guid",
  "testParameterId": "guid",
  "resultValue": "14.5"
}
```

### 6. Create Invoice
```bash
POST /api/invoices
{
  "patientId": "guid",
  "items": [...]
}
```

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📝 License

This project is licensed under the MIT License.

## 🙏 Acknowledgments

- Clean Architecture principles by Robert C. Martin
- CQRS pattern and MediatR library
- ASP.NET Core team for the excellent framework
- Prometheus for monitoring and observability
