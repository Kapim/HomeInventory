using HomeInventory.Desktop.Wpf.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace HomeInventory.Wpf.Tests
{
    public class MockDialogService : IDialogService
    {
        public List<string> Messages = [];
        public void ShowError(string title, string message)
        {
            Messages.Add(message);
        }

        public void ShowInfo(string title, string message)
        {
            Messages.Add(message);
        }
    }
}
