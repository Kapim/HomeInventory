using HomeInventory.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace HomeInventory.Application.Models
{
    public sealed record ItemCreateRequest(string Name,
        int Quantity,
        Guid LocationId,
        string? PlacementNote,
        string? Description);
}
