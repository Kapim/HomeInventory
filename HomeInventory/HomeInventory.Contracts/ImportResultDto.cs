namespace HomeInventory.Contracts
{
    public sealed record ImportResultDto(int LocationsImported, int ItemsImported, List<string> Errors);
}
