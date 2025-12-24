using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Projectly.Api.Models;
using Projectly.Api.Services;
using Projectly.Shared.Services;
using Projectly.Shared.Models;
using Projectly.Api.Dto.Response;
using System.Security.Claims;
using Google.Apis.Auth;
using Projectly.Api.Dto.Request;

namespace Projectly.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UsuarioService _usuarioService;
    private readonly JwtService _jwtService;
    private readonly JwtSettings _jwtSettings;

    public AuthController(UsuarioService usuarioService, JwtService jwtService, JwtSettings jwtSettings)
    {
        _usuarioService = usuarioService;
        _jwtService = jwtService;
        _jwtSettings = jwtSettings;
    }

    [HttpPost("google-login")]
    [AllowAnonymous] 
    public async Task<ActionResult<LoginResponse>> GoogleLogin([FromBody] GoogleLoginRequest request)
    {
        if (string.IsNullOrEmpty(request.IdToken))
        {
            return BadRequest(new { message = "Token de Google requerido" });
        }

        GoogleJsonWebSignature.Payload payload;
        try
        {
            payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken);
        }
        catch
        {
            return Unauthorized(new { message = "Token de Google inválido" });
        }

        var usuario = await _usuarioService.getUsuarioByEmail(payload.Email);
        if (usuario == null)
        {
            usuario = new Usuarios
            {
                UsuarioId = Guid.NewGuid().ToString(),
                Nombre = payload.GivenName ?? payload.Name ?? "",
                Apellido = payload.FamilyName ?? "",
                Email = payload.Email,
                isPremium = false,
                FechaCreacion = DateTime.UtcNow,
                LastModified = DateTime.UtcNow
            };

            if (!await _usuarioService.Insertar(usuario))
            {
                return BadRequest(new { message = "Error al crear el usuario" });
            }
        }

        var token = _jwtService.GenerateToken(usuario);
        var expiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes);

        var response = new LoginResponse
        {
            Token = token,
            UsuarioId = usuario.UsuarioId,
            Email = usuario.Email,
            Nombre = usuario.Nombre,
            IsPremium = usuario.isPremium,
            ExpiresAt = expiresAt
        };

        return Ok(response);
    }

    [HttpPost("refresh-token")]
    [Authorize]
    public async Task<ActionResult<LoginResponse>> RefreshToken()
    {
        var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(currentUserId))
        {
            return Unauthorized(new { message = "Token inválido" });
        }

        var usuario = await _usuarioService.getUsuarioById(currentUserId);
        if (usuario == null)
        {
            return Unauthorized(new { message = "Usuario no encontrado" });
        }

        var newToken = _jwtService.GenerateToken(usuario);
        var expiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes);

        var response = new LoginResponse
        {
            Token = newToken,
            UsuarioId = usuario.UsuarioId,
            Email = usuario.Email,
            Nombre = usuario.Nombre,
            IsPremium = usuario.isPremium,
            ExpiresAt = expiresAt
        };

        return Ok(response);
    }

    [HttpGet("profile")]
    [Authorize]
    public async Task<ActionResult<UsuariosResponse>> GetProfile()
    {
        var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(currentUserId))
        {
            return Unauthorized();
        }

        var usuario = await _usuarioService.getUsuarioById(currentUserId);
        if (usuario == null)
        {
            return NotFound(new { message = "Usuario no encontrado" });
        }

        var response = new UsuariosResponse
        {
            UsuarioId = usuario.UsuarioId,
            Nombre = usuario.Nombre,
            Apellido = usuario.Apellido,
            Email = usuario.Email,
            isPremium = usuario.isPremium,
            FechaCreacion = usuario.FechaCreacion,
            LastModified = usuario.LastModified
        };

        return Ok(response);
    }

    [HttpPost("validate-token")]
    [AllowAnonymous] 
    public IActionResult ValidateToken([FromBody] ValidateTokenRequest request)
    {
        if (string.IsNullOrEmpty(request.Token))
        {
            return BadRequest(new { message = "Token requerido" });
        }

        var principal = _jwtService.ValidateToken(request.Token);
        if (principal == null)
        {
            return Unauthorized(new { message = "Token inválido o expirado" });
        }

        var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var email = principal.FindFirst(ClaimTypes.Email)?.Value;
        var nombre = principal.FindFirst(ClaimTypes.Name)?.Value;

        return Ok(new
        {
            message = "Token válido",
            isValid = true,
            user = new { userId, email, nombre }
        });
    }

    [HttpPost("logout")]
    [Authorize]
    public IActionResult Logout()
    {
        return Ok(new { message = "Sesión cerrada exitosamente" });
    }
}

