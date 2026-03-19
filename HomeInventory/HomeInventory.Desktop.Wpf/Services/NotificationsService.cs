using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Text;

namespace HomeInventory.Desktop.Wpf.Services
{
    public class NotificationsService : INotificationsService
    {
        public ISnackbarMessageQueue SnackbarMessageQueue => new SnackbarMessageQueue(TimeSpan.FromSeconds(2));

        public void Error(string text)
        {
            SnackbarMessageQueue.Enqueue(text);
        }

        public void Success(string text)
        {
            SnackbarMessageQueue.Enqueue(text);
        }
    }
}
