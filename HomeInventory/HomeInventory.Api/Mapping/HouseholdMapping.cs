using HomeInventory.Contracts;
using HomeInventory.Domain;

namespace HomeInventory.Api.Mapping
{
    public static class HouseholdMapping
    {
        public static HouseholdDto Map(Household household) =>
            new(household.Id,
                household.Name);
    }
}
