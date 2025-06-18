namespace ERP.Domain.Entities
{
    public class Inventory
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int WarehouseId { get; set; }
        public int Quantity { get; set; } = 0;
        public int MinimumStock { get; set; } = 0;
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public virtual Product Product { get; set; } = null!;
        public virtual Warehouse Warehouse { get; set; } = null!;
    }
}
