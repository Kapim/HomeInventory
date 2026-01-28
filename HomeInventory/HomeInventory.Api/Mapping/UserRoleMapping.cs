using HomeInventory.Contracts;
using HomeInventory.Domain;

namespace HomeInventory.Api.Mapping
{
    public static class UserRoleMapping
    {
        public static UserRoleDto Map(UserRole role) => role switch
        {
            UserRole.Parent => UserRoleDto.Parent,
            UserRole.Child => UserRoleDto.Child,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}
