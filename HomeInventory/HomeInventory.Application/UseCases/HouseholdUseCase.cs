using HomeInventory.Application.Interfaces;
using HomeInventory.Application.Models;
using HomeInventory.Domain;
using HomeInventory.Domain.Enums;
using System.Text;

namespace HomeInventory.Application.UseCases
{
    public class HouseholdUseCase(IHouseholdRepository households, IItemRepository items, ILocationRepository locations) : IHouseholdUseCase
    {
        private readonly IHouseholdRepository _households = households;
        private readonly IItemRepository _items = items;
        private readonly ILocationRepository _locations = locations;

        public async Task AddHouseholdAsync(string name)
        {
            Household household = new(name); 
            await _households.AddAsync(household, new CancellationTokenSource().Token);
        }
        public async Task<Household> GetHouseholdAsync(Guid id)
        {
            return await _households.GetByIdAsync(id, new CancellationTokenSource().Token);
        }

        public async Task<IReadOnlyList<Household>> GetHouseholdsAsync()
        {
            return await _households.GetHouseholdsAsync(new CancellationTokenSource().Token);
        }

        public async Task<IReadOnlyList<Location>> GetLocationsAsync(Guid householdId)
        {
            return await _households.GetLocationsAsync(householdId, new CancellationTokenSource().Token);
        }

        public async Task<IReadOnlyList<Item>> GetItemsAsync(Guid householdId)
        {
            var locations = await _households.GetLocationsAsync(householdId, new CancellationTokenSource().Token);
            var result = new List<Item>();
            foreach (var loc in locations)
                result.AddRange(await _items.GetByLocationAsync(loc.Id, new CancellationTokenSource().Token));
            return result;
        }

        public async Task<string> ExportCsvAsync(Guid householdId, CancellationToken ct = default)
        {
            var locationList = await _households.GetLocationsAsync(householdId, ct);
            var sb = new StringBuilder();

            sb.AppendLine("#LOCATIONS");
            sb.AppendLine("Id,ParentId,Name,Type,SortOrder,Description");
            foreach (var loc in locationList)
            {
                sb.AppendLine($"{loc.Id},{loc.ParentLocationId?.ToString() ?? ""},\"{EscapeCsv(loc.Name)}\",{loc.LocationType},{loc.SortOrder},\"{EscapeCsv(loc.Description)}\"");
            }

            sb.AppendLine();
            sb.AppendLine("#ITEMS");
            sb.AppendLine("LocationId,Name,Quantity,Description,PlacementNote,Tags");
            foreach (var loc in locationList)
            {
                var locItems = await _items.GetByLocationAsync(loc.Id, ct);
                foreach (var item in locItems)
                {
                    var tags = string.Join(";", item.Tags.Select(t => t.Name));
                    sb.AppendLine($"{loc.Id},\"{EscapeCsv(item.Name)}\",{item.Quantity},\"{EscapeCsv(item.Description)}\",\"{EscapeCsv(item.PlacementNote)}\",\"{EscapeCsv(tags)}\"");
                }
            }

            return sb.ToString();
        }

        public async Task<ImportResult> ImportCsvAsync(Guid householdId, Stream stream, Guid userId, CancellationToken ct = default)
        {
            var errors = new List<string>();
            int locationsImported = 0;
            int itemsImported = 0;

            using var reader = new StreamReader(stream);
            var content = await reader.ReadToEndAsync(ct);
            var lines = content.Split('\n', StringSplitOptions.RemoveEmptyEntries)
                               .Select(l => l.TrimEnd('\r'))
                               .ToArray();

            var section = "";
            var locationIdMap = new Dictionary<Guid, Guid>(); // old CSV Id → new created Id

            foreach (var line in lines)
            {
                if (line.StartsWith('#'))
                {
                    section = line.Trim('#').Trim().ToUpperInvariant();
                    continue;
                }

                if (line.StartsWith("Id,") || line.StartsWith("LocationId,"))
                    continue; // skip headers

                var cols = ParseCsvLine(line);

                if (section == "LOCATIONS" && cols.Length >= 5)
                {
                    try
                    {
                        if (!Guid.TryParse(cols[0], out var oldId))
                        {
                            errors.Add($"Neplatné ID lokace: {cols[0]}");
                            continue;
                        }

                        Guid? parentId = null;
                        if (!string.IsNullOrEmpty(cols[1]) && Guid.TryParse(cols[1], out var oldParentId))
                            parentId = locationIdMap.TryGetValue(oldParentId, out var mappedParent) ? mappedParent : null;

                        var name = cols[2];
                        Enum.TryParse<LocationType>(cols[3], out var locType);
                        int.TryParse(cols[4], out var sortOrder);
                        var description = cols.Length > 5 ? cols[5] : null;

                        var loc = new Location(name, householdId, userId, parentId, locType);
                        loc.SortOrder = sortOrder;
                        if (!string.IsNullOrEmpty(description))
                            loc.SetDescription(description);

                        await _locations.AddAsync(loc, ct);
                        locationIdMap[oldId] = loc.Id;
                        locationsImported++;
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"Lokace '{(cols.Length > 2 ? cols[2] : "?")}': {ex.Message}");
                    }
                }
                else if (section == "ITEMS" && cols.Length >= 3)
                {
                    try
                    {
                        if (!Guid.TryParse(cols[0], out var oldLocId) || !locationIdMap.TryGetValue(oldLocId, out var newLocId))
                        {
                            errors.Add($"Neznámá lokace pro položku '{(cols.Length > 1 ? cols[1] : "?")}'");
                            continue;
                        }

                        var name = cols[1];
                        int.TryParse(cols[2], out var qty);
                        var description = cols.Length > 3 ? cols[3] : null;
                        var placementNote = cols.Length > 4 ? cols[4] : null;

                        var item = new Item(name, userId, newLocId);
                        item.SetQuantity(qty);
                        if (!string.IsNullOrEmpty(description)) item.SetDescription(description);
                        if (!string.IsNullOrEmpty(placementNote)) item.SetPlacementNote(placementNote);

                        await _items.AddAsync(item, ct);
                        itemsImported++;
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"Položka '{(cols.Length > 1 ? cols[1] : "?")}': {ex.Message}");
                    }
                }
            }

            return new ImportResult(locationsImported, itemsImported, errors);
        }

        private static string EscapeCsv(string? value) => (value ?? "").Replace("\"", "\"\"");

        private static string[] ParseCsvLine(string line)
        {
            var result = new List<string>();
            var i = 0;
            while (i < line.Length)
            {
                if (line[i] == '"')
                {
                    i++;
                    var sb = new StringBuilder();
                    while (i < line.Length)
                    {
                        if (line[i] == '"' && i + 1 < line.Length && line[i + 1] == '"')
                        {
                            sb.Append('"');
                            i += 2;
                        }
                        else if (line[i] == '"')
                        {
                            i++;
                            break;
                        }
                        else
                        {
                            sb.Append(line[i++]);
                        }
                    }
                    result.Add(sb.ToString());
                    if (i < line.Length && line[i] == ',') i++;
                }
                else
                {
                    var end = line.IndexOf(',', i);
                    if (end == -1)
                    {
                        result.Add(line[i..]);
                        break;
                    }
                    result.Add(line[i..end]);
                    i = end + 1;
                }
            }
            return [.. result];
        }
    }
}
