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
        public string? Description { get; init; }

        /// <summary>Shortened description shown inline after the name (" — text…"), or "" when there is none.</summary>
        public string DescriptionInline =>
            string.IsNullOrWhiteSpace(Description)
                ? ""
                : " — " + (Description.Length > 50 ? Description[..50].TrimEnd() + "…" : Description);
    };
}
