using HomeInventory.Client.Models;
using HomeInventory.Desktop.Wpf.Enums;

namespace HomeInventory.Desktop.Wpf.Services
{
    public interface IDialogService
    {
        void ShowInfo(string title, string message);
        void ShowError(string title, string message);
        DialogResult ShowConfirmationDialog(string title, string message);
        TagPickerResult? ShowTagPicker(string itemName, IReadOnlyList<Tag> allTags, IReadOnlyList<Guid> currentTagIds);
        TagPickerResult? ShowManageHouseholdTags(IReadOnlyList<Tag> allTags);
    }
}
