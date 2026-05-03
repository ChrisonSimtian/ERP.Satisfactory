namespace ERP.Domain;

public readonly record struct RecipeId(string Value)
{
    public override string ToString() => Value;
}
