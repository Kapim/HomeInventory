using HomeInventory.Client.Models;

namespace HomeInventory.Client.Requests
{
    public sealed record ItemCreateRequest(string Name,
        int Quantity,
        Guid LocationId,
        Guid OwnerId,
        string? PlacementNote,
        string? Description);

}
