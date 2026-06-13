namespace HomeInventory.Mobile.Maui.Services;

public class MauiNotificationsService : INotificationsService
{
    public void Success(string text)
        => System.Diagnostics.Debug.WriteLine($"[✓] {text}");

    public void Warning(string text)
        => ShowAlert("Varování", text);

    public void Error(string text)
        => ShowAlert("Chyba", text);

    private static void ShowAlert(string title, string text)
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            if (Shell.Current?.CurrentPage is Page page)
                await page.DisplayAlertAsync(title, text, "OK");
        });
    }
}
