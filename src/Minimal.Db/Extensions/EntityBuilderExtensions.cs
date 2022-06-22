using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Minimal.Model.Base;

namespace Minimal.Db.Extensions;

internal static class EntityBuilderExtensions
{
    public static void MapBaseProperties<TEntity>(this EntityTypeBuilder<TEntity> builder)
        where TEntity : BaseEntity
    {
        builder.HasKey(static x => x.Id);
        builder.Property(static x => x.Id)
            .ValueGeneratedOnAdd();

        builder.Ignore(static x => x.Events);
    }
}