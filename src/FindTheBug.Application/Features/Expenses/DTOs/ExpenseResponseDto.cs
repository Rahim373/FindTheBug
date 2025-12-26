namespace FindTheBug.Application.Features.Expenses.DTOs;

public record ExpenseResponseDto
{
    public Guid Id { get; init; }
    public DateTime Date { get; init; }
    public string Note { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public string PaymentMethod { get; init; } = string.Empty;
    public string? ReferenceNo { get; init; }
    public string? Attachment { get; init; }
    public string Department { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}

public record ExpenseListItemDto
{
    public Guid Id { get; init; }
    public DateTime Date { get; init; }
    public string Note { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public string PaymentMethod { get; init; } = string.Empty;
    public string? ReferenceNo { get; init; }
    public string Department { get; init; } = string.Empty;
}

public record PaginatedExpenseListDto
{
    public List<ExpenseListItemDto> Items { get; init; } = new();
    public int TotalCount { get; init; }
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
}