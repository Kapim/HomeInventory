using HomeInventory.Client.Models;
using HomeInventory.Desktop.Wpf.Enums;
using HomeInventory.Desktop.Wpf.Views;
using System.Windows;

namespace HomeInventory.Desktop.Wpf.Services
{
    public sealed class DialogService : IDialogService
    {
        public void ShowInfo(string title, string message)
            => MessageBox.Show(message, title);

        public void ShowError(string title, string message)
            => ShowInfo(title, message);

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

        public TagPickerResult? ShowTagPicker(string itemName, IReadOnlyList<Tag> allTags, IReadOnlyList<Guid> currentTagIds)
        {
            var window = new TagPickerWindow($"Štítky pro: {itemName}", allTags, currentTagIds, itemMode: true)
            {
                Owner = Application.Current.MainWindow
            };
            return window.ShowDialog() == true ? window.Result : null;
        }

        public TagPickerResult? ShowManageHouseholdTags(IReadOnlyList<Tag> allTags)
        {
            var window = new TagPickerWindow("Správa štítků domácnosti", allTags, [], itemMode: false)
            {
                Owner = Application.Current.MainWindow
            };
            return window.ShowDialog() == true ? window.Result : null;
        }
    }
}
