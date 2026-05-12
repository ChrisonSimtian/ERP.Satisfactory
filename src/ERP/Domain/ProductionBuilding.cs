namespace ERP.Domain;

/// <summary>
/// A placed factory building that runs a recipe (smelter, constructor,
/// assembler, foundry, manufacturer, refinery, …). RecipeId is unset for v1 —
/// recipe binding requires deeper property-tree parsing and is tracked as a
/// follow-up on milestone #12.
/// </summary>
public sealed record ProductionBuilding(
    string Reference,
    BuildingId Building,
    Position Position,
    RecipeId? Recipe);
