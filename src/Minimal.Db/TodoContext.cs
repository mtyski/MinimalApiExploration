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

    protected override void OnModelCreating(ModelBuilder modelBuilder) => modelBuilder.Entity<TodoItem>(
            item =>
            {
                item.HasKey(x => x.Id);
                item.Property(x => x.Name);
                item.Property(x => x.Status);
            });
}
