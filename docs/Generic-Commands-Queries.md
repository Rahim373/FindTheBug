# Generic Commands and Queries Pattern

This document explains the generic `ICommand` and `IQuery` pattern used in this application to reduce boilerplate code when working with MediatR and ErrorOr.

## Overview

Instead of implementing `IRequest<ErrorOr<TResponse>>` directly for every command and query, we use generic base interfaces that encapsulate this pattern.

## Base Interfaces

### ICommand<TResponse>
```csharp
public interface ICommand<TResponse> : IRequest<ErrorOr<TResponse>>
{
}
```

### IQuery<TResponse>
```csharp
public interface IQuery<TResponse> : IRequest<ErrorOr<TResponse>>
{
}
```

### ICommandHandler<TCommand, TResponse>
```csharp
public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, ErrorOr<TResponse>>
    where TCommand : ICommand<TResponse>
{
}
```

### IQueryHandler<TQuery, TResponse>
```csharp
public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, ErrorOr<TResponse>>
    where TQuery : IQuery<TResponse>
{
}
```

## Benefits

1. **Less Boilerplate**: No need to write `IRequest<ErrorOr<TResponse>>` repeatedly
2. **Clear Intent**: Distinguishes between commands (write operations) and queries (read operations)
3. **Type Safety**: Maintains full type safety with generic constraints
4. **Consistency**: Enforces consistent error handling across all operations

## Usage Examples

### Creating a Command

**Before:**
```csharp
public record CreatePatientCommand(...) : IRequest<ErrorOr<Patient>>;

public class CreatePatientCommandHandler(IUnitOfWork unitOfWork) 
    : IRequestHandler<CreatePatientCommand, ErrorOr<Patient>>
{
    public async Task<ErrorOr<Patient>> Handle(CreatePatientCommand request, CancellationToken cancellationToken)
    {
        // Implementation
    }
}
```

**After:**
```csharp
public record CreatePatientCommand(...) : ICommand<Patient>;

public class CreatePatientCommandHandler(IUnitOfWork unitOfWork) 
    : ICommandHandler<CreatePatientCommand, Patient>
{
    public async Task<ErrorOr<Patient>> Handle(CreatePatientCommand request, CancellationToken cancellationToken)
    {
        // Implementation
    }
}
```

### Creating a Query

**Before:**
```csharp
public record GetPatientByIdQuery(Guid Id) : IRequest<ErrorOr<Patient>>;

public class GetPatientByIdQueryHandler(IUnitOfWork unitOfWork) 
    : IRequestHandler<GetPatientByIdQuery, ErrorOr<Patient>>
{
    public async Task<ErrorOr<Patient>> Handle(GetPatientByIdQuery request, CancellationToken cancellationToken)
    {
        // Implementation
    }
}
```

**After:**
```csharp
public record GetPatientByIdQuery(Guid Id) : IQuery<Patient>;

public class GetPatientByIdQueryHandler(IUnitOfWork unitOfWork) 
    : IQueryHandler<GetPatientByIdQuery, Patient>
{
    public async Task<ErrorOr<Patient>> Handle(GetPatientByIdQuery request, CancellationToken cancellationToken)
    {
        // Implementation
    }
}
```

## CQRS Separation

The distinction between `ICommand` and `IQuery` helps enforce CQRS principles:

- **Commands** (`ICommand<TResponse>`): Modify state, may return created/updated entities
- **Queries** (`IQuery<TResponse>`): Read-only operations, never modify state

## Integration with ErrorOr

All commands and queries automatically return `ErrorOr<TResponse>`, which allows for:

- Functional error handling without exceptions
- Multiple error types (NotFound, Validation, Conflict, etc.)
- Clean separation between success and failure paths

## Controller Usage

Controllers remain unchanged - they still use `ISender` from MediatR:

```csharp
[HttpPost]
public async Task<IActionResult> Create([FromBody] CreatePatientCommand command, CancellationToken cancellationToken)
{
    var result = await mediator.Send(command, cancellationToken);
    return Ok(result);
}
```

The `ErrorOrActionFilter` automatically converts `ErrorOr<T>` responses to appropriate HTTP responses, and the `ResultWrapperMiddleware` wraps everything in a consistent `Result<T>` format.

## File Location

The generic interfaces are defined in:
- `src/FindTheBug.Application/Common/Messaging/ICommand.cs`

## Migration Guide

To migrate existing commands/queries:

1. Replace `IRequest<ErrorOr<TResponse>>` with `ICommand<TResponse>` or `IQuery<TResponse>`
2. Replace `IRequestHandler<TCommand, ErrorOr<TResponse>>` with `ICommandHandler<TCommand, TResponse>` or `IQueryHandler<TQuery, TResponse>`
3. Add `using FindTheBug.Application.Common.Messaging;`
4. Remove `using MediatR;` if it's no longer needed (though it's still compatible)

## Examples in Codebase

See the following files for complete examples:

- `src/FindTheBug.Application/Features/Patients/Commands/CreatePatientCommand.cs`
- `src/FindTheBug.Application/Features/Patients/Queries/GetPatientByIdQuery.cs`
- `src/FindTheBug.Application/Features/Patients/Queries/GetAllPatientsQuery.cs`
