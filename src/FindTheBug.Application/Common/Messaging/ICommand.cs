using ErrorOr;
using FindTheBug.Application.Common.Models;
using MediatR;

namespace FindTheBug.Application.Common.Messaging;

/// <summary>
/// Base interface for commands that return ErrorOr<TResponse>
/// </summary>
public interface ICommand<TResponse> : IRequest<ErrorOr<Result<TResponse>>>
{
}

/// <summary>
/// Base interface for queries that return ErrorOr<TResponse>
/// </summary>
public interface IQuery<TResponse> : IRequest<ErrorOr<Result<TResponse>>>
{
}

/// <summary>
/// Base interface for command handlers
/// </summary>
public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, ErrorOr<Result<TResponse>>>
    where TCommand : ICommand<TResponse>
{
}

/// <summary>
/// Base interface for query handlers
/// </summary>
public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, ErrorOr<Result<TResponse>>>
    where TQuery : IQuery<TResponse>
{
}
