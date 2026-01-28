using HomeInventory.Domain;

namespace HomeInventory.Application.Models
{
    public sealed record LoginResult(Guid Id, UserRole UserRole);
}
