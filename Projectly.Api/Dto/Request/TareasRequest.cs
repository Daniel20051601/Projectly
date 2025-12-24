namespace Projectly.Api.Dto.Request;

public class TareasRequest
{
    public Guid ProyectoId { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public DateTime FechaLimite { get; set; }
    public string Estado { get; set; } = string.Empty;
    public string Prioridad { get; set; } = string.Empty;
    public bool isDeleted { get; set; } = false;
    public DateTime LastModified { get; set; } = DateTime.UtcNow;
}
