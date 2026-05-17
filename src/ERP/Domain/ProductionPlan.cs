namespace ERP.Domain;

public sealed record ProductionPlan(
    IReadOnlyList<ProductionTarget> Targets,
    IReadOnlyList<ResourceAvailability> Available,
    IReadOnlyList<ProductionStep> Steps,
    IReadOnlyList<ItemAmount> RawInputsConsumed,
    IReadOnlyList<InfeasibleItem> MissingInputs,
    IReadOnlyList<ExtractorAllocation>? ExtractorAllocations = null)
{
    public bool IsFeasible => MissingInputs.Count == 0;

    /// <summary>
    /// Non-null shorthand for the optional <see cref="ExtractorAllocations"/>.
    /// Most planner outputs (no node binding) leave it as the empty list.
    /// </summary>
    public IReadOnlyList<ExtractorAllocation> Allocations =>
        ExtractorAllocations ?? Array.Empty<ExtractorAllocation>();
}
