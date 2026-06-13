namespace HomeInventory.Desktop.Wpf.Services
{
    public sealed class TagPickerResult
    {
        public IReadOnlyList<Guid> FinalSelectedTagIds { get; init; } = [];
        public IReadOnlyList<NewTagSpec> NewTagsToCreate { get; init; } = [];
        public IReadOnlyList<Guid> TagIdsToDelete { get; init; } = [];

        public record NewTagSpec(string Name, string Color);
    }
}
