using System.IdentityModel.Tokens.Jwt;
using InvoiceService.Filters;
using InvoiceService.Models;
using InvoiceService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InvoiceService.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IUserAuthService _userAuthService;
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly ITokenRevocationStore _tokenRevocationStore;

    public AuthController(
        IUserAuthService userAuthService,
        IUserRepository userRepository,
        IJwtTokenService jwtTokenService,
        ITokenRevocationStore tokenRevocationStore)
    {
        _userAuthService = userAuthService;
        _userRepository = userRepository;
        _jwtTokenService = jwtTokenService;
        _tokenRevocationStore = tokenRevocationStore;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        if (!await _userAuthService.ValidateCredentialsAsync(request.Username, request.Password, cancellationToken))
        {
            return Unauthorized(new { error = "Invalid username or password." });
        }

        return Ok(_jwtTokenService.CreateToken(request.Username!));
    }

    [ServiceFilter(typeof(RequireApiKeyAttribute))]
    [HttpPost("users")]
    public async Task<ActionResult<UserResponse>> CreateUser([FromBody] CreateUserRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(new { error = "Username and password are required." });
        }

        try
        {
            var user = await _userRepository.CreateAsync(request.Username, request.Password, cancellationToken);
            return Ok(new UserResponse
            {
                Id = user.Id ?? string.Empty,
                Username = user.Username,
                CreatedAtUtc = user.CreatedAtUtc
            });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { error = ex.Message });
        }
    }

    [Authorize(AuthenticationSchemes = "Bearer")]
    [HttpPost("logout")]
    public ActionResult Logout()
    {
        var jti = User.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
        var expiresAt = User.FindFirst(JwtRegisteredClaimNames.Exp)?.Value;

        if (string.IsNullOrWhiteSpace(jti))
        {
            return BadRequest(new { error = "Token is missing jti claim." });
        }

        var expiresAtUtc = DateTime.UtcNow.AddHours(8);
        if (long.TryParse(expiresAt, out var expUnix))
        {
            expiresAtUtc = DateTimeOffset.FromUnixTimeSeconds(expUnix).UtcDateTime;
        }

        _tokenRevocationStore.Revoke(jti, expiresAtUtc);
        return Ok(new { message = "Logged out successfully." });
    }
}
