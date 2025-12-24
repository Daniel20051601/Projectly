using Microsoft.Extensions.DependencyInjection;

namespace Projectly.Shared.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddProjectlyServices(this IServiceCollection services)
    {
        services.AddScoped<UsuarioService>();
        services.AddScoped<ProyectoService>();
        services.AddScoped<TareaService>();
        services.AddScoped<SubtareaService>();
        services.AddScoped<LinkService>();
        services.AddScoped<NotaService>();
        
        return services;
    }
}
