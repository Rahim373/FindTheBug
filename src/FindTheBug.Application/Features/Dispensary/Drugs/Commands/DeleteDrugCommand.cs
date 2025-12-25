using FindTheBug.Application.Common.Messaging;

namespace FindTheBug.Application.Features.Dispensary.Drugs.Commands;

public record DeleteDrugCommand(Guid Id) : ICommand<bool>;
