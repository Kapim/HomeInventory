using HomeInventory.Domain.Enums;

namespace HomeInventory.Application.Models
{
    public sealed record LoginResult(string Token, UserRole UserRole);
}
