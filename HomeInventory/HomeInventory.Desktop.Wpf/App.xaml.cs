using HomeInventory.Desktop.Wpf.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Configuration;
using System.Data;
using System.Windows;
using HomeInventory.Desktop.Wpf.Views;
using HomeInventory.Desktop.Wpf.Services;
using HomeInventory.Desktop.Wpf.Services.Navigation;
using HomeInventory.Client.Services;


namespace HomeInventory.Desktop.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IHost? _host;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            _host = Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddSingleton<ShellWindow>();
                    services.AddSingleton<ShellWindowViewModel>();
                    services.AddTransient<LoginViewModel>();
                    services.AddTransient<ItemsSearchViewModel>();

                    services.AddSingleton<IDialogService, DialogService>();
                    services.AddSingleton<INavigationService, NavigationService>();
                    //services.AddSingleton<IAuthService, HttpAuthService>();
                    services.AddHttpClient<IAuthService, HttpAuthService>(client =>
                    {
                        client.BaseAddress = new Uri("http://localhost:5046/");
                    });
                })
                .Build();

            var shell = _host.Services.GetRequiredService<ShellWindow>();
            shell.DataContext = _host.Services.GetRequiredService<ShellWindowViewModel>();
            shell.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _host?.Dispose();
            base.OnExit(e);            
        }
    }

}
