using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Projectly.Shared.Models
{
    public class Links
    {
        [Key]
        public Guid LinkId { get; set; } = Guid.NewGuid();

        [ForeignKey("Tarea")]
        public Guid TareaId { get; set; }

        [Required]
        public string UrlLink { get; set; } = string.Empty;

        public bool isDeleted { get; set; } = false;

        public DateTime LastModified { get; set; } = DateTime.UtcNow;

        public virtual Tareas Tarea { get; set; }
    }
}