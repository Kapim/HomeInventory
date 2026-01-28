using HomeInventory.Application.Models;

namespace HomeInventory.Application.UseCases
{
    public interface ILoginUseCase
    {
        public Task<LoginResult> LoginAsync(string username, string password);        
    }
}
