namespace HomeInventory.Contracts.Requests
{
    public record RegisterRequestDto(string UserName, string Password, UserRoleDto UserRole);
}
