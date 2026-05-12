namespace ERP.Domain;

public sealed record ResourceNode(
    string Reference,
    ItemId? Resource,
    NodePurity Purity,
    Position Position);
