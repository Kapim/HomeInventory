using HomeInventory.Client.Models;
using HomeInventory.Contracts;

namespace HomeInventory.Client.Mapping
{
    public static class HouseholdMapping
    {
        public static HouseholdDto Map(Household household) =>
            new(household.Id,
                household.Name);

        public static Household Map(HouseholdDto householdDto) =>
            new(householdDto.Id,
                householdDto.Name);
    }
}