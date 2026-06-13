using System.Diagnostics;

namespace HomeInventory.Mobile.Maui;

public partial class App : Application
{
    private readonly AppShell _shell;

    public App(AppShell shell)
    {
        AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
        TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;

        InitializeComponent();
        _shell = shell;
    }

    protected override Window CreateWindow(IActivationState? activationState)
        => new Window(_shell);

    private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        var ex = e.ExceptionObject as Exception;
        var msg = ex?.ToString() ?? e.ExceptionObject?.ToString() ?? "unknown";
        Debug.WriteLine($"[CRASH] UnhandledException: {msg}");
        ShowFatalAlert(ex?.Message ?? msg, ex?.StackTrace);
    }

    private static void OnUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
    {
        e.SetObserved();
        var msg = e.Exception.ToString();
        Debug.WriteLine($"[CRASH] UnobservedTaskException: {msg}");
        ShowFatalAlert(e.Exception.InnerException?.Message ?? msg, e.Exception.InnerException?.StackTrace);
    }

    private static void ShowFatalAlert(string message, string? stackTrace)
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            try
            {
                var detail = stackTrace != null ? $"\n\n{stackTrace[..Math.Min(300, stackTrace.Length)]}" : "";
                if (Application.Current?.Windows.Count > 0)
                {
                    var page = Shell.Current?.CurrentPage
                               ?? Application.Current.Windows[0].Page;
                    if (page != null)
                        await page.DisplayAlertAsync("Chyba aplikace", message + detail, "OK");
                }
            }
            catch { /* ignore errors in the error handler */ }
        });
    }
}
