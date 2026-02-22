using HomeInventory.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace HomeInventory.Application.UseCases
{
    public interface IHouseholdUseCase
    {

        public Task<Household> GetHouseholdAsync(Guid id);
        public Task AddHouseholdAsync(string name);
        public Task<IReadOnlyList<Household>> GetHouseholdsAsync();
    }
}
