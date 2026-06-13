namespace HomeInventory.Mobile.Maui.Services;

public interface ISessionState
{
    Guid? SelectedHouseholdId { get; set; }
    string? SelectedHouseholdName { get; set; }
    Guid? PendingLocationId { get; set; }
}

public class SessionState : ISessionState
{
    private const string IdKey = "last_household_id";
    private const string NameKey = "last_household_name";

    public Guid? SelectedHouseholdId
    {
        get
        {
            var raw = Preferences.Default.Get<string?>(IdKey, null);
            return raw is not null && Guid.TryParse(raw, out var id) ? id : null;
        }
        set
        {
            if (value is Guid id) Preferences.Default.Set(IdKey, id.ToString());
            else Preferences.Default.Remove(IdKey);
        }
    }

    public string? SelectedHouseholdName
    {
        get => Preferences.Default.Get<string?>(NameKey, null);
        set
        {
            if (value is not null) Preferences.Default.Set(NameKey, value);
            else Preferences.Default.Remove(NameKey);
        }
    }

    public Guid? PendingLocationId { get; set; }
}
