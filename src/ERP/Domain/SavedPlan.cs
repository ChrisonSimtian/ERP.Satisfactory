namespace ERP.Domain;

/// <summary>
/// A user-saved plan definition: the inputs (<see cref="ProductionTarget"/>s the user
/// wants and <see cref="ResourceAvailability"/> they have on hand) that the planner
/// re-evaluates into a <see cref="ProductionPlan"/> on demand.
///
/// <para>
/// This is the aggregate persisted by the EF Core infrastructure. The computed
/// <see cref="ProductionPlan"/> result is intentionally NOT persisted — it is a pure
/// function of (catalogue, targets, available) and recomputing keeps saved plans
/// valid across catalogue updates.
/// </para>
///
/// <para>
/// Modelled as a mutable class (not a record) because it has a lifecycle: created
/// once, edited many times. EF Core also tracks mutable entities more naturally.
/// </para>
/// </summary>
public sealed class SavedPlan
{
    // Backing fields are concrete List<T> rather than IReadOnlyList<T> so EF
    // Core's collection accessor can mutate them when materialising owned
    // collections. A bare `= []` default produces a fixed-size array, which
    // EF picks up via the property and then fails on .Add() during hydration
    // (NotSupportedException: Collection was of a fixed size).
    private List<ProductionTarget> _targets = new();
    private List<ResourceAvailability> _available = new();

    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public IReadOnlyList<ProductionTarget> Targets => _targets;
    public IReadOnlyList<ResourceAvailability> Available => _available;
    public DateTime CreatedUtc { get; private set; }
    public DateTime UpdatedUtc { get; private set; }

    /// <summary>Parameterless ctor for EF Core materialisation. Don't call from app code.</summary>
    private SavedPlan() { }

    public SavedPlan(
        Guid id,
        string name,
        IReadOnlyList<ProductionTarget> targets,
        IReadOnlyList<ResourceAvailability> available,
        DateTime createdUtc,
        DateTime updatedUtc)
    {
        if (id == Guid.Empty) throw new ArgumentException("Id must not be empty.", nameof(id));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required.", nameof(name));

        Id = id;
        Name = name;
        _targets = targets.ToList();
        _available = available.ToList();
        CreatedUtc = createdUtc;
        UpdatedUtc = updatedUtc;
    }

    public void Rename(string name, DateTime nowUtc)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required.", nameof(name));
        Name = name;
        UpdatedUtc = nowUtc;
    }

    public void Replace(
        IReadOnlyList<ProductionTarget> targets,
        IReadOnlyList<ResourceAvailability> available,
        DateTime nowUtc)
    {
        _targets = targets.ToList();
        _available = available.ToList();
        UpdatedUtc = nowUtc;
    }
}
