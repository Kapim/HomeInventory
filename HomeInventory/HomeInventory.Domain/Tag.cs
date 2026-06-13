namespace HomeInventory.Domain
{
    public class Tag
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public string Name { get; private set; } = null!;
        public string Color { get; private set; } = "#6B7280";
        public Guid HouseholdId { get; private set; }
        public List<Item> Items { get; } = [];

        private Tag() { }

        public Tag(string name, string color, Guid householdId)
        {
            SetName(name);
            SetColor(color);
            HouseholdId = householdId;
        }

        public void SetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Tag name must be set");
            if (name.Length > 30)
                throw new ArgumentOutOfRangeException(nameof(name), "Tag name must not exceed 30 characters");
            Name = name.Trim();
        }

        public void SetColor(string color)
        {
            if (string.IsNullOrWhiteSpace(color))
                throw new ArgumentException("Tag color must be set");
            Color = color.Trim();
        }
    }
}
