namespace ERP.Domain.Entities
{
    public class Warehouse
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Location { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Property
        public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();
    }
}
