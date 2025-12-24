using Microsoft.EntityFrameworkCore;
using Projectly.Shared.Dal;
using Projectly.Shared.Models;

namespace Projectly.Shared.Services;

public class NotaService(IDbContextFactory<ProjectlyDbContext> DbContext)
{
    public async Task<bool> Existe(Guid NotaId)
    {
        await using var contexto = await DbContext.CreateDbContextAsync();
        return await contexto.Notas.AnyAsync(n => n.NotaId == NotaId);
    }

    public async Task<bool> Insertar(Notas nota)
    {
        await using var contexto = await DbContext.CreateDbContextAsync();

        contexto.Notas.Add(nota);
        return await contexto.SaveChangesAsync() > 0;
    }

    public async Task<bool> Modificar(Notas nota)
    {
        await using var contexto = await DbContext.CreateDbContextAsync();
        contexto.Notas.Update(nota);
        return await contexto.SaveChangesAsync() > 0;
    }

    public async Task<bool> Guardar(Notas nota)
    {
        if (await Existe(nota.NotaId))
        {
            return await Modificar(nota);
        }
        else
        {
            return await Insertar(nota);
        }
    }

    public async Task<Notas?> getNotaById(Guid NotaId)
    {
        await using var contexto = await DbContext.CreateDbContextAsync();
        return await contexto.Notas.FirstOrDefaultAsync(n => n.NotaId == NotaId);
    }

    public async Task<List<Notas>> getNotasByProyectoId(Guid ProyectoId)
    {
        await using var contexto = await DbContext.CreateDbContextAsync();
        return await contexto.Notas.Where(n => n.ProyectoId == ProyectoId && !n.isDeleted).ToListAsync();
    }

    public async Task<bool> deleteNota(Guid NotaId)
    {
        await using var contexto = await DbContext.CreateDbContextAsync();
        var nota = await contexto.Notas.FirstOrDefaultAsync(n => n.NotaId == NotaId);
        if (nota == null)
        {
            return false;
        }
        nota.isDeleted = true;
        contexto.Notas.Update(nota);
        return await contexto.SaveChangesAsync() > 0;
    }
}