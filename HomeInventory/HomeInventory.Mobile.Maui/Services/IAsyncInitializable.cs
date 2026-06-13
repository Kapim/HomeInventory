namespace HomeInventory.Mobile.Maui.Services;

public interface IAsyncInitializable
{
    Task InitializeAsync(CancellationToken ct = default);
}
