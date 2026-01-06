using FindTheBug.Application.Common.Messaging;

namespace FindTheBug.Application.Features.Reception.Commands;

public record DeleteReceiptCommand(Guid Id) : ICommand<bool>;