namespace HomeInventory.Contracts.Requests
{
    public record CreateHouseholdRequestDto(Guid OwnerId, string Name);
}
