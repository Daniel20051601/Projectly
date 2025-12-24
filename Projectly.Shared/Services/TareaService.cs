using Microsoft.EntityFrameworkCore;
using Projectly.Shared.Dal;
using Projectly.Shared.Models;

namespace Projectly.Shared.Services;

public class TareaService(IDbContextFactory<ProjectlyDbContext> DbContext)
{
    public async Task<bool> Existe(Guid TareaId)
    {
        await using var contexto = await DbContext.CreateDbContextAsync();
        return await contexto.Tareas.AnyAsync(t => t.TareaId == TareaId);
    }

    public async Task<bool> Insertar(Tareas tarea)
    {
        await using var contexto = await DbContext.CreateDbContextAsync();

        contexto.Tareas.Add(tarea);
        return await contexto.SaveChangesAsync() > 0;
    }

    public async Task<bool> Modificar(Tareas tarea)
    {
        await using var contexto = await DbContext.CreateDbContextAsync();
        contexto.Tareas.Update(tarea);
        return await contexto.SaveChangesAsync() > 0;
    }

    public async Task<bool> Guardar(Tareas tarea)
    {
        if (await Existe(tarea.TareaId))
        {
            return await Modificar(tarea);
        }
        else
        {
            return await Insertar(tarea);
        }
    }

    public async Task<Tareas?> getTareaById(Guid TareaId)
    {
        await using var contexto = await DbContext.CreateDbContextAsync();
        return await contexto.Tareas
            .Include(t => t.Subtareas)
            .Include(t => t.Links)
            .FirstOrDefaultAsync(t => t.TareaId == TareaId);
    }

    public async Task<List<Tareas>> getTareasByProyectoId(Guid ProyectoId)
    {
        await using var contexto = await DbContext.CreateDbContextAsync();
        return await contexto.Tareas
            .Include(t => t.Subtareas)
            .Include(t => t.Links)
            .Where(t => t.ProyectoId == ProyectoId && !t.isDeleted)
            .ToListAsync();
    }

    public async Task<List<Tareas>> getTareasByEstado(string Estado)
    {
        await using var contexto = await DbContext.CreateDbContextAsync();
        return await contexto.Tareas
            .Where(t => t.Estado == Estado && !t.isDeleted)
            .ToListAsync();
    }

    public async Task<bool> deleteTarea(Guid TareaId)
    {
        await using var contexto = await DbContext.CreateDbContextAsync();
        var tarea = await contexto.Tareas.FirstOrDefaultAsync(t => t.TareaId == TareaId);
        if (tarea == null)
        {
            return false;
        }
        tarea.isDeleted = true;
        contexto.Tareas.Update(tarea);
        return await contexto.SaveChangesAsync() > 0;
    }
}