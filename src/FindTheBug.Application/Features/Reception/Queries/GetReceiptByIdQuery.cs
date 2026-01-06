using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Features.Reception.DTOs;

namespace FindTheBug.Application.Features.Reception.Queries;

public record GetReceiptByIdQuery(Guid Id) : IQuery<ReceiptResponseDto>;