using HomeInventory.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace HomeInventory.Application.Models
{
    public sealed record LocationUpdateRequest(string Name, LocationType LocationType, Guid? ParentLocationId, int SortOrder, string? Description);
}
