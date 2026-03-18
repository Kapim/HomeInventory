using HomeInventory.Desktop.Wpf.Enums;
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

        public DialogResult ShowConfirmationDialog(string title, string message)
        {
            var result = MessageBox.Show(message, title, MessageBoxButton.YesNoCancel);
            return result switch
            {
                MessageBoxResult.Yes => DialogResult.Yes,
                MessageBoxResult.No => DialogResult.No,
                _ => DialogResult.Cancel,
            };
        }
    }
}
