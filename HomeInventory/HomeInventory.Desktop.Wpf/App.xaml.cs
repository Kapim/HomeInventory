using HomeInventory.Client;
using HomeInventory.Client.Auth;
using HomeInventory.Client.Http;
using HomeInventory.Desktop.Wpf.Services;
using HomeInventory.Desktop.Wpf.Services.Navigation;
using HomeInventory.Desktop.Wpf.ViewModels;
using HomeInventory.Desktop.Wpf.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Configuration;
using System.Data;
using System.Windows;

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
                    services.AddTransient<MainViewModel>();
                    services.AddTransient<TopBarViewModel>();
                    services.AddTransient<ItemsSearchViewModel>();
                    

                    services.AddSingleton<IDialogService, DialogService>();
                    services.AddSingleton<INavigationService, NavigationService>();

                    services.AddHomeInventoryClient(new Uri("http://localhost:5046/"));

                    services.AddSingleton<ITokenStore, InMemoryTokenStore>();
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
