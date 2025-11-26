using FindTheBug.Application.Common.Messaging;

namespace FindTheBug.Application.Features.TestResults.Commands;

public record VerifyTestResultsCommand(
    Guid TestEntryId,
    string VerifiedBy
) : ICommand<bool>;