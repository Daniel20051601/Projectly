using Microsoft.EntityFrameworkCore;
using Projectly.Shared.Models;

namespace Projectly.Shared.Dal;

public class ProjectlyDbContext : DbContext
{
    public ProjectlyDbContext(DbContextOptions<ProjectlyDbContext> options) : base(options)
    {
    }

    public DbSet<Usuarios> Usuarios { get; set; }
    public DbSet<Proyectos> Proyectos { get; set; }
    public DbSet<Tareas> Tareas { get; set; }
    public DbSet<Subtareas> Subtareas { get; set; }
    public DbSet<Links> Links { get; set; }
    public DbSet<Notas> Notas { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Proyectos>()
            .HasOne(p => p.Usuario)
            .WithMany(u => u.Proyectos)
            .HasForeignKey(p => p.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Tareas>()
            .HasOne(t => t.Proyecto)
            .WithMany(p => p.Tareas)
            .HasForeignKey(t => t.ProyectoId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Subtareas>()
            .HasOne(s => s.Tarea)
            .WithMany(t => t.Subtareas)
            .HasForeignKey(s => s.TareaId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Links>()
            .HasOne(l => l.Tarea)
            .WithMany(t => t.Links)
            .HasForeignKey(l => l.TareaId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Notas>()
            .HasOne(n => n.Proyecto)
            .WithMany(p => p.Notas)
            .HasForeignKey(n => n.ProyectoId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Usuarios>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<Proyectos>()
            .HasIndex(p => p.UsuarioId);

        modelBuilder.Entity<Tareas>()
            .HasIndex(t => t.ProyectoId);

        modelBuilder.Entity<Subtareas>()
            .HasIndex(s => s.TareaId);

        modelBuilder.Entity<Usuarios>()
            .Property(u => u.UsuarioId)
            .HasMaxLength(450);

        modelBuilder.Entity<Usuarios>()
            .Property(u => u.Email)
            .HasMaxLength(256);

        modelBuilder.Entity<Proyectos>()
            .Property(p => p.Nombre)
            .HasMaxLength(200);

        modelBuilder.Entity<Tareas>()
            .Property(t => t.Titulo)
            .HasMaxLength(300);

        modelBuilder.Entity<Proyectos>()
            .HasQueryFilter(p => !p.isDeleted);

        modelBuilder.Entity<Tareas>()
            .HasQueryFilter(t => !t.isDeleted);

        modelBuilder.Entity<Subtareas>()
            .HasQueryFilter(s => !s.isDeleted);

        modelBuilder.Entity<Links>()
            .HasQueryFilter(l => !l.isDeleted);

        modelBuilder.Entity<Notas>()
            .HasQueryFilter(n => !n.isDeleted);
    }

    public override int SaveChanges()
    {
        UpdateTimestamps();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateTimestamps()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            if (entry.Entity.GetType().GetProperty("LastModified") != null)
            {
                entry.Property("LastModified").CurrentValue = DateTime.UtcNow;
            }
        }
    }
}