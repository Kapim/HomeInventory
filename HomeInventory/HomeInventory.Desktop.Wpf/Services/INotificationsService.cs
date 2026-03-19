
using MaterialDesignThemes.Wpf;

namespace HomeInventory.Desktop.Wpf.Services
{
    public interface INotificationsService
    {
        public ISnackbarMessageQueue SnackbarMessageQueue { get; }
        public void Success(string text);
        public void Error(string text);
    }
}
