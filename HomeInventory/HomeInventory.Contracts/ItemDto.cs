using System;
using System.Collections.Generic;
using System.Text;

namespace HomeInventory.Contracts
{
    public sealed record ItemDto(
        Guid Id,
        string Name,
        int Quantity,
        Guid LocationId,
        Guid OwnerId,
        string? PlacementNote,
        string? Description
    );
}
