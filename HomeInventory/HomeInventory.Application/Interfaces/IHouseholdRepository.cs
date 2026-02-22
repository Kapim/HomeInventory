using HomeInventory.Domain;
using HomeInventory.Domain.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace HomeInventory.Application.Interfaces
{
    public interface IHouseholdRepository
    {
        public Task AddAsync(Household household, CancellationToken ct = default);
        
        public Task RemoveAsync(Household household, CancellationToken ct = default);

        public Task UpdateAsync(Household household, CancellationToken ct = default);

        public Task<IReadOnlyList<Household>> GetHouseholdsAsync(CancellationToken ct = default);
        public Task<Household> GetByIdAsync(Guid id, CancellationToken ct = default);
    }
}
