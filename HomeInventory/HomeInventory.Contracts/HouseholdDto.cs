using System;
using System.Collections.Generic;
using System.Text;

namespace HomeInventory.Contracts
{
    public sealed record HouseholdDto(
        Guid Id,
        string Name
    );
}
