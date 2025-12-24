using Microsoft.EntityFrameworkCore;
using Projectly.Shared.Dal;
using Projectly.Shared.Models;

namespace Projectly.Shared.Services;

public class LinkService(IDbContextFactory<ProjectlyDbContext> DbContext)
{
    public async Task<bool> Existe(Guid LinkId)
    {
        await using var contexto = await DbContext.CreateDbContextAsync();
        return await contexto.Links.AnyAsync(l => l.LinkId == LinkId);
    }

    public async Task<bool> Insertar(Links link)
    {
        await using var contexto = await DbContext.CreateDbContextAsync();

        contexto.Links.Add(link);
        return await contexto.SaveChangesAsync() > 0;
    }

    public async Task<bool> Modificar(Links link)
    {
        await using var contexto = await DbContext.CreateDbContextAsync();
        contexto.Links.Update(link);
        return await contexto.SaveChangesAsync() > 0;
    }

    public async Task<bool> Guardar(Links link)
    {
        if (await Existe(link.LinkId))
        {
            return await Modificar(link);
        }
        else
        {
            return await Insertar(link);
        }
    }

    public async Task<Links?> getLinkById(Guid LinkId)
    {
        await using var contexto = await DbContext.CreateDbContextAsync();
        return await contexto.Links.FirstOrDefaultAsync(l => l.LinkId == LinkId);
    }

    public async Task<List<Links>> getLinksByTareaId(Guid TareaId)
    {
        await using var contexto = await DbContext.CreateDbContextAsync();
        return await contexto.Links.Where(l => l.TareaId == TareaId && !l.isDeleted).ToListAsync();
    }

    public async Task<bool> deleteLink(Guid LinkId)
    {
        await using var contexto = await DbContext.CreateDbContextAsync();
        var link = await contexto.Links.FirstOrDefaultAsync(l => l.LinkId == LinkId);
        if (link == null)
        {
            return false;
        }
        link.isDeleted = true;
        contexto.Links.Update(link);
        return await contexto.SaveChangesAsync() > 0;
    }
}