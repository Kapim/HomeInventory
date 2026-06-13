namespace HomeInventory.Mobile.Maui.Services;

public interface INotificationsService
{
    void Success(string text);
    void Warning(string text);
    void Error(string text);
}
