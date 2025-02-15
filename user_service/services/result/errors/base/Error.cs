namespace user_service.services.result.errors.@base;
public sealed record Error(string Code, string Description)
{
    public static readonly Error None = new(string.Empty, string.Empty);

    public override string ToString() 
        => $"Error (#{Code}): {Description}";
}
