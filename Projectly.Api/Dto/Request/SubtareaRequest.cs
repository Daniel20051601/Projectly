namespace Projectly.Api.Dto.Request;

public class SubtareaRequest
{
    public Guid TareaId { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public bool Completada { get; set; } = false;
    public bool isDeleted { get; set; } = false;
    public DateTime LastModified { get; set; } = DateTime.UtcNow;

}
