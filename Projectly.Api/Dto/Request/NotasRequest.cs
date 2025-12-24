namespace Projectly.Api.Dto.Request;

public class NotasRequest
{
    public Guid ProyectoId { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public bool isDeleted { get; set; } = false;
    public DateTime LastModified { get; set; } = DateTime.UtcNow;
}
