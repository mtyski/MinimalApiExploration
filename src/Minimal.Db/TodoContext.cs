using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Minimal.Db.Extensions;
using Minimal.Model;
using Minimal.Model.Base;

namespace Minimal.Db;

public class TodoContext : DbContext
{
    public TodoContext(
        DbContextOptions<TodoContext> options,
        IMediator mediator)
        : base(options)
    {
        Mediator = mediator;
    }

    public DbSet<TodoItem> Items => Set<TodoItem>();

    private IMediator Mediator { get; }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        var events = GetAllDomainEvents().ToList();

        var result = base.SaveChanges(acceptAllChangesOnSuccess);

        Task.WaitAll(events.Select(e => Mediator.Publish(e)).ToArray());

        return result;
    }

    public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = new ())
    {
        var events = GetAllDomainEvents().ToList();

        var result = await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);

        await Task.WhenAll(events.Select(e => Mediator.Publish(e, cancellationToken)).ToArray());

        return result;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) =>
        modelBuilder.Entity<TodoItem>(
            static item =>
            {
                item.MapBaseProperties();
                item.Property(static x => x.Name);
                item.Property(static x => x.Status);
            });

    private IEnumerable<DomainEvent> GetAllDomainEvents() =>
        ChangeTracker.Entries<BaseEntity>()
            .SelectMany(static e => e.Entity.Events);
}