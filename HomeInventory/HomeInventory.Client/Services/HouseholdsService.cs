using HomeInventory.Client.Mapping;
using HomeInventory.Client.Models;
using HomeInventory.Client.Services.Interfaces;
using HomeInventory.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace HomeInventory.Client.Services
{
    public class HouseholdsService(IHouseholdsApiClient apiClient) : IHouseholdsService
    {
        private readonly IHouseholdsApiClient _apiClient = apiClient;


        public async Task<IReadOnlyList<Household>> GetAllAsync(CancellationToken ct)
        {
            return [.. (await _apiClient.GetAllAsync(ct)).Select(x => HouseholdMapping.Map(x))];
        }

        public Task<Household> CreateAsync(string name)
        {
            throw new NotImplementedException();
        }
    }
}
