# Application Layer Structure Guide

## Overview

The Application layer follows a **feature-based vertical slice architecture** with strict separation of concerns. Each feature is self-contained with its own commands, queries, handlers, and contracts.

## Directory Structure

```
Features/
├── {FeatureName}/
│   ├── Commands/
│   │   ├── {CommandName}.cs              ← Command definition
│   │   └── Handlers/
│   │       └── {CommandName}Handler.cs   ← Command handler
│   ├── Queries/
│   │   ├── {QueryName}.cs                ← Query definition
│   │   └── Handlers/
│   │       └── {QueryName}Handler.cs     ← Query handler
│   └── Contracts/
│       ├── {Dto}.cs                      ← Data Transfer Objects
│       └── {Response}.cs                 ← Response models
```

## Key Principles

### 1. **Separation of Concerns**

Each component has a single, well-defined responsibility:

- **Commands** - Define write operations (Create, Update, Delete)
- **Queries** - Define read operations (Get, List, Search)
- **Handlers** - Implement the business logic for commands/queries
- **Contracts** - Define DTOs and response models

### 2. **CQRS Pattern**

Commands and queries are strictly separated:

```csharp
// Command - Returns result of operation
public record CreatePatientCommand(...) : ICommand<Patient>;

// Query - Returns data
public record GetPatientByIdQuery(Guid Id) : IQuery<Patient>;
```

### 3. **Handler Organization**

All handlers are organized in dedicated `Handlers` subdirectories:

```
Commands/
├── CreatePatientCommand.cs
└── Handlers/
    └── CreatePatientCommandHandler.cs

Queries/
├── GetPatientByIdQuery.cs
└── Handlers/
    └── GetPatientByIdQueryHandler.cs
```

## Example Feature Structure

### Patients Feature

```
Features/Patients/
├── Commands/
│   ├── CreatePatientCommand.cs
│   └── Handlers/
│       └── CreatePatientCommandHandler.cs
├── Queries/
│   ├── GetPatientByIdQuery.cs
│   ├── GetAllPatientsQuery.cs
│   └── Handlers/
│       ├── GetPatientByIdQueryHandler.cs
│       └── GetAllPatientsQueryHandler.cs
└── (No contracts needed - returns domain entities)
```

### Authentication Feature

```
Features/Authentication/
├── Commands/
│   ├── LoginCommand.cs
│   ├── RefreshTokenCommand.cs
│   ├── ChangePasswordCommand.cs
│   ├── RequestPasswordResetCommand.cs
│   ├── ResetPasswordCommand.cs
│   ├── RevokeTokenCommand.cs
│   └── Handlers/
│       ├── LoginCommandHandler.cs
│       ├── RefreshTokenCommandHandler.cs
│       ├── ChangePasswordCommandHandler.cs
│       ├── RequestPasswordResetCommandHandler.cs
│       ├── ResetPasswordCommandHandler.cs
│       └── RevokeTokenCommandHandler.cs
└── Contracts/
    ├── LoginResponse.cs
    ├── RefreshTokenResponse.cs
    └── UserInfo.cs
```

## Command Definition

Commands represent write operations and implement `ICommand<TResponse>`:

```csharp
using FindTheBug.Application.Common.Messaging;
using FindTheBug.Domain.Entities;

namespace FindTheBug.Application.Features.Patients.Commands;

public record CreatePatientCommand(
    string FirstName,
    string LastName,
    string MobileNumber,
    DateTime? DateOfBirth,
    string? Gender,
    string? Email,
    string? Address
) : ICommand<Patient>;
```

## Command Handler Implementation

Handlers implement `ICommandHandler<TCommand, TResponse>`:

```csharp
using ErrorOr;
using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Application.Common.Messaging;
using FindTheBug.Domain.Entities;

namespace FindTheBug.Application.Features.Patients.Commands;

public class CreatePatientCommandHandler(IUnitOfWork unitOfWork) 
    : ICommandHandler<CreatePatientCommand, Patient>
{
    public async Task<ErrorOr<Patient>> Handle(
        CreatePatientCommand request, 
        CancellationToken cancellationToken)
    {
        var patient = new Patient
        {
            Id = Guid.NewGuid(),
            FirstName = request.FirstName,
            LastName = request.LastName,
            // ... other properties
        };

        var created = await unitOfWork.Repository<Patient>()
            .AddAsync(patient, cancellationToken);
        
        return created;
    }
}
```

## Query Definition

Queries represent read operations and implement `IQuery<TResponse>`:

```csharp
using FindTheBug.Application.Common.Messaging;
using FindTheBug.Domain.Entities;

namespace FindTheBug.Application.Features.Patients.Queries;

public record GetPatientByIdQuery(Guid Id) : IQuery<Patient>;
```

## Query Handler Implementation

Handlers implement `IQueryHandler<TQuery, TResponse>`:

```csharp
using ErrorOr;
using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Application.Common.Messaging;
using FindTheBug.Domain.Entities;

namespace FindTheBug.Application.Features.Patients.Queries;

public class GetPatientByIdQueryHandler(IUnitOfWork unitOfWork) 
    : IQueryHandler<GetPatientByIdQuery, Patient>
{
    public async Task<ErrorOr<Patient>> Handle(
        GetPatientByIdQuery request, 
        CancellationToken cancellationToken)
    {
        var patient = await unitOfWork.Repository<Patient>()
            .GetByIdAsync(request.Id, cancellationToken);
        
        if (patient is null)
            return Error.NotFound("Patient.NotFound", 
                $"Patient with ID {request.Id} not found");

        return patient;
    }
}
```

## Contracts (DTOs and Response Models)

Contracts are separated into their own files in the `Contracts` directory:

```csharp
namespace FindTheBug.Application.Features.Authentication.Contracts;

public record LoginResponse(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt,
    UserInfo User
);

public record UserInfo(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    string Roles
);
```

## Controller Integration

Controllers use `ISender` from MediatR to dispatch commands and queries:

```csharp
[ApiController]
[Route("api/[controller]")]
public class PatientsController(ISender mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreatePatientCommand command, 
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(
        Guid id, 
        CancellationToken cancellationToken)
    {
        var query = new GetPatientByIdQuery(id);
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }
}
```

## Benefits of This Structure

### 1. **Clear Organization**
- Easy to locate commands, queries, and handlers
- Consistent structure across all features
- Reduced cognitive load when navigating codebase

### 2. **Separation of Concerns**
- Commands/queries define the contract
- Handlers implement the logic
- Contracts define the data shapes
- Each has a single responsibility

### 3. **Maintainability**
- Changes to command structure don't require opening handler code
- Easy to add new commands/queries
- Clear boundaries between components

### 4. **Testability**
- Handlers can be tested independently
- Commands/queries are just data structures
- Easy to mock dependencies

### 5. **Scalability**
- New features follow the same pattern
- Easy for teams to work in parallel
- Minimal merge conflicts

## Naming Conventions

- **Commands**: `{Action}{Entity}Command` (e.g., `CreatePatientCommand`)
- **Queries**: `Get{Entity}By{Criteria}Query` or `GetAll{Entity}Query`
- **Handlers**: `{CommandOrQueryName}Handler`
- **Contracts**: `{Entity}{Purpose}` (e.g., `LoginResponse`, `UserInfo`)

## Complete Feature List

The application includes the following features:

1. **Patients** - Patient management (3 commands/queries)
2. **TestParameters** - Test parameter configuration (4 commands/queries)
3. **Authentication** - User authentication and security (6 commands)
4. **TestResults** - Test result management (4 commands/queries)
5. **Metrics** - System metrics and monitoring (1 query)
6. **DiagnosticTests** - Test catalog management (1 command)
7. **Invoices** - Billing and invoicing (1 command)
8. **TestEntries** - Test entry registration (1 command)

Total: **21 commands/queries** with **21 handlers** organized in **11 Handlers directories**.
