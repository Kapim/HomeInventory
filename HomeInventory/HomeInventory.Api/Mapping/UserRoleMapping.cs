using HomeInventory.Contracts;
using HomeInventory.Domain.Enums;

namespace HomeInventory.Api.Mapping
{
    public static class UserRoleMapping
    {
        public static UserRoleDto Map(UserRole role) => role switch
        {
            UserRole.Parent => UserRoleDto.Parent,
            UserRole.Child => UserRoleDto.Child,
            _ => throw new ArgumentOutOfRangeException(nameof(role))
        };

        public static UserRole Map(UserRoleDto role) => role switch
        {
            UserRoleDto.Parent => UserRole.Parent,
            UserRoleDto.Child => UserRole.Child,
            _ => throw new ArgumentOutOfRangeException(nameof(role))
        };
    }
}
