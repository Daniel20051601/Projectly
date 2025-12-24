namespace Projectly.Api.Dto.Response;

public class ProyectosResponse
{
    public Guid ProyectoId { get; set; }
    public string UsuarioId { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public DateTime FechaCreacion { get; set; }
    public DateTime FechaLimite { get; set; }
    public string Color { get; set; } = string.Empty;
    public string Icono { get; set; } = string.Empty;
    public bool isPlantilla { get; set; } = false;
    public bool isDeleted { get; set; } = false;
    public DateTime LastModified { get; set; }
}
