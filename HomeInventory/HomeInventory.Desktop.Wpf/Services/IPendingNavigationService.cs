namespace HomeInventory.Desktop.Wpf.Services
{
    public interface IPendingNavigationService
    {
        Guid? PendingLocationId { get; set; }
    }

    public class PendingNavigationService : IPendingNavigationService
    {
        public Guid? PendingLocationId { get; set; }
    }
}
