namespace HomeInventory.Contracts.Requests
{
    public sealed record CreateItemRequestDto(string Name,
        int Quantity,
        Guid LocationId,
        Guid OwnerId,
        string? PlacementNote,
        string? Description);
}
