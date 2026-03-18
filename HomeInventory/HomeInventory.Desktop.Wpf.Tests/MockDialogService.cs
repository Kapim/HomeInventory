using HomeInventory.Desktop.Wpf.Enums;
using HomeInventory.Desktop.Wpf.Services;

namespace HomeInventory.Wpf.Tests
{
    public class MockDialogService : IDialogService
    {
        public List<string> Messages { get; } = [];
        public DialogResult NextConfirmationResult { get; set; } = DialogResult.Yes;
        public int ConfirmationDialogCalls { get; private set; }

        public void ShowError(string title, string message)
        {
            Messages.Add(message);
        }

        public void ShowInfo(string title, string message)
        {
            Messages.Add(message);
        }

        public DialogResult ShowConfirmationDialog(string title, string message)
        {
            ConfirmationDialogCalls++;
            return NextConfirmationResult;
        }
    }
}
