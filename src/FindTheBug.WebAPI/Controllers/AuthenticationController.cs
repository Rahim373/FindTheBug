using FindTheBug.Application.Features.Authentication.Commands;
using FindTheBug.Application.Features.Authentication.Contracts;
using FindTheBug.WebAPI.Contracts.Requests;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FindTheBug.WebAPI.Controllers;

/// <summary>
/// Authentication and authorization endpoints
/// </summary>
public class AuthenticationController(ISender mediator, IMapper mapper) : BaseApiController
{
    /// <summary>
    /// Authenticate user and get access token
    /// </summary>
    /// <param name="request">Login credentials</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Authentication response with access and refresh tokens</returns>
    /// <response code="200">Returns the authentication response</response>
    /// <response code="401">If credentials are invalid</response>
    /// <response code="400">If the request is invalid</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginCommand request, CancellationToken cancellationToken)
    {
        var command = mapper.Map<LoginCommand>(request);
        var result = await mediator.Send(command, cancellationToken);

        return result.Match(
            response => Ok(response),
            Problem);
    }

    /// <summary>
    /// Refresh access token using refresh token
    /// </summary>
    /// <param name="request">Refresh token request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>New authentication response with refreshed tokens</returns>
    /// <response code="200">Returns the new authentication response</response>
    /// <response code="401">If the refresh token is invalid or expired</response>
    [HttpPost("refresh-token")]
    [ProducesResponseType(typeof(RefreshTokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var command = mapper.Map<RefreshTokenCommand>(request);
        var result = await mediator.Send(command, cancellationToken);

        return result.Match(
            response => Ok(response),
            Problem);
    }

    /// <summary>
    /// Request password reset for a user
    /// </summary>
    /// <param name="request">Email address for password reset</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success indicator</returns>
    /// <response code="200">Password reset email sent successfully</response>
    /// <response code="404">If the user with the email is not found</response>
    [HttpPost("request-password-reset")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RequestPasswordReset([FromBody] RequestPasswordResetCommand request, CancellationToken cancellationToken)
    {
        var command = mapper.Map<RequestPasswordResetCommand>(request);
        var result = await mediator.Send(command, cancellationToken);

        return result.Match(
            success => Ok(success),
            Problem);
    }

    /// <summary>
    /// Reset password using reset token
    /// </summary>
    /// <param name="request">Reset token and new password</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success indicator</returns>
    /// <response code="200">Password reset successfully</response>
    /// <response code="400">If the reset token is invalid or expired</response>
    [HttpPost("reset-password")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var command = mapper.Map<ResetPasswordCommand>(request);
        var result = await mediator.Send(command, cancellationToken);

        return result.Match(
            success => Ok(success),
            Problem);
    }

    /// <summary>
    /// Change password for authenticated user
    /// </summary>
    /// <param name="request">Current and new password</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success indicator</returns>
    /// <response code="200">Password changed successfully</response>
    /// <response code="400">If the current password is incorrect</response>
    /// <response code="401">If the user is not authenticated</response>
    [HttpPost("change-password")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request, CancellationToken cancellationToken)
    {
        var command = mapper.Map<ChangePasswordCommand>(request);
        var result = await mediator.Send(command, cancellationToken);

        return result.Match(
            success => Ok(success),
            Problem);
    }
}
