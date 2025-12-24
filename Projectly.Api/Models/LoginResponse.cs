namespace Projectly.Api.Models;

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public string UsuarioId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public bool IsPremium { get; set; }
    public DateTime ExpiresAt { get; set; }
}