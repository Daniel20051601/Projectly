using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Projectly.Api.Dto.Request;
using Projectly.Api.Dto.Response;
using Projectly.Shared.Models;
using Projectly.Shared.Services;
using System.Security.Claims;

namespace Projectly.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsuariosController : ControllerBase
{
    private readonly UsuarioService _usuarioService;

    public UsuariosController(UsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
    }

    [HttpGet("email/{email}")]
    public async Task<ActionResult<UsuariosResponse>> GetUsuarioByEmail(string email)
    {
        var usuario = await _usuarioService.getUsuarioByEmail(email);
        if (usuario == null)
        {
            return NotFound($"Usuario con email {email} no encontrado");
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

  

    [HttpPut("{id}")]
    public async Task<ActionResult<UsuariosResponse>> UpdateUsuario(string id, [FromBody] UsuariosRequest request)
    {
        var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if(currentUserId != id)
        {
            return Forbid("No tienes permiso para modificar este usuario");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var usuarioExistente = await _usuarioService.getUsuarioById(id);
        if (usuarioExistente == null)
        {
            return NotFound($"Usuario con ID {id} no encontrado");
        }

        if (usuarioExistente.Email != request.Email && await _usuarioService.ExisteEmail(request.Email))
        {
            return Conflict(new { message = "El email ya está en uso por otro usuario", email = request.Email });
        }

        usuarioExistente.Nombre = request.Nombre;
        usuarioExistente.Apellido = request.Apellido;
        usuarioExistente.LastModified = DateTime.UtcNow;

        if (await _usuarioService.Modificar(usuarioExistente))
        {
            var response = new UsuariosResponse
            {
                UsuarioId = usuarioExistente.UsuarioId,
                Nombre = usuarioExistente.Nombre,
                Apellido = usuarioExistente.Apellido,
                Email = usuarioExistente.Email,
                isPremium = usuarioExistente.isPremium,
                FechaCreacion = usuarioExistente.FechaCreacion,
                LastModified = usuarioExistente.LastModified
            };

            return Ok(response);
        }

        return BadRequest(new { message = "No se pudo actualizar el usuario" });
    }

    [HttpPatch("{id}/premium")]
    public async Task<ActionResult<UsuariosResponse>> UpdatePremium(string id, [FromBody] bool isPremium)
    {
        var usuarioExistente = await _usuarioService.getUsuarioById(id);
        if (usuarioExistente == null)
        {
            return NotFound($"Usuario con ID {id} no encontrado");
        }

        if (await _usuarioService.actualizarPremium(id, isPremium))
        {
            var usuarioActualizado = await _usuarioService.getUsuarioById(id);

            var response = new UsuariosResponse
            {
                UsuarioId = usuarioActualizado!.UsuarioId,
                Nombre = usuarioActualizado.Nombre,
                Apellido = usuarioActualizado.Apellido,
                Email = usuarioActualizado.Email,
                isPremium = usuarioActualizado.isPremium,
                FechaCreacion = usuarioActualizado.FechaCreacion,
                LastModified = usuarioActualizado.LastModified
            };

            return Ok(response);
        }

        return BadRequest(new { message = "No se pudo actualizar el estado premium" });
    }
}