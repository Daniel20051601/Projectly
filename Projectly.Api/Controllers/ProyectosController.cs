using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Projectly.Api.Dto.Request;
using Projectly.Api.Dto.Response;
using Projectly.Shared.Models;
using Projectly.Shared.Services;

namespace Projectly.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProyectosController : ControllerBase
{
    private readonly ProyectoService _proyectoService;

    public ProyectosController(ProyectoService proyectoService)
    {
        _proyectoService = proyectoService;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProyectosResponse>> GetProyecto(Guid id)
    {
        var proyecto = await _proyectoService.getProyectoById(id);
        if (proyecto == null)
        {
            return NotFound($"Proyecto con ID {id} no encontrado");
        }

        var response = new ProyectosResponse
        {
            ProyectoId = proyecto.ProyectoId,
            UsuarioId = proyecto.UsuarioId,
            Nombre = proyecto.Nombre,
            Descripcion = proyecto.Descripcion,
            FechaCreacion = proyecto.FechaCreacion,
            FechaLimite = proyecto.FechaLimite,
            Color = proyecto.Color,
            Icono = proyecto.Icono,
            isPlantilla = proyecto.isPlantilla,
            isDeleted = proyecto.isDeleted,
            LastModified = proyecto.LastModified
        };

        return Ok(response);
    }

    [HttpGet("usuario/{usuarioId}")]
    public async Task<ActionResult<List<ProyectosResponse>>> GetProyectosByUsuario(string usuarioId)
    {
        var proyectos = await _proyectoService.getProyectosByUsuarioId(usuarioId);
        var response = proyectos.Select(p => new ProyectosResponse
        {
            ProyectoId = p.ProyectoId,
            UsuarioId = p.UsuarioId,
            Nombre = p.Nombre,
            Descripcion = p.Descripcion,
            FechaCreacion = p.FechaCreacion,
            FechaLimite = p.FechaLimite,
            Color = p.Color,
            Icono = p.Icono,
            isPlantilla = p.isPlantilla,
            isDeleted = p.isDeleted,
            LastModified = p.LastModified
        }).ToList();

        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<ProyectosResponse>> CreateProyecto([FromBody] ProyectosRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var proyecto = new Proyectos
        {
            ProyectoId = Guid.NewGuid(),
            UsuarioId = request.UsuarioId,
            Nombre = request.Nombre,
            Descripcion = request.Descripcion,
            FechaCreacion = DateTime.UtcNow,
            FechaLimite = request.FechaLimite,
            Color = request.Color,
            Icono = request.Icono,
            isPlantilla = request.isPlantilla,
            isDeleted = false,
            LastModified = DateTime.UtcNow
        };

        if (await _proyectoService.Insertar(proyecto))
        {
            var response = new ProyectosResponse
            {
                ProyectoId = proyecto.ProyectoId,
                UsuarioId = proyecto.UsuarioId,
                Nombre = proyecto.Nombre,
                Descripcion = proyecto.Descripcion,
                FechaCreacion = proyecto.FechaCreacion,
                FechaLimite = proyecto.FechaLimite,
                Color = proyecto.Color,
                Icono = proyecto.Icono,
                isPlantilla = proyecto.isPlantilla,
                isDeleted = proyecto.isDeleted,
                LastModified = proyecto.LastModified
            };

            return CreatedAtAction(nameof(GetProyecto), new { id = proyecto.ProyectoId }, response);
        }

        return BadRequest(new { message = "No se pudo crear el proyecto" });
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ProyectosResponse>> UpdateProyecto(Guid id, [FromBody] ProyectosRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var proyectoExistente = await _proyectoService.getProyectoById(id);
        if (proyectoExistente == null)
        {
            return NotFound($"Proyecto con ID {id} no encontrado");
        }

        var proyecto = new Proyectos
        {
            ProyectoId = id,
            UsuarioId = request.UsuarioId,
            Nombre = request.Nombre,
            Descripcion = request.Descripcion,
            FechaCreacion = proyectoExistente.FechaCreacion,
            FechaLimite = request.FechaLimite,
            Color = request.Color,
            Icono = request.Icono,
            isPlantilla = request.isPlantilla,
            isDeleted = request.isDeleted,
            LastModified = DateTime.UtcNow
        };

        if (await _proyectoService.Modificar(proyecto))
        {
            var response = new ProyectosResponse
            {
                ProyectoId = proyecto.ProyectoId,
                UsuarioId = proyecto.UsuarioId,
                Nombre = proyecto.Nombre,
                Descripcion = proyecto.Descripcion,
                FechaCreacion = proyecto.FechaCreacion,
                FechaLimite = proyecto.FechaLimite,
                Color = proyecto.Color,
                Icono = proyecto.Icono,
                isPlantilla = proyecto.isPlantilla,
                isDeleted = proyecto.isDeleted,
                LastModified = proyecto.LastModified
            };

            return Ok(response);
        }

        return BadRequest(new { message = "No se pudo actualizar el proyecto" });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProyecto(Guid id)
    {
        if (!await _proyectoService.Existe(id))
        {
            return NotFound($"Proyecto con ID {id} no encontrado");
        }

        if (await _proyectoService.deleteProyecto(id))
        {
            return NoContent();
        }

        return BadRequest(new { message = "No se pudo eliminar el proyecto" });
    }
}