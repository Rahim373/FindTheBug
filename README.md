# FindTheBug

A modern ASP.NET Core Web API built with Clean Architecture principles, designed for scalability, maintainability, and testability.

## 🏗️ Architecture

This project follows **Clean Architecture** (also known as Onion Architecture) with clear separation of concerns across four layers:

```
FindTheBug/
├── src/
│   ├── FindTheBug.Domain/          # Core business entities and logic
│   ├── FindTheBug.Application/     # Use cases and business rules
│   ├── FindTheBug.Infrastructure/  # Data access and external services
│   └── FindTheBug.WebAPI/          # API controllers and configuration
└── FindTheBug.sln
```

### Layer Dependencies

```
WebAPI → Infrastructure → Application → Domain
```

- **Domain**: Contains enterprise business rules, entities, and domain exceptions (no dependencies)
- **Application**: Contains application business rules, interfaces, and DTOs (depends on Domain)
- **Infrastructure**: Contains data access implementations, repositories, and DbContext (depends on Application)
- **WebAPI**: Contains API controllers, middleware, and configuration (depends on Application & Infrastructure)

## 🚀 Features

- ✅ **Clean Architecture** - Clear separation of concerns with dependency inversion
- ✅ **RESTful API** - Following REST principles with proper HTTP verbs
- ✅ **Repository Pattern** - Generic repository for data access abstraction
- ✅ **Entity Framework Core** - ORM with in-memory database support
- ✅ **Swagger/OpenAPI** - Interactive API documentation
- ✅ **Audit Tracking** - Automatic CreatedAt/UpdatedAt timestamps
- ✅ **Result Pattern** - Consistent response handling with success/failure states
- 🔄 **PostgreSQL Support** - (Planned) Production-ready database
- 🔄 **Enhanced OpenAPI** - (Planned) XML documentation and versioning

## 📋 Prerequisites

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download) or later
- IDE: Visual Studio 2022, VS Code, or Rider
- (Optional) PostgreSQL for production database

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

## 📚 API Endpoints

### Sample Entity CRUD Operations

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/sample` | Get all sample entities |
| GET | `/api/sample/{id}` | Get a specific entity by ID |
| POST | `/api/sample` | Create a new entity |
| PUT | `/api/sample/{id}` | Update an existing entity |
| DELETE | `/api/sample/{id}` | Delete an entity |

### Example Request

**Create a new entity:**

```bash
curl -X POST https://localhost:5001/api/sample \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Sample Item",
    "description": "This is a sample entity",
    "isActive": true
  }'
```

## 🗂️ Project Structure

### Domain Layer (`FindTheBug.Domain`)

```
Domain/
├── Common/
│   ├── BaseEntity.cs              # Base entity with Id
│   └── BaseAuditableEntity.cs     # Base entity with audit fields
├── Entities/
│   └── SampleEntity.cs            # Sample domain entity
└── Exceptions/
    └── DomainException.cs         # Domain-specific exceptions
```

### Application Layer (`FindTheBug.Application`)

```
Application/
├── Common/
│   ├── Interfaces/
│   │   ├── IRepository.cs         # Generic repository interface
│   │   └── IApplicationDbContext.cs
│   └── Models/
│       └── Result.cs              # Result wrapper for responses
└── DependencyInjection.cs         # Service registration
```

### Infrastructure Layer (`FindTheBug.Infrastructure`)

```
Infrastructure/
├── Data/
│   └── ApplicationDbContext.cs    # EF Core DbContext
├── Repositories/
│   └── Repository.cs              # Generic repository implementation
└── DependencyInjection.cs         # Infrastructure service registration
```

### WebAPI Layer (`FindTheBug.WebAPI`)

```
WebAPI/
├── Controllers/
│   └── SampleController.cs        # Sample API controller
├── Program.cs                     # Application entry point
└── appsettings.json              # Configuration
```

## 🔧 Configuration

### Database Configuration

Currently using **in-memory database** for development. To switch to PostgreSQL (planned):

1. Update `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=findthebug;Username=postgres;Password=yourpassword"
  }
}
```

2. Run migrations:
```bash
dotnet ef database update --project src/FindTheBug.Infrastructure --startup-project src/FindTheBug.WebAPI
```

## 🧪 Testing

```bash
# Run all tests (when test projects are added)
dotnet test
```

## 📦 NuGet Packages

### Infrastructure
- `Microsoft.EntityFrameworkCore.InMemory` - In-memory database provider
- `Microsoft.Extensions.DependencyInjection.Abstractions` - DI support
- `Microsoft.Extensions.Configuration.Abstractions` - Configuration support

### WebAPI
- `Swashbuckle.AspNetCore` - Swagger/OpenAPI support

## 🎯 Roadmap

- [ ] PostgreSQL integration
- [ ] Enhanced OpenAPI documentation with XML comments
- [ ] API versioning
- [ ] Authentication & Authorization (JWT)
- [ ] Unit tests and integration tests
- [ ] CQRS pattern with MediatR
- [ ] FluentValidation for request validation
- [ ] Global exception handling middleware
- [ ] Logging with Serilog
- [ ] Docker support

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📝 License

This project is licensed under the MIT License.

## 👥 Authors

- Your Name - Initial work

## 🙏 Acknowledgments

- Clean Architecture principles by Robert C. Martin
- ASP.NET Core team for the excellent framework
- Community contributors and supporters
