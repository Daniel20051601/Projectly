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
public class SubtareasController : ControllerBase
{
    private readonly SubtareaService _subtareaService;

    public SubtareasController(SubtareaService subtareaService)
    {
        _subtareaService = subtareaService;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SubtareaResponse>> GetSubtarea(Guid id)
    {
        var subtarea = await _subtareaService.getSubtareaById(id);
        if (subtarea == null)
        {
            return NotFound($"Subtarea con ID {id} no encontrada");
        }

        var response = new SubtareaResponse
        {
            SubtareaId = subtarea.SubtareaId,
            TareaId = subtarea.TareaId,
            Titulo = subtarea.Titulo,
            Completada = subtarea.Completada,
            isDeleted = subtarea.isDeleted,
            LastModified = subtarea.LastModified
        };

        return Ok(response);
    }

    [HttpGet("tarea/{tareaId}")]
    public async Task<ActionResult<List<SubtareaResponse>>> GetSubtareasByTarea(Guid tareaId)
    {
        var subtareas = await _subtareaService.getSubtareasByTareaId(tareaId);
        var response = subtareas.Select(s => new SubtareaResponse
        {
            SubtareaId = s.SubtareaId,
            TareaId = s.TareaId,
            Titulo = s.Titulo,
            Completada = s.Completada,
            isDeleted = s.isDeleted,
            LastModified = s.LastModified
        }).ToList();

        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<SubtareaResponse>> CreateSubtarea([FromBody] SubtareaRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var subtarea = new Subtareas
        {
            SubtareaId = Guid.NewGuid(),
            TareaId = request.TareaId,
            Titulo = request.Titulo,
            Completada = false,
            isDeleted = false,
            LastModified = DateTime.UtcNow
        };

        if (await _subtareaService.Insertar(subtarea))
        {
            var response = new SubtareaResponse
            {
                SubtareaId = subtarea.SubtareaId,
                TareaId = subtarea.TareaId,
                Titulo = subtarea.Titulo,
                Completada = subtarea.Completada,
                isDeleted = subtarea.isDeleted,
                LastModified = subtarea.LastModified
            };

            return CreatedAtAction(nameof(GetSubtarea), new { id = subtarea.SubtareaId }, response);
        }

        return BadRequest(new { message = "No se pudo crear la subtarea" });
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<SubtareaResponse>> UpdateSubtarea(Guid id, [FromBody] SubtareaRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (!await _subtareaService.Existe(id))
        {
            return NotFound($"Subtarea con ID {id} no encontrada");
        }

        var subtarea = new Subtareas
        {
            SubtareaId = id,
            TareaId = request.TareaId,
            Titulo = request.Titulo,
            Completada = request.Completada,
            isDeleted = request.isDeleted,
            LastModified = DateTime.UtcNow
        };

        if (await _subtareaService.Modificar(subtarea))
        {
            var response = new SubtareaResponse
            {
                SubtareaId = subtarea.SubtareaId,
                TareaId = subtarea.TareaId,
                Titulo = subtarea.Titulo,
                Completada = subtarea.Completada,
                isDeleted = subtarea.isDeleted,
                LastModified = subtarea.LastModified
            };

            return Ok(response);
        }

        return BadRequest(new { message = "No se pudo actualizar la subtarea" });
    }

    [HttpPatch("{id}/completada")]
    public async Task<ActionResult<SubtareaResponse>> MarcarComoCompletada(Guid id, [FromBody] bool completada)
    {
        if (!await _subtareaService.Existe(id))
        {
            return NotFound($"Subtarea con ID {id} no encontrada");
        }

        if (await _subtareaService.marcarComoCompletada(id, completada))
        {
            var subtareaActualizada = await _subtareaService.getSubtareaById(id);
            var response = new SubtareaResponse
            {
                SubtareaId = subtareaActualizada!.SubtareaId,
                TareaId = subtareaActualizada.TareaId,
                Titulo = subtareaActualizada.Titulo,
                Completada = subtareaActualizada.Completada,
                isDeleted = subtareaActualizada.isDeleted,
                LastModified = subtareaActualizada.LastModified
            };

            return Ok(response);
        }

        return BadRequest(new { message = "No se pudo marcar como completada" });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSubtarea(Guid id)
    {
        if (!await _subtareaService.Existe(id))
        {
            return NotFound($"Subtarea con ID {id} no encontrada");
        }

        if (await _subtareaService.deleteSubtarea(id))
        {
            return NoContent();
        }

        return BadRequest(new { message = "No se pudo eliminar la subtarea" });
    }
}