namespace HomeInventory.Contracts.Requests
{
    public sealed record CreateTagRequestDto(string Name, string Color, Guid HouseholdId);
}
