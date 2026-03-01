using HomeInventory.Client.Models;

namespace HomeInventory.Client.Requests
{
    public sealed record ItemUpdateRequest(string Name, string? Description, int Quantinty, string? PlacementNote, Guid LocationId);

}
