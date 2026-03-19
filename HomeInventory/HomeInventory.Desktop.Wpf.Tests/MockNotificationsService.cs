using HomeInventory.Desktop.Wpf.Services;
using MaterialDesignThemes.Wpf;

namespace HomeInventory.Desktop.Wpf.Tests
{
    public class MockNotificationsService : INotificationsService
    {
        public ISnackbarMessageQueue SnackbarMessageQueue => null!;

        public List<string> Messages { get; } = [];
        public int SuccessCalls { get; private set; }
        public int WarningCalls { get; private set; }
        public int ErrorCalls { get; private set; }

        public void Error(string text)
        {
            Messages.Add(text);
            ErrorCalls++;
        }

        public void Success(string text)
        {
            Messages.Add(text);
            SuccessCalls++;
        }

        public void Warning(string text)
        {
            Messages.Add(text);
            WarningCalls++;
        }
    }
}
