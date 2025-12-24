using Microsoft.EntityFrameworkCore;
using Projectly.Shared.Dal;
using Projectly.Shared.Models;

namespace Projectly.Shared.Services;

public class UsuarioService(IDbContextFactory<ProjectlyDbContext> DbContext)
{
    public async Task<bool> Existe(string UsuarioId)
    {
        await using var contexto = await DbContext.CreateDbContextAsync();
        return await contexto.Usuarios.AnyAsync(u => u.UsuarioId == UsuarioId);
    }

    public async Task<bool> ExisteEmail(string Email)
    {
        await using var contexto = await DbContext.CreateDbContextAsync();
        return await contexto.Usuarios.AnyAsync(u => u.Email == Email);
    }

    public async Task<bool> Insertar(Usuarios usuario)
    {
        await using var contexto = await DbContext.CreateDbContextAsync();

        contexto.Usuarios.Add(usuario);
        return await contexto.SaveChangesAsync() > 0;
    }

    public async Task<bool> Modificar(Usuarios usuario)
    {
        await using var contexto = await DbContext.CreateDbContextAsync();
        contexto.Usuarios.Update(usuario);
        return await contexto.SaveChangesAsync() > 0;
    }

    public async Task<bool> Guardar(Usuarios usuario)
    {
        if (await Existe(usuario.UsuarioId))
        {
            return await Modificar(usuario);
        }
        else
        {
            return await Insertar(usuario);
        }
    }

    public async Task<Usuarios?> getUsuarioById(string UsuarioId)
    {
        await using var contexto = await DbContext.CreateDbContextAsync();
        return await contexto.Usuarios
            .FirstOrDefaultAsync(u => u.UsuarioId == UsuarioId);
    }

    public async Task<Usuarios?> getUsuarioByEmail(string Email)
    {
        await using var contexto = await DbContext.CreateDbContextAsync();
        return await contexto.Usuarios.FirstOrDefaultAsync(u => u.Email == Email);
    }

    public async Task<bool> actualizarPremium(string UsuarioId, bool isPremium)
    {
        await using var contexto = await DbContext.CreateDbContextAsync();
        var usuario = await contexto.Usuarios.FirstOrDefaultAsync(u => u.UsuarioId == UsuarioId);
        if (usuario == null)
        {
            return false;
        }
        usuario.isPremium = isPremium;
        contexto.Usuarios.Update(usuario);
        return await contexto.SaveChangesAsync() > 0;
    }
}