namespace HomeInventory.Application.Models
{
    public sealed record TagCreateRequest(string Name, string Color, Guid HouseholdId);
}
