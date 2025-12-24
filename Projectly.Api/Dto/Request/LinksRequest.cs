namespace Projectly.Api.Dto.Request;

public class LinksRequest
{
    public Guid TareaId { get; set; }
    public string UrlLink { get; set; } = string.Empty;
    public bool isDeleted { get; set; } = false;
    public DateTime LastModified { get; set; } = DateTime.UtcNow;
}
