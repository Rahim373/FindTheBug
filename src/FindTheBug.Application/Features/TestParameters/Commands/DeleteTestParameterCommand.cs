using FindTheBug.Application.Common.Messaging;

namespace FindTheBug.Application.Features.TestParameters.Commands;

public record DeleteTestParameterCommand(Guid Id) : ICommand<bool>;