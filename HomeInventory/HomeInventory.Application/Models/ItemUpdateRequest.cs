using HomeInventory.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace HomeInventory.Application.Models
{
    public sealed record ItemUpdateRequest(string Name, string? Description, int Quantinty, string? PlacementNote, Guid LocationId);
}
