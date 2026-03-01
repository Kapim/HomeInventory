using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace HomeInventory.Desktop.Wpf.Services
{
    public sealed class DialogService : IDialogService
    {
        public void ShowInfo(string title, string message)
        {
            MessageBox.Show(message, title);
        }

        //TODO: change to some error dialog
        public void ShowError(string title, string message)
        {
            ShowInfo(title, message);
        }
    }
}
