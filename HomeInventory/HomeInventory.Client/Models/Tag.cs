namespace HomeInventory.Client.Models
{
    public class Tag(Guid id, string name, string color)
    {
        public Guid Id { get; } = id;
        public string Name { get; } = name;
        public string Color { get; } = color;
    }
}
