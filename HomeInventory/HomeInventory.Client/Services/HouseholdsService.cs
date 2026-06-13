using HomeInventory.Client.Mapping;
using HomeInventory.Client.Models;
using HomeInventory.Client.Services.Interfaces;
using HomeInventory.Contracts;
using HomeInventory.Contracts.Requests;
using System;
using System.Collections.Generic;
using System.Text;

namespace HomeInventory.Client.Services
{
    public class HouseholdsService(IHouseholdsApiClient apiClient, ITagsApiClient tagsApiClient) : IHouseholdsService
    {
        private readonly IHouseholdsApiClient _apiClient = apiClient;
        private readonly ITagsApiClient _tagsApiClient = tagsApiClient;


        public async Task<IReadOnlyList<Household>> GetAllAsync(CancellationToken ct)
        {
            return [.. (await _apiClient.GetAllAsync(ct)).Select(x => HouseholdMapping.Map(x))];
        }

        public async Task<Household> GetByIdAsync(Guid householdId, CancellationToken ct)
        {
            return HouseholdMapping.Map(await _apiClient.GetByIdAsync(householdId, ct));
        }

        public async Task<Household> CreateAsync(string name, CancellationToken ct)
        {
            CreateHouseholdRequestDto request = new(name);
            return HouseholdMapping.Map(await _apiClient.CreateAsync(request, ct));
        }

        public async Task<IReadOnlyList<LocationListItem>> GetLocationsAsync(Guid householdId, CancellationToken ct)
        {
            return [.. (await _apiClient.GetLocations(householdId, ct)).Select(x => LocationMapping.Map(x))];
        }

        public async Task<IReadOnlyList<Item>> GetItemsAsync(Guid householdId, CancellationToken ct)
        {
            return [.. (await _apiClient.GetItems(householdId, ct)).Select(x => ItemMapping.Map(x))];
        }

        public async Task<IReadOnlyList<Models.Tag>> GetTagsAsync(Guid householdId, CancellationToken ct)
        {
            var dtos = await _tagsApiClient.GetTagsForHouseholdAsync(householdId, ct);
            return dtos.Select(d => new Models.Tag(d.Id, d.Name, d.Color)).ToList();
        }
    }
}
