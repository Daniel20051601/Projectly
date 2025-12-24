using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Projectly.Shared.Models
{
    public class Notas
    {
        [Key]
        public Guid NotaId { get; set; } = Guid.NewGuid();

        [ForeignKey("Proyecto")]
        public Guid ProyectoId { get; set; }

        [Required]
        public string Titulo { get; set; } = string.Empty;

        public string Descripcion { get; set; } = string.Empty;

        public bool isDeleted { get; set; } = false;

        public DateTime LastModified { get; set; } = DateTime.UtcNow;

        public virtual Proyectos Proyecto { get; set; }
    }
}