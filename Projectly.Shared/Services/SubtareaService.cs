using Microsoft.EntityFrameworkCore;
using Projectly.Shared.Dal;
using Projectly.Shared.Models;

namespace Projectly.Shared.Services;

public class SubtareaService(IDbContextFactory<ProjectlyDbContext> DbContext)
{
    public async Task<bool> Existe(Guid SubtareaId)
    {
        await using var contexto = await DbContext.CreateDbContextAsync();
        return await contexto.Subtareas.AnyAsync(s => s.SubtareaId == SubtareaId);
    }

    public async Task<bool> Insertar(Subtareas subtarea)
    {
        await using var contexto = await DbContext.CreateDbContextAsync();

        contexto.Subtareas.Add(subtarea);
        return await contexto.SaveChangesAsync() > 0;
    }

    public async Task<bool> Modificar(Subtareas subtarea)
    {
        await using var contexto = await DbContext.CreateDbContextAsync();
        contexto.Subtareas.Update(subtarea);
        return await contexto.SaveChangesAsync() > 0;
    }

    public async Task<bool> Guardar(Subtareas subtarea)
    {
        if (await Existe(subtarea.SubtareaId))
        {
            return await Modificar(subtarea);
        }
        else
        {
            return await Insertar(subtarea);
        }
    }

    public async Task<Subtareas?> getSubtareaById(Guid SubtareaId)
    {
        await using var contexto = await DbContext.CreateDbContextAsync();
        return await contexto.Subtareas.FirstOrDefaultAsync(s => s.SubtareaId == SubtareaId);
    }

    public async Task<List<Subtareas>> getSubtareasByTareaId(Guid TareaId)
    {
        await using var contexto = await DbContext.CreateDbContextAsync();
        return await contexto.Subtareas.Where(s => s.TareaId == TareaId && !s.isDeleted).ToListAsync();
    }

    public async Task<bool> marcarComoCompletada(Guid SubtareaId, bool completada)
    {
        await using var contexto = await DbContext.CreateDbContextAsync();
        var subtarea = await contexto.Subtareas.FirstOrDefaultAsync(s => s.SubtareaId == SubtareaId);
        if (subtarea == null)
        {
            return false;
        }
        subtarea.Completada = completada;
        contexto.Subtareas.Update(subtarea);
        return await contexto.SaveChangesAsync() > 0;
    }

    public async Task<bool> deleteSubtarea(Guid SubtareaId)
    {
        await using var contexto = await DbContext.CreateDbContextAsync();
        var subtarea = await contexto.Subtareas.FirstOrDefaultAsync(s => s.SubtareaId == SubtareaId);
        if (subtarea == null)
        {
            return false;
        }
        subtarea.isDeleted = true;
        contexto.Subtareas.Update(subtarea);
        return await contexto.SaveChangesAsync() > 0;
    }
}