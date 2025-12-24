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
public class TareasController : ControllerBase
{
    private readonly TareaService _tareaService;

    public TareasController(TareaService tareaService)
    {
        _tareaService = tareaService;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TareasResponse>> GetTarea(Guid id)
    {
        var tarea = await _tareaService.getTareaById(id);
        if (tarea == null)
        {
            return NotFound($"Tarea con ID {id} no encontrada");
        }

        var response = new TareasResponse
        {
            TareaId = tarea.TareaId,
            ProyectoId = tarea.ProyectoId,
            Titulo = tarea.Titulo,
            FechaLimite = tarea.FechaLimite,
            Estado = tarea.Estado,
            Prioridad = tarea.Prioridad,
            isDeleted = tarea.isDeleted,
            LastModified = tarea.LastModified
        };

        return Ok(response);
    }

    [HttpGet("proyecto/{proyectoId}")]
    public async Task<ActionResult<List<TareasResponse>>> GetTareasByProyecto(Guid proyectoId)
    {
        var tareas = await _tareaService.getTareasByProyectoId(proyectoId);
        var response = tareas.Select(t => new TareasResponse
        {
            TareaId = t.TareaId,
            ProyectoId = t.ProyectoId,
            Titulo = t.Titulo,
            FechaLimite = t.FechaLimite,
            Estado = t.Estado,
            Prioridad = t.Prioridad,
            isDeleted = t.isDeleted,
            LastModified = t.LastModified
        }).ToList();

        return Ok(response);
    }

    [HttpGet("estado/{estado}")]
    public async Task<ActionResult<List<TareasResponse>>> GetTareasByEstado(string estado)
    {
        var tareas = await _tareaService.getTareasByEstado(estado);
        var response = tareas.Select(t => new TareasResponse
        {
            TareaId = t.TareaId,
            ProyectoId = t.ProyectoId,
            Titulo = t.Titulo,
            FechaLimite = t.FechaLimite,
            Estado = t.Estado,
            Prioridad = t.Prioridad,
            isDeleted = t.isDeleted,
            LastModified = t.LastModified
        }).ToList();

        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<TareasResponse>> CreateTarea([FromBody] TareasRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var tarea = new Tareas
        {
            TareaId = Guid.NewGuid(),
            ProyectoId = request.ProyectoId,
            Titulo = request.Titulo,
            FechaLimite = request.FechaLimite,
            Estado = request.Estado,
            Prioridad = request.Prioridad.ToString(),
            isDeleted = false,
            LastModified = DateTime.UtcNow
        };

        if (await _tareaService.Insertar(tarea))
        {
            var response = new TareasResponse
            {
                TareaId = tarea.TareaId,
                ProyectoId = tarea.ProyectoId,
                Titulo = tarea.Titulo,
                FechaLimite = tarea.FechaLimite,
                Estado = tarea.Estado,
                Prioridad = tarea.Prioridad,
                isDeleted = tarea.isDeleted,
                LastModified = tarea.LastModified
            };

            return CreatedAtAction(nameof(GetTarea), new { id = tarea.TareaId }, response);
        }

        return BadRequest(new { message = "No se pudo crear la tarea" });
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<TareasResponse>> UpdateTarea(Guid id, [FromBody] TareasRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (!await _tareaService.Existe(id))
        {
            return NotFound($"Tarea con ID {id} no encontrada");
        }

        var tarea = new Tareas
        {
            TareaId = id,
            ProyectoId = request.ProyectoId,
            Titulo = request.Titulo,
            FechaLimite = request.FechaLimite,
            Estado = request.Estado,
            Prioridad = request.Prioridad.ToString(),
            isDeleted = request.isDeleted,
            LastModified = DateTime.UtcNow
        };

        if (await _tareaService.Modificar(tarea))
        {
            var response = new TareasResponse
            {
                TareaId = tarea.TareaId,
                ProyectoId = tarea.ProyectoId,
                Titulo = tarea.Titulo,
                FechaLimite = tarea.FechaLimite,
                Estado = tarea.Estado,
                Prioridad = tarea.Prioridad,
                isDeleted = tarea.isDeleted,
                LastModified = tarea.LastModified
            };

            return Ok(response);
        }

        return BadRequest(new { message = "No se pudo actualizar la tarea" });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTarea(Guid id)
    {
        if (!await _tareaService.Existe(id))
        {
            return NotFound($"Tarea con ID {id} no encontrada");
        }

        if (await _tareaService.deleteTarea(id))
        {
            return NoContent();
        }

        return BadRequest(new { message = "No se pudo eliminar la tarea" });
    }
}