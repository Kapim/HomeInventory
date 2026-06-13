namespace HomeInventory.Mobile.Maui.Services;

public interface IDialogService
{
    void ShowInfo(string title, string message);
    void ShowError(string title, string message);
    Task<bool> ShowConfirmationDialog(string title, string message);
}
