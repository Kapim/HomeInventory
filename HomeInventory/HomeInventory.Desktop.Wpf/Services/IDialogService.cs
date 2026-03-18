using System;
using System.Collections.Generic;
using System.Text;

namespace HomeInventory.Desktop.Wpf.Services
{
    public interface IDialogService
    {
        void ShowInfo(string title, string message);
        void ShowError(string title, string message);
        bool ShowConfirmationDialog(string title, string message);
    }
}
