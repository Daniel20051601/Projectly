namespace Projectly.Api.Dto.Response;

public class SubtareaResponse
{
    public Guid SubtareaId { get; set; }
    public Guid TareaId { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public bool Completada { get; set; } = false;
    public bool isDeleted { get; set; } = false;
    public DateTime LastModified { get; set; }
}
