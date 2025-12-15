using FindTheBug.Application.Common.Messaging;

namespace FindTheBug.Application.Features.Laboratory.TestParameters.Commands;

public record DeleteTestParameterCommand(Guid Id) : ICommand<bool>;