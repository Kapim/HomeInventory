namespace HomeInventory.Contracts
{
    public record RegisterRequestDto(string UserName, string Password, UserRoleDto UserRole);
}
