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
public class NotasController : ControllerBase
{
    private readonly NotaService _notaService;

    public NotasController(NotaService notaService)
    {
        _notaService = notaService;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<NotasResponse>> GetNota(Guid id)
    {
        var nota = await _notaService.getNotaById(id);
        if (nota == null)
        {
            return NotFound($"Nota con ID {id} no encontrada");
        }

        var response = new NotasResponse
        {
            NotaId = nota.NotaId,
            ProyectoId = nota.ProyectoId,
            Titulo = nota.Titulo,
            Descripcion = nota.Descripcion,
            isDeleted = nota.isDeleted,
            LastModified = nota.LastModified
        };

        return Ok(response);
    }

    [HttpGet("proyecto/{proyectoId}")]
    public async Task<ActionResult<List<NotasResponse>>> GetNotasByProyecto(Guid proyectoId)
    {
        var notas = await _notaService.getNotasByProyectoId(proyectoId);
        var response = notas.Select(n => new NotasResponse
        {
            NotaId = n.NotaId,
            ProyectoId = n.ProyectoId,
            Titulo = n.Titulo,
            Descripcion = n.Descripcion,
            isDeleted = n.isDeleted,
            LastModified = n.LastModified
        }).ToList();

        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<NotasResponse>> CreateNota([FromBody] NotasRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var nota = new Notas
        {
            NotaId = Guid.NewGuid(),
            ProyectoId = request.ProyectoId,
            Titulo = request.Titulo,
            Descripcion = request.Descripcion,
            isDeleted = false,
            LastModified = DateTime.UtcNow
        };

        if (await _notaService.Insertar(nota))
        {
            var response = new NotasResponse
            {
                NotaId = nota.NotaId,
                ProyectoId = nota.ProyectoId,
                Titulo = nota.Titulo,
                Descripcion = nota.Descripcion,
                isDeleted = nota.isDeleted,
                LastModified = nota.LastModified
            };

            return CreatedAtAction(nameof(GetNota), new { id = nota.NotaId }, response);
        }

        return BadRequest(new { message = "No se pudo crear la nota" });
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<NotasResponse>> UpdateNota(Guid id, [FromBody] NotasRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (!await _notaService.Existe(id))
        {
            return NotFound($"Nota con ID {id} no encontrada");
        }

        var nota = new Notas
        {
            NotaId = id,
            ProyectoId = request.ProyectoId,
            Titulo = request.Titulo,
            Descripcion = request.Descripcion,
            isDeleted = request.isDeleted,
            LastModified = DateTime.UtcNow
        };

        if (await _notaService.Modificar(nota))
        {
            var response = new NotasResponse
            {
                NotaId = nota.NotaId,
                ProyectoId = nota.ProyectoId,
                Titulo = nota.Titulo,
                Descripcion = nota.Descripcion,
                isDeleted = nota.isDeleted,
                LastModified = nota.LastModified
            };

            return Ok(response);
        }

        return BadRequest(new { message = "No se pudo actualizar la nota" });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteNota(Guid id)
    {
        if (!await _notaService.Existe(id))
        {
            return NotFound($"Nota con ID {id} no encontrada");
        }

        if (await _notaService.deleteNota(id))
        {
            return NoContent();
        }

        return BadRequest(new { message = "No se pudo eliminar la nota" });
    }
}