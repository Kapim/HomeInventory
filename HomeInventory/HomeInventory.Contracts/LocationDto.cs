using System;
using System.Collections.Generic;
using System.Text;

namespace HomeInventory.Contracts
{
    public sealed record LocationDto(
        Guid Id,
        string Name,
        LocationTypeDto Type,
        Guid? ParentLocationId,
        Guid HouseholdId,
        Guid OwnerUserId,
        int SortOrder,
        string? Description
    );
}
