using HomeInventory.Domain;
using HomeInventory.Domain.Enums;

namespace HomeInventory.Application.UseCases
{
    public interface IUserUserCase
    {

        public Task<Household> FindUserByNameAsync(string name);
        public Task AddUserAsync(string userName, string password, UserRole userRole);
        public Task RenameUserAsync(Guid id, string newUserName);
        public Task ChangePasswordAsync(Guid id, string newPassword);
        public Task ChangeRole(Guid id, UserRole newUserRole);
    }
}
