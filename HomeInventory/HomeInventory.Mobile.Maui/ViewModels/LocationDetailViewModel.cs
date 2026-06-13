using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HomeInventory.Client;
using HomeInventory.Client.Errors;
using HomeInventory.Client.Mapping;
using HomeInventory.Client.Models;
using HomeInventory.Client.Requests;
using HomeInventory.Client.Services.Interfaces;
using HomeInventory.Mobile.Maui.Services;
using ClientLocation = HomeInventory.Client.Models.Location;

namespace HomeInventory.Mobile.Maui.ViewModels;

public partial class LocationDetailViewModel : ObservableObject
{
    private readonly ILocationsService _locations;
    private readonly IDialogService _dialogs;
    private readonly IErrorLocalizer _errorLocalizer;
    private readonly INotificationsService _notifications;
    private readonly IBusyService _busy;

    private LocationNodeViewModel? _node;
    private ClientLocation? _location;
    private bool _suppressAutoSave;
    private readonly SemaphoreSlim _autoSaveGate = new(1, 1);
    private int _autoSaveVersion;

    public IReadOnlyList<LocationType> LocationTypes { get; } = Enum.GetValues<LocationType>();

    public bool HasLocation => _location is not null;
    public bool IsBusy => _busy.IsBusy;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    private string? name;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    private LocationType selectedLocationType;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    private string? description;

    public LocationDetailViewModel(
        ILocationsService locations,
        IDialogService dialogs,
        IErrorLocalizer errorLocalizer,
        INotificationsService notifications,
        IBusyService busy)
    {
        _locations = locations;
        _dialogs = dialogs;
        _errorLocalizer = errorLocalizer;
        _notifications = notifications;
        _busy = busy;
        _busy.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(IBusyService.IsBusy))
            {
                OnPropertyChanged(nameof(IsBusy));
                SaveCommand.NotifyCanExecuteChanged();
            }
        };
    }

    public async Task LoadAsync(LocationNodeViewModel node, CancellationToken ct = default)
    {
        if (node.IsNew)
        {
            Clear();
            Name = node.Name;
            return;
        }

        await _busy.Run(async () =>
        {
            try
            {
                var location = await _locations.GetByIdAsync(node.Id, ct);
                _node = node;
                _location = location;
                SetFieldsFromLocation(location);
                OnPropertyChanged(nameof(HasLocation));
                SaveCommand.NotifyCanExecuteChanged();
            }
            catch (ApiException ex)
            {
                Clear();
                var message = _errorLocalizer.GetString(ex.Type);
                _dialogs.ShowError("Operace selhala", message);
            }
        });
    }

    public void Clear()
    {
        _node = null;
        _location = null;
        _suppressAutoSave = true;
        try
        {
            Name = null;
            Description = null;
            SelectedLocationType = default;
        }
        finally
        {
            _suppressAutoSave = false;
        }
        OnPropertyChanged(nameof(HasLocation));
        SaveCommand.NotifyCanExecuteChanged();
    }

    private bool CanSave()
        => HasLocation && !IsBusy && !string.IsNullOrWhiteSpace(Name);

    [RelayCommand(CanExecute = nameof(CanSave))]
    private async Task Save()
        => await SaveCore(CancellationToken.None);

    private async Task SaveCore(CancellationToken ct)
    {
        if (_location is null || _node is null)
            return;

        if (string.IsNullOrWhiteSpace(Name))
            return;

        var request = new LocationUpdateRequest(
            Name.Trim(),
            SelectedLocationType,
            _location.ParentLocationId,
            _location.SortOrder,
            Description);

        await _busy.Run(async () =>
        {
            try
            {
                var updated = await _locations.UpdateAsync(_location.Id, request, ct);
                _location = updated;
                _node.SetLocation(LocationMapping.MapToListItem(updated));
                SetFieldsFromLocation(updated);
                _notifications.Success("Uloženo");
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested) { }
            catch (Exception ex) when (ex is ApiException || ex is InvalidOperationException)
            {
                string message = ex.Message;
                if (ex is ApiException apiEx)
                    message = _errorLocalizer.GetString(apiEx.Type);
                _dialogs.ShowError("Operace selhala", message);
            }
        });
    }

    partial void OnNameChanged(string? value) => QueueAutoSave();
    partial void OnSelectedLocationTypeChanged(LocationType value) => QueueAutoSave();
    partial void OnDescriptionChanged(string? value) => QueueAutoSave();

    private void QueueAutoSave()
    {
        if (_suppressAutoSave || !CanSave())
            return;

        var version = Interlocked.Increment(ref _autoSaveVersion);
        _ = SaveLatestChangeAsync(version);
    }

    private async Task SaveLatestChangeAsync(int version)
    {
        await _autoSaveGate.WaitAsync();
        try
        {
            if (version != _autoSaveVersion) return;
            await SaveCore(CancellationToken.None);
        }
        finally
        {
            _autoSaveGate.Release();
        }
    }

    private void SetFieldsFromLocation(ClientLocation location)
    {
        _suppressAutoSave = true;
        try
        {
            Name = location.Name;
            SelectedLocationType = location.LocationType;
            Description = location.Description;
        }
        finally
        {
            _suppressAutoSave = false;
        }
    }
}
