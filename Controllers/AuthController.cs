using System.IdentityModel.Tokens.Jwt;
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
    private readonly IJwtTokenService _jwtTokenService;
    private readonly ITokenRevocationStore _tokenRevocationStore;

    public AuthController(
        IUserAuthService userAuthService,
        IJwtTokenService jwtTokenService,
        ITokenRevocationStore tokenRevocationStore)
    {
        _userAuthService = userAuthService;
        _jwtTokenService = jwtTokenService;
        _tokenRevocationStore = tokenRevocationStore;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public ActionResult<LoginResponse> Login([FromBody] LoginRequest request)
    {
        if (!_userAuthService.ValidateCredentials(request.Username, request.Password))
        {
            return Unauthorized(new { error = "Invalid username or password." });
        }

        return Ok(_jwtTokenService.CreateToken(request.Username!));
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
