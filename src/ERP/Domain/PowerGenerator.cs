namespace ERP.Domain;

public sealed record PowerGenerator(
    string Reference,
    GeneratorKind Kind,
    Position Position);
