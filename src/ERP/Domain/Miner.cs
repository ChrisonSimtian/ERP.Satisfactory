namespace ERP.Domain;

public sealed record Miner(
    string Reference,
    MinerTier Tier,
    Position Position,
    string? ResourceNodeReference);
