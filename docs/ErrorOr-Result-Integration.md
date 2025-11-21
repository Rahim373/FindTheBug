# ErrorOr + Result Wrapper Integration

## Overview
This document explains how the ErrorOr pattern integrates with the Result wrapper middleware to provide consistent API responses.

## Request/Response Flow

```
Client Request
    ↓
1. RequestLoggingMiddleware (logs request)
    ↓
2. GlobalExceptionHandlerMiddleware (catches unhandled exceptions)
    ↓
3. TenantResolutionMiddleware (resolves tenant)
    ↓
4. ResultWrapperMiddleware (wraps responses - INTERCEPTS HERE)
    ↓
5. Controller receives request
    ↓
6. Controller calls MediatR: mediator.Send(command)
    ↓
7. MediatR Handler executes and returns ErrorOr<T>
    ↓
8. ErrorOrActionFilter intercepts (registered globally)
    ↓
9. Filter checks ErrorOr.IsError:
   - If TRUE: Creates ProblemDetails with status code (404, 400, 409, etc.)
   - If FALSE: Extracts value from ErrorOr<T>
    ↓
10. Response goes back through middleware pipeline
    ↓
11. ResultWrapperMiddleware intercepts response:
    - Reads response body
    - Wraps in Result<T> format
    - Returns to client
```

## Response Formats

### Success Response (2xx)
```json
{
  "isSuccess": true,
  "data": {
    "id": "123e4567-e89b-12d3-a456-426614174000",
    "name": "John Doe",
    "mobileNumber": "+1234567890"
  },
  "errorMessage": null,
  "errors": []
}
```

### Error Response (4xx/5xx)
```json
{
  "isSuccess": false,
  "data": null,
  "errorMessage": "Patient with ID 123 not found",
  "errors": [
    "Patient with ID 123 not found"
  ]
}
```

### Validation Error Response
```json
{
  "isSuccess": false,
  "data": null,
  "errorMessage": "Validation failed",
  "errors": [
    "Validation failed",
    "Mobile number is required",
    "Name must be at least 2 characters"
  ]
}
```

## Code Examples

### 1. MediatR Handler (Application Layer)
```csharp
public record GetPatientByIdQuery(Guid Id) : IRequest<ErrorOr<Patient>>;

public class GetPatientByIdQueryHandler(IUnitOfWork unitOfWork) 
    : IRequestHandler<GetPatientByIdQuery, ErrorOr<Patient>>
{
    public async Task<ErrorOr<Patient>> Handle(
        GetPatientByIdQuery request, 
        CancellationToken cancellationToken)
    {
        var patient = await unitOfWork.Repository<Patient>()
            .GetByIdAsync(request.Id, cancellationToken);

        if (patient is null)
            return Error.NotFound("Patient.NotFound", $"Patient with ID {request.Id} not found");

        return patient;
    }
}
```

### 2. Controller (WebAPI Layer)
```csharp
[HttpGet("{id}")]
public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
{
    var query = new GetPatientByIdQuery(id);
    var result = await mediator.Send(query, cancellationToken);
    return Ok(result); // Just return Ok with ErrorOr<T>
}
```

### 3. ErrorOrActionFilter (Automatic)
```csharp
// Registered globally in Program.cs
// Automatically intercepts all controller responses
// Checks if result is ErrorOr<T>
// - If error: converts to ProblemDetails with proper status code
// - If success: extracts value from ErrorOr<T>
```

### 4. ResultWrapperMiddleware (Automatic)
```csharp
// Registered in middleware pipeline
// Intercepts all responses after controllers
// Wraps in Result<T> format:
// - Success: { isSuccess: true, data: {...} }
// - Error: { isSuccess: false, errorMessage: "...", errors: [...] }
```

## Benefits

### 1. **Clean Controller Code**
- Controllers just return `Ok(result)` where result is `ErrorOr<T>`
- No manual error handling or Match() calls
- No try-catch blocks needed

### 2. **Type-Safe Error Handling**
- Handlers return `ErrorOr<T>` which forces handling of both success and error cases
- Compile-time safety for error scenarios

### 3. **Consistent API Responses**
- All responses wrapped in same `Result<T>` format
- Clients always know what to expect
- Easy to parse and handle on client side

### 4. **Automatic Status Code Mapping**
- `Error.NotFound` → 404
- `Error.Validation` → 400
- `Error.Conflict` → 409
- `Error.Unauthorized` → 401
- `Error.Forbidden` → 403
- Default → 500

### 5. **No Boilerplate Code**
- Middleware handles all wrapping automatically
- Filters handle all ErrorOr conversion automatically
- Developers focus on business logic

## Middleware Pipeline Order

```
1. RequestLoggingMiddleware
2. GlobalExceptionHandlerMiddleware
3. TenantResolutionMiddleware
4. ResultWrapperMiddleware ← Wraps responses
5. HTTP Metrics
6. Controllers (with ErrorOrActionFilter) ← Handles ErrorOr
```

## Testing Example

### Request
```http
GET /api/patients/123e4567-e89b-12d3-a456-426614174000
```

### Handler Returns (Internal)
```csharp
ErrorOr<Patient> result = patient; // Success case
// OR
ErrorOr<Patient> result = Error.NotFound(...); // Error case
```

### ErrorOrActionFilter Processes
```csharp
// Success: Extracts patient object
// Error: Creates ProblemDetails with 404 status
```

### ResultWrapperMiddleware Wraps
```json
// Success
{
  "isSuccess": true,
  "data": { "id": "...", "name": "..." },
  "errorMessage": null,
  "errors": []
}

// Error
{
  "isSuccess": false,
  "data": null,
  "errorMessage": "Patient not found",
  "errors": ["Patient not found"]
}
```

## Excluded Endpoints

The following endpoints are NOT wrapped by ResultWrapperMiddleware:
- `/health` - Health check endpoint
- `/metrics` - Prometheus metrics
- `/swagger` - Swagger UI and documentation

These endpoints return their native formats.
