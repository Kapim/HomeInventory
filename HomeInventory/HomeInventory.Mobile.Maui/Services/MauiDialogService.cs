namespace HomeInventory.Mobile.Maui.Services;

public class MauiDialogService : IDialogService
{
    public void ShowInfo(string title, string message) => ShowAlert(title, message);

    public void ShowError(string title, string message) => ShowAlert(title, message);

    public async Task<bool> ShowConfirmationDialog(string title, string message)
    {
        if (Shell.Current?.CurrentPage is Page page)
            return await page.DisplayAlertAsync(title, message, "Ano", "Ne");
        return false;
    }

    private static void ShowAlert(string title, string message)
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            if (Shell.Current?.CurrentPage is Page page)
                await page.DisplayAlertAsync(title, message, "OK");
        });
    }
}
