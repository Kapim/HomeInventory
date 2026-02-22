using HomeInventory.Application.Models;
using HomeInventory.Domain.Enums;

namespace HomeInventory.Application.UseCases
{
    public interface IRegisterUseCase
    {
        public Task RegisterAsync(string username, string password, UserRole userRole);        
    }
}
