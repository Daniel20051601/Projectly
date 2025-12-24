namespace Projectly.Api.Dto.Response;

public class NotasResponse
{
    public Guid NotaId { get; set; }
    public Guid ProyectoId { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public bool isDeleted { get; set; } = false;
    public DateTime LastModified { get; set; }
}
