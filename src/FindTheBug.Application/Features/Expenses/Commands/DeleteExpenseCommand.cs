using FindTheBug.Application.Common.Messaging;

namespace FindTheBug.Application.Features.Expenses.Commands;

public record DeleteExpenseCommand(Guid Id) : ICommand<bool>;