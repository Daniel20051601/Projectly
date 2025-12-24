using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Projectly.Shared.Models
{
    public class Tareas
    {
        [Key]
        public Guid TareaId { get; set; } = Guid.NewGuid();

        [ForeignKey("Proyecto")]
        public Guid ProyectoId { get; set; }

        [Required]
        public string Titulo { get; set; } = string.Empty;

        public DateTime FechaLimite { get; set; }

        public string Estado { get; set; } = string.Empty;

        public string Prioridad { get; set; } = string.Empty;

        public bool isDeleted { get; set; } = false;

        public DateTime LastModified { get; set; } = DateTime.UtcNow;

        public virtual Proyectos Proyecto { get; set; }
        public virtual ICollection<Subtareas> Subtareas { get; set; } = new List<Subtareas>();
        public virtual ICollection<Links> Links { get; set; } = new List<Links>();
    }
}