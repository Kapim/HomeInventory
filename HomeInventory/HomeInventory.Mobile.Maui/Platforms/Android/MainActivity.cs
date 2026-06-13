using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;

namespace HomeInventory.Mobile.Maui
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode |
                               ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            AndroidEnvironment.UnhandledExceptionRaiser += OnAndroidUnhandledException;
        }

        private void OnAndroidUnhandledException(object? sender, RaiseThrowableEventArgs e)
        {
            var ex = e.Exception;
            var msg = ex.ToString();

            // Write to logcat with clear tag so user can filter
            Android.Util.Log.Error("HOMEINV_CRASH", msg);

            // Write to file in app private storage for easy retrieval
            try
            {
                var path = System.IO.Path.Combine(
                    System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal),
                    "crash_log.txt");
                System.IO.File.WriteAllText(path, $"{DateTime.Now}\n{msg}");
                Android.Util.Log.Info("HOMEINV_CRASH", $"Crash log written to: {path}");
            }
            catch { /* ignore write errors */ }

            e.Handled = false; // let the app die normally (don't mask the crash)
        }
    }
}
