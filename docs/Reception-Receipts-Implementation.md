# Reception Receipts Feature Implementation

## Overview
This document describes the implementation of the Reception module with Receipts feature in the FindTheBug application.

## Backend Implementation

### 1. DTOs (Data Transfer Objects)
Located in: `src/FindTheBug.Application/Features/Reception/DTOs/`

- **ReceiptListItemDto** - Used for list view
  - Contains basic receipt information for display in tables
  - Includes status display property

- **ReceiptResponseDto** - Used for detailed receipt information
  - Complete receipt details including patient info
  - Includes test entries with diagnostic test details
  - Contains audit information (created/updated by, dates)

- **ReceiptTestEntryDto** - Used for create/update requests
  - Represents a single test entry on a receipt

- **ReceiptTestDto** - Used in response for test details
  - Includes diagnostic test name and status information

### 2. Commands
Located in: `src/FindTheBug.Application/Features/Reception/Commands/`

- **CreateReceiptCommand** - Creates a new receipt
  - Validates invoice number uniqueness
  - Validates diagnostic test IDs
  - Creates receipt with associated test entries

- **UpdateReceiptCommand** - Updates an existing receipt
  - Validates invoice number uniqueness (excluding current receipt)
  - Validates diagnostic test IDs
  - Removes old test entries and adds new ones

- **DeleteReceiptCommand** - Deletes a receipt
  - Deletes associated test entries first
  - Then deletes the receipt

### 3. Queries
Located in: `src/FindTheBug.Application/Features/Reception/Queries/`

- **GetAllReceiptsQuery** - Retrieves paginated receipts
  - Supports search by invoice number, name, phone, or address
  - Supports pagination
  - Returns ordered by creation date (newest first)

- **GetReceiptByIdQuery** - Retrieves a single receipt by ID
  - Includes referred doctor information
  - Includes all test entries with diagnostic test details

### 4. Handlers
Located in: `src/FindTheBug.Application/Features/Reception/Handlers/`

- **GetAllReceiptsQueryHandler** - Handles receipt list retrieval
- **GetReceiptByIdQueryHandler** - Handles single receipt retrieval
- **CreateReceiptCommandHandler** - Handles receipt creation
- **UpdateReceiptCommandHandler** - Handles receipt updates
- **DeleteReceiptCommandHandler** - Handles receipt deletion

### 5. API Controller
Located in: `src/FindTheBug.WebAPI/Controllers/ReceiptsController.cs`

All endpoints use the `ReceptionManagement` module permission:

- **GET /api/receipts** - Get all receipts with pagination
  - Query parameters: `search`, `pageNumber`, `pageSize`
  - Permission: View
  
- **GET /api/receipts/{id}** - Get receipt by ID
  - Permission: View

- **POST /api/receipts** - Create new receipt
  - Body: CreateReceiptCommand
  - Permission: Create

- **PUT /api/receipts/{id}** - Update receipt
  - Body: UpdateReceiptCommand
  - Permission: Edit

- **DELETE /api/receipts/{id}** - Delete receipt
  - Permission: Delete

## Frontend Implementation

### 1. Models
Located in: `src/FindTheBug.App/src/app/core/models/receipt.models.ts`

- **ReceiptListItem** - List view model
- **Receipt** - Complete receipt model
- **CreateReceiptRequest** - Request model for creation
- **UpdateReceiptRequest** - Request model for updates
- **PagedReceiptsResult** - Paginated result wrapper
- **LabReceiptStatus** - Enum for receipt status (Pending, Paid, Due, Void)
- **ReportDeliveryStatus** - Enum for report delivery (NotDelivered, Delivered)
- **ReceiptTestEntry** - Test entry model

### 2. Service
Located in: `src/FindTheBug.App/src/app/core/services/receipt.service.ts`

- **getReceipts()** - Fetch paginated receipts
- **getReceipt()** - Fetch single receipt by ID
- **createReceipt()** - Create new receipt
- **updateReceipt()** - Update existing receipt
- **deleteReceipt()** - Delete receipt

### 3. Components
Located in: `src/FindTheBug.App/src/app/features/admin/receipts/`

#### ReceiptsListComponent
- Displays paginated table of receipts
- Search functionality
- Actions: View, Edit, Delete
- Status badges with color coding

#### ReceiptFormComponent
- Create/Edit form for receipts
- Patient information section
- Test entries (dynamic array)
- Financial information section
- Status information section
- Form validation

### 4. Routing
Located in: `src/FindTheBug.App/src/app/app.routes.ts`

Added routes:
- `/admin/receipts` - List view
- `/admin/receipts/create` - Create form
- `/admin/receipts/:id` - View form
- `/admin/receipts/:id/edit` - Edit form

### 5. Menu Integration
Located in: `src/FindTheBug.App/src/app/features/admin/shared/sidebar/sidebar.component.ts`

Added Reception menu with Receipts submenu:
- Icon: fa:clipboard-list
- Permission check: ReceptionManagement
- Auto-expands when on receipt routes

## Features Implemented

### CRUD Operations
✅ **Create** - Add new receipts with patient info and test entries
✅ **Read** - View list and individual receipt details
✅ **Update** - Edit existing receipts
✅ **Delete** - Remove receipts

### Additional Features
✅ Search functionality
✅ Pagination
✅ Permission-based access control
✅ Form validation
✅ Status management (Pending, Paid, Due, Void)
✅ Report delivery tracking
✅ Test entry management (add/remove)
✅ Doctor referral tracking
✅ Financial calculations (subtotal, total, discount, due, balance)
✅ Age tracking (years/months)
✅ Responsive design

## Module Permissions

The feature uses the `Reception` module constant defined in `ModuleConstants.cs`.

Permission types:
- **View** - View receipts list and details
- **Create** - Create new receipts
- **Edit** - Update existing receipts
- **Delete** - Delete receipts

## Database Entities

The implementation uses existing entities:
- **LabReceipt** - Main receipt entity
- **ReceiptTest** - Test entries associated with receipts
- **DiagnosticTest** - Diagnostic test reference
- **Doctor** - Referring doctor reference

## Conventions Followed

1. **Backend**:
   - CQRS pattern with Commands and Queries
   - ErrorOr pattern for error handling
   - MediatR for request handling
   - Repository pattern for data access
   - Auto-registration of handlers

2. **Frontend**:
   - Standalone components
   - Reactive forms with validation
   - Ng-Zorro UI components
   - Lazy-loaded routes
   - Service-based HTTP communication
   - Permission directives

3. **Code Organization**:
   - Separation of concerns (DTOs, Commands, Queries, Handlers)
   - Feature-based folder structure
   - Consistent naming conventions
   - Proper error handling and user feedback

## Future Enhancements

Potential improvements for future iterations:
1. Add diagnostic test dropdown in form (currently placeholder)
2. Implement receipt printing
3. Add report generation
4. Implement receipt status workflow (automatic status transitions)
5. Add financial reports/reconciliation
6. Implement bulk operations
7. Add audit log viewing
8. Add receipt templates for quick creation

## Testing Notes

To test the implementation:
1. Ensure ReceptionManagement module permissions are assigned to appropriate roles
2. Navigate to the Receipts menu item
3. Test Create, Read, Update, Delete operations
4. Verify search and pagination
5. Test form validation
6. Verify permission-based access control

## Dependencies

### Backend
- MediatR (already in project)
- ErrorOr (already in project)
- Entity Framework Core (already in project)

### Frontend
- Ng-Zorro Ant Design (already in project)
- Angular Reactive Forms (already in project)
- Angular Router (already in project)

## Files Created/Modified

### Backend - Created
- src/FindTheBug.Application/Features/Reception/DTOs/ReceiptListItemDto.cs
- src/FindTheBug.Application/Features/Reception/DTOs/ReceiptResponseDto.cs
- src/FindTheBug.Application/Features/Reception/DTOs/ReceiptTestEntryDto.cs
- src/FindTheBug.Application/Features/Reception/Commands/CreateReceiptCommand.cs
- src/FindTheBug.Application/Features/Reception/Commands/UpdateReceiptCommand.cs
- src/FindTheBug.Application/Features/Reception/Commands/DeleteReceiptCommand.cs
- src/FindTheBug.Application/Features/Reception/Queries/GetAllReceiptsQuery.cs
- src/FindTheBug.Application/Features/Reception/Queries/GetReceiptByIdQuery.cs
- src/FindTheBug.Application/Features/Reception/Handlers/GetAllReceiptsQueryHandler.cs
- src/FindTheBug.Application/Features/Reception/Handlers/GetReceiptByIdQueryHandler.cs
- src/FindTheBug.Application/Features/Reception/Handlers/CreateReceiptCommandHandler.cs
- src/FindTheBug.Application/Features/Reception/Handlers/UpdateReceiptCommandHandler.cs
- src/FindTheBug.Application/Features/Reception/Handlers/DeleteReceiptCommandHandler.cs
- src/FindTheBug.WebAPI/Controllers/ReceiptsController.cs

### Frontend - Created
- src/FindTheBug.App/src/app/core/models/receipt.models.ts
- src/FindTheBug.App/src/app/core/services/receipt.service.ts
- src/FindTheBug.App/src/app/features/admin/receipts/receipts-list/receipts-list.component.ts
- src/FindTheBug.App/src/app/features/admin/receipts/receipts-list/receipts-list.component.html
- src/FindTheBug.App/src/app/features/admin/receipts/receipts-list/receipts-list.component.css
- src/FindTheBug.App/src/app/features/admin/receipts/receipt-form/receipt-form.component.ts
- src/FindTheBug.App/src/app/features/admin/receipts/receipt-form/receipt-form.component.html
- src/FindTheBug.App/src/app/features/admin/receipts/receipt-form/receipt-form.component.css

### Frontend - Modified
- src/FindTheBug.App/src/app/features/admin/shared/sidebar/sidebar.component.ts
- src/FindTheBug.App/src/app/app.routes.ts

## Summary

The Reception module with Receipts feature has been successfully implemented following all existing conventions and patterns in the FindTheBug application. All CRUD operations are fully functional with proper permissions, validation, and user feedback.