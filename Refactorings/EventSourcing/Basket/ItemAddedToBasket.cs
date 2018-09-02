using System;

namespace EventSourcing
{
    public class ItemAddedToBasket : IEvent
    {
        public SKU Sku { get; }
        public Money Cost { get; }
        public int Quantity { get; }
        public DateTime AddedAt { get; }

        public ItemAddedToBasket(SKU sku, Money cost, int quantity, DateTime addedAt)
        {
            Sku = sku;
            Cost = cost;
            Quantity = quantity;
            AddedAt = addedAt;
            PId = System.Diagnostics.Process.GetCurrentProcess().Id;
        }

        public int PId { get; }
    }
}