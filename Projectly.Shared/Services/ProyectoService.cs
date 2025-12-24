using Microsoft.EntityFrameworkCore;
using Projectly.Shared.Dal;
using Projectly.Shared.Models;

namespace Projectly.Shared.Services;

public class ProyectoService(IDbContextFactory<ProjectlyDbContext> DbContext)
{
    public async Task<bool> Existe(Guid ProyectoId)
    {
        await using var contexto = await DbContext.CreateDbContextAsync();
        return await contexto.Proyectos.AnyAsync(p => p.ProyectoId == ProyectoId);
    }

    public async Task<bool> Insertar(Proyectos proyecto)
    {
        await using var contexto = await DbContext.CreateDbContextAsync();

        contexto.Proyectos.Add(proyecto);
        return await contexto.SaveChangesAsync() > 0;
    }

    public async Task<bool> Modificar(Proyectos proyecto)
    {
        await using var contexto = await DbContext.CreateDbContextAsync();
        contexto.Proyectos.Update(proyecto);
        return await contexto.SaveChangesAsync() > 0;
    }

    public async Task<bool> Guardar(Proyectos proyecto)
    {
        if(await Existe(proyecto.ProyectoId))
        {
            return await Modificar(proyecto);
        }
        else
        {
            return await Insertar(proyecto);
        }
    }

    public async Task<Proyectos?> getProyectoById(Guid ProyectoId)
    {
        await using var contexto = await DbContext.CreateDbContextAsync();
        return await contexto.Proyectos.FirstOrDefaultAsync(p => p.ProyectoId == ProyectoId);
    }

    public async Task<List<Proyectos>> getProyectosByUsuarioId(string UsuarioId)
    {
        await using var contexto = await DbContext.CreateDbContextAsync();
        return await contexto.Proyectos.Where(p => p.UsuarioId == UsuarioId && !p.isDeleted).ToListAsync();
    }

    public async Task<bool> deleteProyecto(Guid ProyectoId)
    {
        await using var contexto = await DbContext.CreateDbContextAsync();
        var proyecto = await contexto.Proyectos.FirstOrDefaultAsync(p => p.ProyectoId == ProyectoId);
        if (proyecto == null)
        {
            return false;
        }
        proyecto.isDeleted = true;
        contexto.Proyectos.Update(proyecto);
        return await contexto.SaveChangesAsync() > 0;
    }
}
