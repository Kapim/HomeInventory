namespace HomeInventory.Desktop.Wpf.ViewModels
{
    public enum SearchResultType { Item, Location }

    public record SearchResult(
        Guid Id,
        string Name,
        SearchResultType Type,
        string LocationPath,
        Guid LocationId
    )
    {
        public string? TagMatch { get; init; }
    };
}
