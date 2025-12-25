using FindTheBug.Application.Common.Messaging;

namespace FindTheBug.Application.Features.Dispensary.Products.Commands;

public record DeleteProductCommand(Guid Id) : ICommand<bool>;
