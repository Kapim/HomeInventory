using HomeInventory.Desktop.Wpf.Services;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Text;

namespace HomeInventory.Desktop.Wpf.Tests
{
    public class MockNotificationsService : INotificationsService
    {
        public ISnackbarMessageQueue SnackbarMessageQueue => throw new NotImplementedException();

        public List<string> Messages { get; } = [];
        public int SuccessCalls = 0;
        public int ErrorCalls = 0;
        public void Error(string text)
        {
            Messages.Add(text);
            ++ErrorCalls;
        }

        public void Success(string text)
        {
            Messages.Add(text);
            ++SuccessCalls;
        }
    }
}
