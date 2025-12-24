using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Projectly.Shared.Models
{
    public class Subtareas
    {
        [Key]
        public Guid SubtareaId { get; set; } = Guid.NewGuid();

        [ForeignKey("Tarea")]
        public Guid TareaId { get; set; }

        [Required]
        public string Titulo { get; set; } = string.Empty;

        public bool Completada { get; set; } = false;

        public bool isDeleted { get; set; } = false;

        public DateTime LastModified { get; set; } = DateTime.UtcNow;

        public virtual Tareas Tarea { get; set; }
    }
}   