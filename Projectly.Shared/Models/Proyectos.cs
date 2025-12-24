using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading;

namespace Projectly.Shared.Models
{
    public class Proyectos
    {
        [Key]
        public Guid ProyectoId { get; set; } = Guid.NewGuid();

        [ForeignKey("Usuario")]
        public string UsuarioId { get; set; } = string.Empty;

        [Required]
        public string Nombre { get; set; } = string.Empty;

        public string Descripcion { get; set; } = string.Empty;

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        public DateTime FechaLimite { get; set; }

        public string Color { get; set; } = string.Empty;

        public string Icono { get; set; } = string.Empty;

        public bool isPlantilla { get; set; } = false;

        public bool isDeleted { get; set; } = false;

        public DateTime LastModified { get; set; } = DateTime.UtcNow;

        public virtual Usuarios Usuario { get; set; }
        public virtual ICollection<Tareas> Tareas { get; set; } = new List<Tareas>();
        public virtual ICollection<Notas> Notas { get; set; } = new List<Notas>();
    }
}