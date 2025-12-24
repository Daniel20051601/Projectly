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
public class LinksController : ControllerBase
{
    private readonly LinkService _linkService;

    public LinksController(LinkService linkService)
    {
        _linkService = linkService;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<LinkResponse>> GetLink(Guid id)
    {
        var link = await _linkService.getLinkById(id);
        if (link == null)
        {
            return NotFound($"Link con ID {id} no encontrado");
        }

        var response = new LinkResponse
        {
            LinkId = link.LinkId,
            TareaId = link.TareaId,
            UrlLink = link.UrlLink,
            isDeleted = link.isDeleted,
            LastModified = link.LastModified
        };

        return Ok(response);
    }

    [HttpGet("tarea/{tareaId}")]
    public async Task<ActionResult<List<LinkResponse>>> GetLinksByTarea(Guid tareaId)
    {
        var links = await _linkService.getLinksByTareaId(tareaId);
        var response = links.Select(l => new LinkResponse
        {
            LinkId = l.LinkId,
            TareaId = l.TareaId,
            UrlLink = l.UrlLink,
            isDeleted = l.isDeleted,
            LastModified = l.LastModified
        }).ToList();

        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<LinkResponse>> CreateLink([FromBody] LinksRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var link = new Links
        {
            LinkId = Guid.NewGuid(),
            TareaId = request.TareaId,
            UrlLink = request.UrlLink,
            isDeleted = false,
            LastModified = DateTime.UtcNow
        };

        if (await _linkService.Insertar(link))
        {
            var response = new LinkResponse
            {
                LinkId = link.LinkId,
                TareaId = link.TareaId,
                UrlLink = link.UrlLink,
                isDeleted = link.isDeleted,
                LastModified = link.LastModified
            };

            return CreatedAtAction(nameof(GetLink), new { id = link.LinkId }, response);
        }

        return BadRequest(new { message = "No se pudo crear el link" });
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<LinkResponse>> UpdateLink(Guid id, [FromBody] LinksRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (!await _linkService.Existe(id))
        {
            return NotFound($"Link con ID {id} no encontrado");
        }

        var link = new Links
        {
            LinkId = id,
            TareaId = request.TareaId,
            UrlLink = request.UrlLink,
            isDeleted = request.isDeleted,
            LastModified = DateTime.UtcNow
        };

        if (await _linkService.Modificar(link))
        {
            var response = new LinkResponse
            {
                LinkId = link.LinkId,
                TareaId = link.TareaId,
                UrlLink = link.UrlLink,
                isDeleted = link.isDeleted,
                LastModified = link.LastModified
            };

            return Ok(response);
        }

        return BadRequest(new { message = "No se pudo actualizar el link" });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteLink(Guid id)
    {
        if (!await _linkService.Existe(id))
        {
            return NotFound($"Link con ID {id} no encontrado");
        }

        if (await _linkService.deleteLink(id))
        {
            return NoContent();
        }

        return BadRequest(new { message = "No se pudo eliminar el link" });
    }
}