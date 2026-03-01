namespace HomeInventory.Contracts.Requests
{
    public record UpdateLocationRequestDto(string Name, LocationTypeDto Type, int SortOrder, string? Description, Guid? parentLocationId);
}