namespace HomeInventory.Contracts.Requests
{
    public record UpdateItemRequestDto(string Name, string? Description, int Quantinty, string? PlacementNote, Guid LocationId);
}