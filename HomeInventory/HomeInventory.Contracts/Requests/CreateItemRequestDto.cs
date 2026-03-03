namespace HomeInventory.Contracts.Requests
{
    public sealed record CreateItemRequestDto(string Name,
        int Quantity,
        Guid LocationId,
        string? PlacementNote,
        string? Description);
}
