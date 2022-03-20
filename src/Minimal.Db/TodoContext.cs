using Microsoft.EntityFrameworkCore;
using Minimal.Model;

namespace Minimal.Db;

public class TodoContext : DbContext
{
    public TodoContext(DbContextOptions<TodoContext> options)
        : base(options)
    {
    }

    public DbSet<TodoItem> Items => Set<TodoItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder) => 
        modelBuilder.Entity<TodoItem>(
            static item =>
            {
                item.HasKey(static x => x.Id);
                item.Property(static x => x.Name);
                item.Property(static x => x.Status);
            });
}
