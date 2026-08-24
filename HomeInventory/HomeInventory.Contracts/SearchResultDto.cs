namespace HomeInventory.Contracts
{
    public enum SearchResultKindDto { Item, Location }

    public sealed record SearchResultDto(
        Guid Id,
        string Name,
        SearchResultKindDto Kind,
        Guid LocationId,
        string LocationPath,
        string? Description,
        string? TagMatch);
}
