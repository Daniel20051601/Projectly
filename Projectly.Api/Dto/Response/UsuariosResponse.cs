namespace Projectly.Api.Dto.Response;

public class UsuariosResponse
{
    public string UsuarioId { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool isPremium { get; set; } = false;
    public DateTime FechaCreacion { get; set; }
    public DateTime LastModified { get; set; }

}
