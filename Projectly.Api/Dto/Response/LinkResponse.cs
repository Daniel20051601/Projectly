namespace Projectly.Api.Dto.Response;

public class LinkResponse
{
    public Guid LinkId { get; set; }
    public Guid TareaId { get; set; }
    public string UrlLink { get; set; } = string.Empty;
    public bool isDeleted { get; set; } = false;
    public DateTime LastModified { get; set; }
}
