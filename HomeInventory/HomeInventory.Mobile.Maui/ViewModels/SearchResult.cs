namespace HomeInventory.Mobile.Maui.ViewModels;

public enum SearchResultType { Item, Location }

public record SearchResult(Guid Id, string Name, SearchResultType Type, string LocationPath, Guid LocationId)
{
    public string Icon => Type == SearchResultType.Item ? "📦" : "📁";
    public string? TagMatch { get; init; }
    public bool HasTagMatch => TagMatch is not null;

    public string? Description { get; init; }
    public bool HasDescription => !string.IsNullOrWhiteSpace(Description);

    /// <summary>Shortened description shown inline in results (full text is up to 100 chars).</summary>
    public string DescriptionPreview =>
        Description is { Length: > 60 } ? Description[..60].TrimEnd() + "…" : Description ?? "";
}
