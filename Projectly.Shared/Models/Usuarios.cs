

using System.ComponentModel.DataAnnotations;

namespace Projectly.Shared.Models;

public class Usuarios
{
    [Key]
    public string UsuarioId { get; set; }
    [Required(ErrorMessage = "El nombre es obligatorio")]
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    public bool isPremium { get; set; } = false;
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    public DateTime LastModified { get; set; } = DateTime.UtcNow;
    public virtual ICollection<Proyectos> Proyectos { get; set; } = new List<Proyectos>();
}
