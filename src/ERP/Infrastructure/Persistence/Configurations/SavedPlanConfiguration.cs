using ERP.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core mapping for the <see cref="SavedPlan"/> aggregate.
///
/// <para>
/// The two collections (<see cref="SavedPlan.Targets"/>, <see cref="SavedPlan.Available"/>)
/// are persisted as owned collections in their own tables. This keeps the model
/// relational (queryable, indexable) without forcing JSON column support, which
/// not every candidate provider offers equally (SQLite needs 3.39+, SQL Server
/// has its own variant). When a provider is picked, switch to <c>ToJson()</c>
/// instead if simpler — both shapes work, and the aggregate boundary is the same.
/// </para>
/// </summary>
internal sealed class SavedPlanConfiguration : IEntityTypeConfiguration<SavedPlan>
{
    public void Configure(EntityTypeBuilder<SavedPlan> builder)
    {
        builder.ToTable("Plans");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id).ValueGeneratedNever();

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.CreatedUtc).IsRequired();
        builder.Property(p => p.UpdatedUtc).IsRequired();

        builder.OwnsMany<ProductionTarget>(
            nameof(SavedPlan.Targets),
            targets =>
            {
                targets.ToTable("PlanTargets");
                targets.WithOwner().HasForeignKey("PlanId");
                // ValueGeneratedNever: the ordinal is assigned by the
                // PlanDbContext.SaveChanges override based on list position —
                // owned collections under EF Core 10 default this property to
                // store-generated, which works on Postgres (identity) but not
                // SQLite (no auto-increment for composite-key columns).
                targets.Property<int>("Ordinal").ValueGeneratedNever();
                targets.HasKey("PlanId", "Ordinal");

                targets.Property(t => t.Item)
                    .HasConversion(id => id.Value, value => new ItemId(value))
                    .HasColumnName("ItemId")
                    .IsRequired()
                    .HasMaxLength(200);

                targets.Property(t => t.ItemsPerMinute)
                    .HasColumnType("decimal(18,4)");
            });

        builder.OwnsMany<ResourceAvailability>(
            nameof(SavedPlan.Available),
            avail =>
            {
                avail.ToTable("PlanAvailability");
                avail.WithOwner().HasForeignKey("PlanId");
                // See note on Targets — explicit ordinal assignment in the
                // SaveChanges override, so disable EF's store-generated default.
                avail.Property<int>("Ordinal").ValueGeneratedNever();
                avail.HasKey("PlanId", "Ordinal");

                avail.Property(a => a.Item)
                    .HasConversion(id => id.Value, value => new ItemId(value))
                    .HasColumnName("ItemId")
                    .IsRequired()
                    .HasMaxLength(200);

                avail.Property(a => a.ItemsPerMinute)
                    .HasColumnType("decimal(18,4)");
            });

        // SavedPlan exposes its child lists as read-only views over private
        // List<T> backing fields (_targets, _available). Field-mode lets EF
        // hydrate those mutable backings during materialisation; using the
        // public IReadOnlyList property would route through SZArrayHelper.Add
        // for the parameterless-ctor case and fail with a fixed-size error.
        builder.Navigation(nameof(SavedPlan.Targets))
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasField("_targets");
        builder.Navigation(nameof(SavedPlan.Available))
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasField("_available");
    }
}
