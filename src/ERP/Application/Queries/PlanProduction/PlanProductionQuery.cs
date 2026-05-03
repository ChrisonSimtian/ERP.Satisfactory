using ERP.Domain;

namespace ERP.Application.Queries.PlanProduction;

public sealed record PlanProductionQuery(
    IReadOnlyList<ProductionTarget> Targets,
    IReadOnlyList<ResourceAvailability> Available);

public sealed record PlanProductionResult(bool IsFeasible, string Message);

public static class PlanProductionHandler
{
    public static PlanProductionResult Handle(PlanProductionQuery query)
        => new(IsFeasible: false, Message: "Planner not yet implemented");
}
