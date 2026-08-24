using HomeInventory.Domain;

namespace HomeInventory.Application.UseCases
{
    public interface IHouseholdUseCase
    {

        public Task<Household> GetHouseholdAsync(Guid id);
        public Task AddHouseholdAsync(string name);
        public Task<IReadOnlyList<Household>> GetHouseholdsAsync();
        Task<IReadOnlyList<Location>> GetLocationsAsync(Guid householdId);
        Task<IReadOnlyList<Item>> GetItemsAsync(Guid householdId);
        Task<string> ExportCsvAsync(Guid householdId, CancellationToken ct = default);
        Task<ImportResult> ImportCsvAsync(Guid householdId, Stream stream, Guid userId, CancellationToken ct = default);
        Task<IReadOnlyList<SearchResultItem>> SearchAsync(Guid householdId, string query, CancellationToken ct = default);
    }

    public sealed record ImportResult(int LocationsImported, int ItemsImported, List<string> Errors);

    public enum SearchResultKind { Item, Location }

    public sealed record SearchResultItem(
        Guid Id,
        string Name,
        SearchResultKind Kind,
        Guid LocationId,
        string LocationPath,
        string? Description,
        string? TagMatch);
}
