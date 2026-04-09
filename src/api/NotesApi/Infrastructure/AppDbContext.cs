using Microsoft.EntityFrameworkCore;
using NotesApi.Features.Notes;

namespace NotesApi.Infrastructure;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Note> Notes => Set<Note>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Note>(entity =>
        {
            entity.HasKey(n => n.Id);
            entity.Property(n => n.Title).HasMaxLength(150).IsRequired();
            entity.Property(n => n.Content).HasMaxLength(2000).IsRequired();
            entity.HasQueryFilter(n => n.IsActive);
        });
    }
}
